using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace techsearch
{
    public static class TipsSearch
    {
        [FunctionName("TipsSearch")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["q"];

			var searcher = new Searcher("sairamasearch", "tips");
			dynamic data = await searcher.SearchAsync($"{name}*");

            return name != null
                ? (ActionResult)new OkObjectResult(data)
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }
    }
}
