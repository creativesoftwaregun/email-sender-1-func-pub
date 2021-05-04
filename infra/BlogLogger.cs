using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;

using Microsoft.Azure.Functions.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(acme.Function.Startup))]

namespace acme.Function.infra
{
    public class BlogLogger
    {
      private readonly CloudBlobClient _client;

      public BlogLogger(string storageConnectionString)
      {
          _client = GetCloudBlobClient(storageConnectionString);
      }

      CloudBlobClient GetCloudBlobClient(string connectionString)
      {
        var storageAccount = CloudStorageAccount.Parse(connectionString);
        return storageAccount.CreateCloudBlobClient();
      }

      public async Task Log(string msg)
      {
        

        string containerName = "email-sender-1-log";
        string logFileName = "incoming.log";

        var container = _client.GetContainerReference(containerName.ToString());
        var blob = container.GetAppendBlobReference(logFileName);

        bool isPresent = await blob.ExistsAsync();

        if (!isPresent)
        {
            await blob.CreateOrReplaceAsync();
        }

        await blob.AppendTextAsync($"{DateTime.UtcNow.ToString("MMM dd yyyy - hh:mm:ss UTC")} -- {msg} \n");
      }
    }


}
