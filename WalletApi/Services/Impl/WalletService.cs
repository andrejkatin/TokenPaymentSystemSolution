using WalletEFDAL.Models;
using WalletEFDAL.DataAccess;
using Microsoft.EntityFrameworkCore;
using WalletApi.Dtos.WalletDtos;
using AutoMapper;
using WalletApi.Exceptions.CustomExceptions;
using WalletApi.Exceptions.ExceptionErrorCodes;
using Newtonsoft.Json;
using System.Dynamic;
using WalletApi.Dtos.Enums;

namespace WalletApi.Services.Impl
{
    public class WalletService : IWalletService
    {
        private readonly WalletDbContext Context;
        private readonly IAzureKeyVaultService AzureKeyVaultService;
        private readonly IMapper Mapper;

        public WalletService(WalletDbContext context, IAzureKeyVaultService azureKeyVaultService, IMapper mapper)
        {
            Context = context;
            AzureKeyVaultService = azureKeyVaultService;
            Mapper = mapper;
        }

        public async Task<WalletDto> GetWallet(string sub)
        {
            var walletIdFromSecret = await AzureKeyVaultService.GetSecret(sub);

            Guid.TryParse(walletIdFromSecret, out Guid walletId);

            var wallet = await Context.Wallets
                .Include(w => w.Tokens)
                .FirstOrDefaultAsync(it => it.Address == walletId);

            WalletDto walletDto = new WalletDto() { Address = wallet.Address };
            foreach (var token in wallet.Tokens)
            {
                dynamic tokenData = JsonConvert.DeserializeObject<ExpandoObject>(token.TokenData);
                walletDto.Tokens.Add(new Dtos.TokenDtos.TokenDto()
                {
                    TokenId = token.TokenId,
                    Balance = (decimal)tokenData.Balance,
                    TokenType = (TokenTypeEnum)tokenData.TokenTypeId,
                });
            }

            return wallet is not null ? walletDto : null;
        }

        public async Task<(Guid walletAddressFrom, Guid WalletAddressTo)> GetWalletsForTransaction(string subFrom, string subTo)
        {
            var walletFrom = await GetWallet(subFrom);
            if (walletFrom is null)
            {
                throw new NotFoundException(NotFoundExceptionErrorCodes.WalletNotFound, "User that wants to pay does not have created wallet.");
            }

            var walletTo = await GetWallet(subTo);
            if (walletTo is null)
            {
                throw new NotFoundException(NotFoundExceptionErrorCodes.WalletNotFound, "User to be paid does not have created wallet.");
            }

            return (walletFrom.Address, walletTo.Address);
        }

        public async Task<WalletDto> CreateWallet(string sub)
        {
            var walletId = Guid.NewGuid();
            await AzureKeyVaultService.CreateSecret(sub, walletId.ToString());

            var wallet = Context.Wallets.Add(new Wallet { Address = walletId });
            await Context.SaveChangesAsync();

            return Mapper.Map<WalletDto>(wallet.Entity); ;
        }
    }
}
