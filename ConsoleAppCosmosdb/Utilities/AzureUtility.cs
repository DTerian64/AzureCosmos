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
            string kvUri = $"https://{keyVaultName}.vault.azure.net";

            var client = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());
            KeyVaultSecret secret = await client.GetSecretAsync(secretName);

            return secret.Value;
        }
        public static string GetEnvironmentVariable(string variable)
        {
            var value = Environment.GetEnvironmentVariable(variable);
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException($"Environment variable '{variable}' is not set.");
            }
            return value;
        }
    }
}