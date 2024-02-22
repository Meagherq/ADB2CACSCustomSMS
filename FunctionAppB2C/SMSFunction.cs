using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Mail;
using Azure.Communication.Sms;

namespace FunctionAppIUB2C
{
    public static class SMSFunction
    {
        [FunctionName("SMSFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            SmsClient smsClient = new SmsClient("");

            string To = data.To ?? data.mfaPhoneNumber;
            string Message = data.Body;
            SmsSendResult sendResult = smsClient.Send(
                from: "+18333260917", // Your E.164 formatted from phone number used to send SMS
                to: "+1" + To, // E.164 formatted recipient phone number
                message: Message);
            Console.WriteLine($"Message id {sendResult.MessageId}");
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }
    }
}
