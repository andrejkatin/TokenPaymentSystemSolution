using System.Collections.Concurrent;
using TransactionEFDAL.DataAccess;
using TransactionApi.Utils;
using TransactionApi.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Dynamic;

namespace TransactionApi.HostedServices
{
    public class TransactionPoolHostedService : IHostedService, IDisposable
    {
        private readonly TransactionDbContext Context;
        private readonly IHttpClientFactory HttpClientFactory;
        private static readonly ConcurrentBag<TransactionToProcess> TransactionPool = new ConcurrentBag<TransactionToProcess>();
        private static Timer Timer;

        public TransactionPoolHostedService(TransactionDbContext context, IHttpClientFactory httpClientFactory)
        {
            Context = context;
            HttpClientFactory = httpClientFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Timer = new Timer(ProcessTransactions, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void AddTransaction(TransactionToProcess transactionToProcess)
        {
            TransactionPool.Add(transactionToProcess);
        }

        private async void ProcessTransactions(object state)
        {
            var transactionsToProcess = new List<TransactionToProcess>();

            while (TransactionPool.TryTake(out var transaction))
            {
                transactionsToProcess.Add(transaction);
            }

            if (transactionsToProcess.Count > 0)
            {
                var transactionsToGenerateMerkleRoot = transactionsToProcess.Select(t => t.Transaction.TransactionSignature).ToList();
                var merkleRoot = MerkleRoot.GenerateMerkleRoot(transactionsToGenerateMerkleRoot);

                foreach (var transactionToProcess in transactionsToProcess)
                {
                    //create proof
                    var proof = MerkleRoot.GetMerklePath(transactionsToGenerateMerkleRoot, transactionToProcess.Transaction.TransactionSignature, merkleRoot);

                    //add proof for new token from
                    if (transactionToProcess.CreatedTokens.TokenFrom is not null)
                    {
                        dynamic tokenDataFrom = JsonConvert.DeserializeObject<ExpandoObject>(transactionToProcess.CreatedTokens.TokenFrom.TokenData);
                        tokenDataFrom.TokenProof = JsonConvert.SerializeObject(proof);
                        transactionToProcess.CreatedTokens.TokenFrom.TokenData = JsonConvert.SerializeObject(tokenDataFrom);
                    }

                    //add proof for new token to
                    if (transactionToProcess.CreatedTokens.TokenTo is not null)
                    {
                        dynamic tokenDataTo = JsonConvert.DeserializeObject<ExpandoObject>(transactionToProcess.CreatedTokens.TokenTo.TokenData);
                        tokenDataTo.TokenProof = JsonConvert.SerializeObject(proof);
                        transactionToProcess.CreatedTokens.TokenTo.TokenData = JsonConvert.SerializeObject(tokenDataTo);
                    }

                    var client = HttpClientFactory.CreateClient("WalletApi");
                    var mediaType = new MediaTypeHeaderValue("application/json");
                    StringContent content = new StringContent(JsonConvert.SerializeObject(transactionToProcess.CreatedTokens), mediaType);

                    var response = client.PostAsync("/api/wallet/updateTokensByTransaction", content);
                    await Context.Transactions.AddAsync(transactionToProcess.Transaction);
                }

                await Context.SaveChangesAsync();             
            }
        }

        public void Dispose()
        {
            Timer?.Dispose();
        }
    }
}
