using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;

using Microsoft.Azure.Functions.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(acme.Function.Startup))]

namespace acme.Function.infra
{
    public class FunctionConfig
    {

      readonly ExecutionContext _context;
      IConfiguration _config;

      public string StorageConnectionString => _config["email_sender_1_log"];
      public string EmailSenderServiceUri => _config["email_sender_service_uri"];
      public string FuncApiKey => _config["func_api_key"];
      public string ApiKeyForEmailSenderService => _config["api_key_for_email_sender_service"];

      public string LocalSettings => "local.settings.json";
      public FunctionConfig(ExecutionContext context)
      {
        _context = context;

        _config = new ConfigurationBuilder()
            .SetBasePath(context.FunctionAppDirectory)
            .AddJsonFile(LocalSettings, optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
      }

    }


}
