using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Azure;
using WalletApi.Exceptions.CustomExceptions;
using WalletApi.Exceptions.ExceptionErrorCodes;

namespace WalletApi.Services.Impl
{
    public class AzureKeyVaultService : IAzureKeyVaultService
    {
        private readonly SecretClient SecretClient;
        public AzureKeyVaultService(SecretClient secretClient)
        {
            SecretClient = secretClient;
        }

        public async Task<string> GetSecret(string key)
        {
            try
            {
                var secret = (await SecretClient.GetSecretAsync(key)).Value.Value;
                return secret;
            }
            catch (Azure.RequestFailedException)
            {
                return null;
            }            
        }

        public async Task CreateSecret(string key, string value)
        {
             await SecretClient.SetSecretAsync(new KeyVaultSecret(key, value));
        }
    }
}
