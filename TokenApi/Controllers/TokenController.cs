using Microsoft.AspNetCore.Mvc;
using TokenApi.Dtos.Enums;
using TokenApi.Dtos.TokenDtos;
using TokenApi.Services;

namespace TokenApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly ITokenService TokenService;

        public TokenController(ITokenService tokenService)
        {
            TokenService = tokenService;
        }

        [HttpPost("createNewToken/{walletAddress}/{transactionId}/{transactionType}/{tokenType}/{amount}")]
        public async Task<ActionResult<TokenDto>> CreateNewToken([FromRoute] Guid walletAddress, [FromRoute] Guid transactionId, [FromRoute] TransactionTypeEnum transactionType, [FromRoute] TokenTypeEnum tokenType, [FromRoute] decimal amount, [FromBody] TokenDto currentToken)
        {
            if (transactionType != TransactionTypeEnum.Deposit && transactionType != TransactionTypeEnum.Withdraw)
            {
                return BadRequest("Wrong transaction type.");
            }
            var newToken = TokenService.CreateNewToken(walletAddress, transactionId, transactionType, tokenType, amount, currentToken);
            return Ok(newToken);
        }

        [HttpPost("createNewTokens/{transactionId}/{tokenType}/{amount}")]
        public async Task<ActionResult<TokensForTransactionDto>> CreateNewTokens([FromRoute] Guid transactionId, [FromRoute] TokenTypeEnum tokenType, [FromRoute] decimal amount, [FromBody] TokensForTransactionDto tokensForTransaction)
        {

            return Ok();
        }


        [HttpPost("validateToken")]
        public ActionResult<bool> ValidateToken(TokenDto token)
        {
            var result = TokenService.ValidateToken(token);
            return Ok(result);
        }
    }
}
