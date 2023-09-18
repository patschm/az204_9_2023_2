
using System;
using System.Threading.Tasks;

#region KeyVault
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;
using Azure.Security.KeyVault.Secrets;
using Azure.Security.KeyVault;
#endregion

#region AppConfiguration
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Azure.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using Microsoft.Extensions.Azure;
#endregion


namespace KeyVault
{
    class Program
    {
        static string tenentId = "030b09d5-7f0f-40b0-8c01-03ac319b2d71";
        static string clientId = "85281bd4-e07c-4bb6-865f-033b85fa74e6";
        static string clientSecret = "bSk8Q~omRIOgt12MOOE.E2UnUx5KoF_LTBbPxdA.";
        static string kvUri = "https://ps-keybos.vault.azure.net/";
        
        static async Task Main(string[] args)
        {
            //await ReadKeyVault();
            await ReadConfigurationLocalAsync();
            //await ReadAppConfigurationAsync();

            Console.WriteLine("Done");
            Console.ReadLine();
        }
        private static async Task ReadKeyVault()
        {
            ClientSecretCredential cred = new ClientSecretCredential(tenentId, clientId, clientSecret);
            //DefaultAzureCredential cred = new DefaultAzureCredential();
            SecretClient kvClient = new SecretClient(new Uri(kvUri), cred);

            var result = await kvClient.GetSecretAsync("mijngeheim");
            Console.WriteLine($"Hello: {result.Value?.Value}");
        }
        private static Task ReadConfigurationLocalAsync()
        {
            // From user-secrets
            // dotnet init
            // dotnet user-secrets set APPC "Endpoint=https://ps-config.azconfig.io;Id=LDpu;Secret=zE8JVpCLRSBwkKyE+sHBE1uPnexZTenYGpvY2eQoY04="
            // var constr = configuration["APPC"];
            // Console.WriteLine(  constr);
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json")
                    .AddUserSecrets<KeyVault.Program>(true)
                   .AddEnvironmentVariables();
            IConfiguration configuration = builder.Build();

            Console.WriteLine(configuration["MySetings:hello"]);
            Console.WriteLine(configuration["KlantA:KeyVault:BackgroundColor"]);
            Console.WriteLine(configuration["ConnectionString"]);
            return Task.CompletedTask;
        }

        private static async Task ReadAppConfigurationAsync()
        {
            var builder = new ConfigurationBuilder();
            //ClientSecretCredential cred = new ClientSecretCredential(tenentId, clientId, clientSecret);
            //var env = Environment.GetEnvironmentVariable("Bla");
            // builder.AddAzureKeyVault(new Uri(kvUri), cred);
            builder.AddAzureAppConfiguration(opts => {
                    //opts.ConfigureKeyVault(kvopts =>
                    //{
                    //    kvopts.SetCredential(new ClientSecretCredential(tenentId, clientId, clientSecret));
                    //});

                    opts.Connect("Endpoint=https://ps-config.azconfig.io;Id=wqyc;Secret=Dkmh9tT9TsWUqK9XWlCH6eoI+I5szowYmkzXkuCO5Zo=")
                       .Select(KeyFilter.Any, "Development");
                });
                IConfiguration conf = builder.Build();
                
                Console.WriteLine($"{conf["KeyVault:Test:MyConfig"]}");
                Console.WriteLine($"Hello {conf["ThaKey"]}");
                //Console.WriteLine(conf["KlantA:KeyVault:BackgroundColor"]);

                IServiceCollection services = new ServiceCollection();
                services.AddFeatureManagement(conf);
                //services.AddSingleton<IConfiguration>(conf).AddFeatureManagement();

                using (var svcProvider = services.BuildServiceProvider())
                {
                    do
                    {
                        using (var scope = svcProvider.CreateScope())
                        {
                            var featureManager = scope.ServiceProvider.GetRequiredService<IFeatureManager>();

                            if (await featureManager.IsEnabledAsync("FeatureA"))
                            {
                                Console.WriteLine("We have a new feature");
                            }
                            Console.Write(".");
                            await Task.Delay(2000);
                        }
                    }
                    while (true);
                }
            }

        private static async Task ReadAppFeaturesAsync()
        {
            var builder = new ConfigurationBuilder();
            builder.AddAzureAppConfiguration(opts => {
                opts.Connect("Endpoint=https://ps-config.azconfig.io;Id=wqyc;Secret=Dkmh9tT9TsWUqK9XWlCH6eoI+I5szowYmkzXkuCO5Zo=")
                   .UseFeatureFlags(opts =>
                   {
                       opts.CacheExpirationInterval = TimeSpan.FromSeconds(1);
                       opts.Label = "Production";
                   });
            });
            IConfiguration conf = builder.Build();

            IServiceCollection services = new ServiceCollection();
            services.AddFeatureManagement(conf);
            services.AddSingleton<IConfiguration>(conf).AddFeatureManagement();

            using (var svcProvider = services.BuildServiceProvider())
            {
                do
                {
                    using (var scope = svcProvider.CreateScope())
                    {
                        var featureManager = scope.ServiceProvider.GetRequiredService<IFeatureManager>();

                        if (await featureManager.IsEnabledAsync("FeatureA"))
                        {
                            Console.WriteLine("We have a new feature");
                        }
                        Console.Write(".");
                        await Task.Delay(2000);
                    }
                }
                while (true);
            }
        }
    }
}
