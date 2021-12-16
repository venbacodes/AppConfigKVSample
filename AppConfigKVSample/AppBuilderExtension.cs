using Azure.Identity;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;

namespace AppConfigKVSample
{
    public static class AppBuilderExtension
    {
        private static readonly string AppConfigConnectionStringSection = "Appconfig:ConnectionString";

        public static void UseAzureAppConfiguration(this WebApplicationBuilder builder, ILogger logger)
        {
            try
            {
                builder.Host.ConfigureAppConfiguration((webHostBuilderContext, config) =>
                {
                    var isDevelopment = webHostBuilderContext.HostingEnvironment.IsDevelopment();
                    var environment = webHostBuilderContext.HostingEnvironment.EnvironmentName.ToLower();

                    config
                    .AddJsonFile("appsettings.json", false, true)
                    .AddJsonFile($"appsettings.{webHostBuilderContext.HostingEnvironment.EnvironmentName}.json", true, true);

                    var settings = config.Build();

                    var appConfigConnectionString = settings.GetSection(AppConfigConnectionStringSection);                    

                    if (appConfigConnectionString != null && !string.IsNullOrWhiteSpace(appConfigConnectionString.Value))
                    {
                        config.AddAzureAppConfiguration(options =>
                        {
                            options
                            .Connect(appConfigConnectionString.Value)
                            .ConfigureKeyVault(kv =>
                            {
                                if (isDevelopment)
                                {
                                    ///You WON'T need this if you are logged into Visual Studio and your account has been setup 
                                    ///with access policy in the Key Vault you are trying to connect. You can just use the DefaultAzureCredential
                                    ///that is set in the else part.
                                    var cred = new ClientSecretCredential(
                                        settings.GetSection("DevCredential:TenantId").Value,
                                        settings.GetSection("DevCredential:ClientId").Value,
                                        settings.GetSection("DevCredential:ClientSecret").Value);

                                    kv.SetCredential(cred);
                                }
                                else
                                {
                                    kv.SetCredential(new DefaultAzureCredential());
                                }
                            })
                            .Select(KeyFilter.Any, LabelFilter.Null)
                            .Select(KeyFilter.Any, environment ?? LabelFilter.Null);
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in reading app configuration. Message: {ex.Message}", ex);
            }
        }

    }
}