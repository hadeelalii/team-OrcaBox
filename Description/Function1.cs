using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.SemanticKernel;
using System.Threading;

namespace FunctionApp6
{
    public static class Function1
    {
        public class ResultObject
        {
            public string description { get; set; }
        }

        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            // The object it's requesting
            Console.WriteLine(requestBody);

            // Initialize semantic kernel
            var builder = new KernelBuilder();
            builder.WithAzureTextCompletionService(
                "text-davinci-003",
                "<url>",
                "<key>");

            var kernel = builder.Build();

            string prompt = "You are an AI tasked with generating an image ALT text from the following details: {{$data}}. The ALT text should clearly describe what is happening in the image. The ALT text should be short and only be 1 sentence long. The alt text should include the image type (photo, drawing, etc) ";
            var myPromptTemplate = kernel.CreateSemanticFunction(prompt, maxTokens: 500, temperature: 0, topP: 0.5);
            var context = kernel.CreateNewContext();
            context["data"] = requestBody;
            var result = await myPromptTemplate.InvokeAsync(context);
            Console.WriteLine(result.ToString());

            // Remove newline characters
            string cleanedResult = result.ToString().Replace("\n", "");

            var responseObject = new ResultObject { description = cleanedResult };
            var json = JsonConvert.SerializeObject(responseObject);

            return new ContentResult
            {
                Content = json,
                ContentType = "application/json",
                StatusCode = 200
            };
        }
    }
}
