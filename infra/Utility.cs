
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

[assembly: FunctionsStartup(typeof(acme.Function.Startup))]

namespace acme.Function.infra
{
  public class Utility 
  {

    public string GetInboundIp(HttpRequest  request)
    {
      if(request.Headers.ContainsKey("X-Forwarded-For"))
      {
        return (request.Headers["X-Forwarded-For"].FirstOrDefault() ?? "").Split(new char[] { ':' }).FirstOrDefault();
      }
      return string.Empty;
    }
  }

}
