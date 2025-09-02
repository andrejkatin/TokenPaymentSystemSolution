namespace WalletApi.Services
{
    public interface IAzureKeyVaultService
    {
        Task<string> GetSecret(string key);

        Task CreateSecret(string key, string value);
    }
}
