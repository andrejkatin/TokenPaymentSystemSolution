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
using System.Text;
using System.Threading;

namespace TransactionApi.Services.Impl
{
    public class TransactionService : ITransactionService
    {
        private readonly TransactionDbContext Context;
        private readonly TransactionPoolHostedService TransactionPoolHostedService;
        private readonly IHttpClientFactory HttpClientFactory;

        // Konfigurable defaults (mogu se povući iz configa u kasnijem commitu)
        private static readonly TimeSpan _httpTimeout = TimeSpan.FromSeconds(10);
        private const int _maxAttempts = 2;
        private static readonly TimeSpan _retryDelay = TimeSpan.FromMilliseconds(200);

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

            transactionToProcess.Transaction.TransactionSignature = HashFunctions.ComputeSha256Hash(JsonConvert.SerializeObject(transactionToProcess.Transaction));
            transactionToProcess.CreatedTokens = newTokens;

            return transactionToProcess;
        }

        public async Task<TokenDto> CreateNewToken(Guid walletAddress, Guid transactionId, TransactionTypeEnum transactionType, TokenDto currentToken, decimal amount, TokenTypeEnum tokenType)
        {
            string url = $"/api/token/createNewToken/{walletAddress}/{transactionId}/{transactionType}/{tokenType}/{amount}";
            return await PostJsonAsync<TokenDto>("TokenApi", url, currentToken).ConfigureAwait(false);
        }

        public async Task<TokensForTransactionDto> CreateNewTokens(Guid transactionId, TokensForTransactionDto currentTokens, decimal amount, TokenTypeEnum tokenType)
        {
            string url = $"/api/token/createNewTokens/{transactionId}/{tokenType}/{amount}";
            return await PostJsonAsync<TokensForTransactionDto>("TokenApi", url, currentTokens).ConfigureAwait(false);
        }

        public async Task<TokensForTransactionDto> GetTokensForTransaction(TokenTypeEnum tokenType, string from, string to)
        {
            string url = $"/api/wallet/getTokensForTransaction/{tokenType}/{from}/{to}";
            return await GetJsonAsync<TokensForTransactionDto>("WalletApi", url).ConfigureAwait(false);
        }

        public async Task<TokenDto> GetTokenForTransaction(TokenTypeEnum tokenType, string address)
        {
            string url = $"/api/wallet/getTokenForTransaction/{tokenType}/{address}";
            return await GetJsonAsync<TokenDto>("WalletApi", url).ConfigureAwait(false);
        }

        private async Task<T> PostJsonAsync<T>(string clientName, string url, object payload, CancellationToken cancellationToken = default)
        {
            var client = HttpClientFactory.CreateClient(clientName);
            var json = JsonConvert.SerializeObject(payload);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            int attempts = 0;
            while (true)
            {
                attempts++;
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                cts.CancelAfter(_httpTimeout);

                try
                {
                    using var response = await client.PostAsync(url, content, cts.Token).ConfigureAwait(false);
                    var respContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    if (!response.IsSuccessStatusCode)
                    {
                        if ((int)response.StatusCode >= 500 && attempts < _maxAttempts)
                        {
                            await Task.Delay(_retryDelay).ConfigureAwait(false);
                            continue;
                        }

                        throw new ExternalApiException(clientName, url, response.StatusCode, respContent);
                    }

                    return JsonConvert.DeserializeObject<T>(respContent);
                }
                catch (HttpRequestException ex) when (attempts < _maxAttempts)
                {
                    await Task.Delay(_retryDelay).ConfigureAwait(false);
                    continue;
                }
                catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested && attempts < _maxAttempts)
                {
                    await Task.Delay(_retryDelay).ConfigureAwait(false);
                    continue;
                }
            }
        }

        private async Task<T> GetJsonAsync<T>(string clientName, string url, CancellationToken cancellationToken = default)
        {
            var client = HttpClientFactory.CreateClient(clientName);

            int attempts = 0;
            while (true)
            {
                attempts++;
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                cts.CancelAfter(_httpTimeout);

                try
                {
                    using var response = await client.GetAsync(url, cts.Token).ConfigureAwait(false);
                    var respContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    if (!response.IsSuccessStatusCode)
                    {
                        if ((int)response.StatusCode >= 500 && attempts < _maxAttempts)
                        {
                            await Task.Delay(_retryDelay).ConfigureAwait(false);
                            continue;
                        }

                        throw new ExternalApiException(clientName, url, response.StatusCode, respContent);
                    }

                    return JsonConvert.DeserializeObject<T>(respContent);
                }
                catch (HttpRequestException ex) when (attempts < _maxAttempts)
                {
                    await Task.Delay(_retryDelay).ConfigureAwait(false);
                    continue;
                }
                catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested && attempts < _maxAttempts)
                {
                    await Task.Delay(_retryDelay).ConfigureAwait(false);
                    continue;
                }
            }
        }
    }

    public class ExternalApiException : Exception
    {
        public string ClientName { get; }
        public string Url { get; }
        public HttpStatusCode? StatusCode { get; }
        public string ResponseContent { get; }

        public ExternalApiException(string clientName, string url, HttpStatusCode? statusCode, string responseContent)
            : base($"External API call to '{clientName}' ({url}) failed with status {(statusCode.HasValue ? ((int)statusCode).ToString() : "N/A")}")
        {
            ClientName = clientName;
            Url = url;
            StatusCode = statusCode;
            ResponseContent = responseContent;
        }
    }
}
