using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Text;

using Microsoft.Extensions.Primitives;

using acme.Function.infra;
using acme.Function.model;

[assembly: FunctionsStartup(typeof(acme.Function.Startup))]

namespace acme.Function
{
    public class Startup : FunctionsStartup
  {
      public override void Configure(IFunctionsHostBuilder builder)
      {
        builder.Services.AddHttpClient();
        builder.Services.AddSingleton<Utility>();
      }
    }

    public class EmailSender_HttpTrigger
    {

      private readonly HttpClient _httpClient;
        private readonly Utility _utility;

        public EmailSender_HttpTrigger(HttpClient httpClient, Utility utility)
      {
          _httpClient = httpClient;
          _utility = utility;
        }

      [FunctionName("email-sender-1-func")]
      public async Task<IActionResult> Run(
          [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
          ILogger log, ExecutionContext context)
      {

        FunctionConfig funcConfig =null;
        BlogLogger blobLogger=null;

        try{
          funcConfig = new FunctionConfig(context);
          blobLogger = new BlogLogger(funcConfig.StorageConnectionString);

          string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

          var item = new KeyValuePair<string, StringValues>("x-api-key", new StringValues(funcConfig.FuncApiKey));
          if(!req.Headers.Contains(item))
          {
            return new NotFoundObjectResult(new { Id = -1, error = "Unknown error." });
          }

          if(!_httpClient.DefaultRequestHeaders.Contains("x-api-key"))
          {
            _httpClient.DefaultRequestHeaders.Add("x-api-key", funcConfig.ApiKeyForEmailSenderService);
          }

          var payload = new StringContent(requestBody, Encoding.UTF8, "application/json");
          var response = await _httpClient.PostAsync(funcConfig.EmailSenderServiceUri, payload);
          var result = await response.Content.ReadAsStringAsync();          

          await blobLogger.Log($"requestBody: {requestBody}");
          await blobLogger.Log($"inboundIP: {_utility.GetInboundIp(req)}");
          
          return new OkObjectResult(result);
        }
        catch(Exception exc)
        {
          await blobLogger?.Log($"error: {exc.Message}");          
          return new NoContentResult();
        }

      }

    }


}
