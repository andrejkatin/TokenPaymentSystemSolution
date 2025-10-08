using TransactionApi.HostedServices;
using TransactionEFDAL.DataAccess;
using TransactionEFDAL.Models;
using Newtonsoft.Json;
using TransactionApi.Dtos.TokenDtos;
using TransactionEFDAL.Models.Enums;
using System.Net.Http.Headers;
using TransactionApi.Models;
using TransactionApi.Utils;
using System.Net;

namespace TransactionApi.Services.Impl
{
    public class TransactionService : ITransactionService 
    {
        private readonly TransactionDbContext Context;
        private readonly TransactionPoolHostedService TransactionPoolHostedService;
        private readonly IHttpClientFactory HttpClientFactory;

        public TransactionService(TransactionPoolHostedService transactionPoolHostedService, TransactionDbContext context, IHttpClientFactory httpClientFactory)
        {
            Context = context;
            TransactionPoolHostedService = transactionPoolHostedService;
            HttpClientFactory = httpClientFactory;
        }

        public void AddTransactionToPool(TransactionToProcess transactionToProcess)
        {
            TransactionPoolHostedService.AddTransaction(transactionToProcess);
        }

        public TransactionToProcess CreateTransactionToProcess(Guid transactionId, TransactionStatus status, string iniciator, TokensForTransactionDto currentTokens, TokensForTransactionDto newTokens, decimal amount, TokenTypeEnum tokenType)
        {
            TransactionToProcess transactionToProcess = new();
            transactionToProcess.Transaction = new Transaction()
            {
                TransactionId = transactionId,
                TransactionType = TransactionTypeEnum.Payment,
                TokenFromOld = JsonConvert.SerializeObject(currentTokens.TokenFrom),
                TokenToOld = JsonConvert.SerializeObject(currentTokens.TokenTo),
                TokenType = tokenType,
                Amount = amount,
                FeeFrom = 0,
                FeeTo = 0,
                Created = DateTime.UtcNow,
                TransactionStatus = status,
                TransactionIniciator = iniciator
            };

            transactionToProcess.Transaction.TransactionSignature = HashFunctions.ComputeMd5Hash(JsonConvert.SerializeObject(transactionToProcess.Transaction));
            transactionToProcess.CreatedTokens = newTokens;

            return transactionToProcess;
        }

        public async Task<TokenDto> CreateNewToken(Guid walletAddress, Guid transactionId, TransactionTypeEnum transactionType, TokenDto currentToken, decimal amount, TokenTypeEnum tokenType)
        {
            var client = HttpClientFactory.CreateClient("TokenApi");

            var mediaType = new MediaTypeHeaderValue("application/json");
            StringContent jsonContent = new(JsonConvert.SerializeObject(currentToken), mediaType);

            var response = await client.PostAsync("/api/token/createNewToken/" +
                walletAddress.ToString() + "/" +
                transactionId.ToString() + "/" +
                transactionType + "/" +
                tokenType + "/" +
                amount, jsonContent);

            var newTokens = JsonConvert.DeserializeObject<TokenDto>(await response.Content.ReadAsStringAsync());

            return newTokens;
        }

        public async Task<TokensForTransactionDto> CreateNewTokens(Guid transactionId, TokensForTransactionDto currentTokens, decimal amount, TokenTypeEnum tokenType)
        {
            var client = HttpClientFactory.CreateClient("TokenApi");

            var mediaType = new MediaTypeHeaderValue("application/json");
            StringContent jsonContent = new (JsonConvert.SerializeObject(currentTokens), mediaType);

            var response = await client.PostAsync("/api/token/createNewTokens/" +
                transactionId.ToString() + "/" +
                tokenType + "/" +
                amount, jsonContent);

            var newTokens = JsonConvert.DeserializeObject<TokensForTransactionDto>(await response.Content.ReadAsStringAsync());

            return newTokens;
        }

        public async Task<TokensForTransactionDto> GetTokensForTransaction(TokenTypeEnum tokenType, string from, string to)
        {
            var client = HttpClientFactory.CreateClient("WalletApi");

            var response = await client.GetAsync("/api/wallet/getTokensForTransaction/" +
                tokenType + "/" +
                from + "/" +
                to);

            var tokensForTransaction = JsonConvert.DeserializeObject<TokensForTransactionDto>(await response.Content.ReadAsStringAsync());

            return tokensForTransaction;
        }

        public async Task<TokenDto> GetTokenForTransaction(TokenTypeEnum tokenType, string address)
        {
            var client = HttpClientFactory.CreateClient("WalletApi");

            var response = await client.GetAsync("/api/wallet/getTokenForTransaction/" +
                tokenType + "/" +
                address);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception();
            }

            var token = JsonConvert.DeserializeObject<TokenDto>(await response.Content.ReadAsStringAsync());

            return token;
        }
    }
}
