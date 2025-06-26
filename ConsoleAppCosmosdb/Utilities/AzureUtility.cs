using System;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace ConsoleAppCosmosdb.Utilities
{

    public static class AzureUtility
    {
       public async static Task<string> GetRideshareCosmosPrimaryKey()
        {
            string keyVaultName = "RideshareKeyVault2";
            string secretName = "RIDESHARE-COSMOS-PRIMARY-KEY";            
            return await GetSecretFromKeyVault(keyVaultName, secretName);
            
        }
        public async static Task<string> GetRideshareCosmosEndPoint()
        {
            string keyVaultName = "RideshareKeyVault2";
            string secretName = "RIDESHARE-COSMOS-ENDPOINT";                        
            return await GetSecretFromKeyVault(keyVaultName, secretName);
        } 

        private async static Task<string> GetSecretFromKeyVault(string keyVaultName, string secretName)
        {
            string kvUri = $"https://{keyVaultName}.vault.azure.net";
            var client = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());
            KeyVaultSecret secret = await client.GetSecretAsync(secretName);
            return secret.Value;
        }
    }
}