using TokenApi.Utils.HashFunctions;
using TokenApi.Utils.Extensions;
using Newtonsoft.Json;
using TokenApi.Dtos.TokenDtos;
using TokenApi.Dtos.Enums;

namespace TokenApi.Models.TokenVersions.V1
{
    public class TokenV1 : IToken
    {
        private static readonly Func<byte[], int> HashFunction = HashFunctionV1.ComputeHash;

        public Guid TokenId;

        public int TokenIdHash { get; set; }

        public TokenDataV1 TokenData;

        public int TokenDataSignature { get; set; }

        public Guid WalletAddress { get; set; }

        public int WalletAddressHash { get; set; }

        public TokenV1(TokenDto token)
        {
            TokenId = token.TokenId;
            TokenIdHash = token.TokenIdHash;
            TokenDataSignature = token.TokenDataSignature;
            WalletAddress = token.WalletAddress;
            WalletAddressHash = token.WalletAddressHash;
            TokenData = JsonConvert.DeserializeObject<TokenDataV1>(token.TokenData);
        }

        public TokenV1(Guid walletAddress, decimal balance, Guid transactionId, TokenTypeEnum tokenType)
        {
            TokenId = Guid.NewGuid();
            WalletAddress = walletAddress;
            TokenData = new TokenDataV1()
            {
                Balance = balance,
                Version = "1.0",
                Created = DateTime.UtcNow,
                TokenTypeId = tokenType,
                TransactionId = transactionId
            };
            CalculateHashValues();
        }

        public bool IsTokenValid()
        {
            bool isValid = true;
            if (TokenDataSignature != HashFunction(CreateIntArrayForSignatureHashCalculation().ToByteArray()))
            {
                isValid = false;
            }
            if (TokenIdHash != HashFunction(TokenId.ToByteArray()))
            {
                isValid = false;
            }
            if (WalletAddressHash != HashFunction(WalletAddress.ToByteArray()))
            {
                isValid = false;
            }
            if (TokenData.BalanceHash != HashFunction(TokenData.Balance.ToByteArray()))
            {
                isValid = false;
            }
            if (TokenData.VersionHash != HashFunction(TokenData.Version.ToByteArray()))
            {
                isValid = false;
            }
            if (TokenData.CreatedHash != HashFunction(TokenData.Created.ToByteArray()))
            {
                isValid = false;
            }
            if (TokenData.TokenTypeIdHash != HashFunction(TokenData.TokenTypeId.ToString().ToByteArray()))
            {
                isValid = false;
            }
            if (TokenData.TransactionIdHash != HashFunction(TokenData.TransactionId.ToByteArray()))
            {
                isValid = false;
            }
            return isValid;
        }

        public TokenDto ToTokenDto()
        {
            var tokenDto = new TokenDto()
            {
                TokenId = TokenId,
                TokenIdHash = TokenIdHash,
                TokenData = JsonConvert.SerializeObject(TokenData),
                TokenDataSignature = TokenDataSignature,
                WalletAddress = WalletAddress,
                WalletAddressHash = WalletAddressHash
            };
            return tokenDto;
        }

        private void CalculateHashValues()
        {
            TokenData.BalanceHash = HashFunction(TokenData.Balance.ToByteArray());
            TokenData.VersionHash = HashFunction(TokenData.Version.ToByteArray());
            TokenData.CreatedHash = HashFunction(TokenData.Created.ToByteArray());
            TokenData.TokenTypeIdHash = HashFunction(TokenData.TokenTypeId.ToString().ToByteArray());
            TokenData.TransactionIdHash = HashFunction(TokenData.TransactionId.ToByteArray());
            TokenIdHash = HashFunction(TokenId.ToByteArray());
            WalletAddressHash = HashFunction(WalletAddress.ToByteArray());
            TokenDataSignature = HashFunction(CreateIntArrayForSignatureHashCalculation().ToByteArray());
        }

        private int[] CreateIntArrayForSignatureHashCalculation()
        {
            return [
                TokenIdHash,
                WalletAddressHash,
                TokenData.BalanceHash,
                TokenData.VersionHash,
                TokenData.CreatedHash,
                TokenData.TokenTypeIdHash,
                TokenData.TransactionIdHash
            ];
        }
    }
}
