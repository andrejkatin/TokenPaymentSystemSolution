using Microsoft.AspNetCore.Mvc;
using WalletApi.Dtos.Enums;
using WalletApi.Dtos.TokenDtos;
using WalletApi.Dtos.WalletDtos;
using WalletApi.Exceptions.CustomExceptions;
using WalletApi.Exceptions.ExceptionErrorCodes;
using WalletApi.Services;

namespace WalletApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService WalletService;
        private readonly ITokenService TokenService;
        private readonly IRedisService RedisService;
        private const string LockedWalletsKey = "LockedWallets";

        public WalletController(IWalletService walletService, ITokenService tokenService, IRedisService redisService)
        {
            WalletService = walletService;
            TokenService = tokenService;
            RedisService = redisService;
        }

        [HttpGet("getWalletForCurrentUser")]
        public async Task<ActionResult<WalletDto>> GetWallet()
        {
            var sub = HttpContext.Request.Headers["sub"].FirstOrDefault();

            if (String.IsNullOrEmpty(sub))
            {
                throw new BadRequestException(BadRequestExceptionErrorCodes.SubKeyNotProvidedInHeader, "There is no sub key provided in requests header.");
            }

            var wallet = await WalletService.GetWallet(sub);

            if (wallet is null)
            {
                wallet = await WalletService.CreateWallet(sub);
            }

            return Ok(wallet);         
        }

        [HttpGet("getTokenForTransaction/{tokenType}/{sub}")]
        public async Task<ActionResult<UpdateTokenDto>> GetTokensForTransaction([FromRoute] string sub, [FromRoute] TokenTypeEnum tokenType)
        {
            var wallet = await WalletService.GetWallet(sub);
            if (wallet is null)
            {
                throw new NotFoundException(NotFoundExceptionErrorCodes.WalletNotFound, "User that wants to withdraw or deposit does not have created wallet.");
            }

            var lockedWallets = await RedisService.GetListAsync<string>(LockedWalletsKey);
            if (lockedWallets.Contains(wallet.Address.ToString()))
            {
                throw new ForbiddenException(ForbiddenExceptionErrorCodes.LockedWallet, "User wallet is locked by another transaction.");
            }

            var tokenForTransaction = await TokenService.GetTokenForTransaction(wallet.Address, tokenType);

            lockedWallets.Add(wallet.Address.ToString());
            await RedisService.StoreListAsync<string>(LockedWalletsKey, lockedWallets);

            return Ok(tokenForTransaction);
        }

        [HttpGet("getTokensForTransaction/{tokenType}/{from}/{to}")]
        public async Task<ActionResult<TokensForTransactionDto>> GetTokensForTransaction([FromRoute] string from, [FromRoute] string to, [FromRoute] TokenTypeEnum tokenType)
        {
            (Guid walletIdFrom, Guid walletIdTo) = await WalletService.GetWalletsForTransaction(from, to);

            var lockedWallets = await RedisService.GetListAsync<string>(LockedWalletsKey);
            if (lockedWallets.Any(w => w == walletIdFrom.ToString() || w == walletIdTo.ToString()))
            {
                throw new ForbiddenException(ForbiddenExceptionErrorCodes.LockedWallet, "Some of wallets are locked by another transaction.");
            }

            var tokensForTransaction = TokenService.GetTokensForTransaction(walletIdFrom, walletIdTo, tokenType);

            lockedWallets.Add(walletIdTo.ToString());
            lockedWallets.Add(walletIdFrom.ToString());
            await RedisService.StoreListAsync<string>(LockedWalletsKey, lockedWallets);

            return Ok(tokensForTransaction);
        }

        [HttpPost("updateTokensByTransaction")]
        public async Task<ActionResult> UpdateTokensByTransaction([FromBody] TokensForTransactionDto updatedTokens)
        {
            await TokenService.UpdateTokensByTransaction(updatedTokens.TokenFrom, updatedTokens.TokenTo);
            var lockedWallets = await RedisService.GetListAsync<string>(LockedWalletsKey);
            if (updatedTokens.TokenFrom is not null)
            {
                lockedWallets.Remove(updatedTokens.TokenFrom.WalletAddress.ToString());
            }
            if (updatedTokens.TokenTo is not null)
            {
                lockedWallets.Remove(updatedTokens.TokenTo.WalletAddress.ToString());
            }

            await RedisService.StoreListAsync(LockedWalletsKey, lockedWallets);
            return Ok();
        }
    }
}
