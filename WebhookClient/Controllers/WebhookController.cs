using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebhookClient.Models;

namespace WebhookClient.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebhookController : ControllerBase
    {
        // This is the customer's API key which has been hard-coded here for simplicity.
        private readonly string secret = "f5a9a34e3b4bbfd6403d49c3b8aa5026041646f0208e2eadc27f6cad5421c3b3";

        [HttpPost]
        public IActionResult Post([FromBody] WebhookPayload webhookPayload)
        {
            // E.g. {"SubmissionId":"1e6a29ba-5e2a-46ea-a25c-87f16dab756a","NextAction":"Send","CreationDate":"2021-07-15T14:47:58.6815487+01:00","Attempt":1}
            string serializedPayload = JsonConvert.SerializeObject(webhookPayload);
            Console.WriteLine($"Payload: {serializedPayload}");
            Console.WriteLine($"Submission ID: {webhookPayload.SubmissionId.ToString()}");
            Console.WriteLine($"NextAction: {webhookPayload.NextAction}");
            Console.WriteLine($"Attempt: {webhookPayload.Attempt}");
            Console.WriteLine($"CreationDate: {webhookPayload.CreationDate}");

            bool signatureMatch = CheckPayloadSignature(serializedPayload);
            Console.WriteLine($"Signature match: {signatureMatch}");

            // This is where the customer's application would do it's thing.
            // DoStuff();

            if (signatureMatch)
                return Ok();
            else
                return BadRequest();
        }

        private bool CheckPayloadSignature(string payload)
        {
            if (!HttpContext.Request.Headers.ContainsKey("PaygateBacsApiWebhookSignature"))
                return false;

            // E.g. SignatureHeaderKey=31-DF-96-6E-62-94-EF-FB-EA-77-1D-96-4E-0A-F6-AE-80-81-0B-4F-64-4A-7E-42-28-0B-25-1A-9A-A6-5F-CB
            string[] receivedSignature = HttpContext.Request.Headers["PaygateBacsApiWebhookSignature"].ToString().Split("=");
            if (receivedSignature.Length < 2 || string.Compare(receivedSignature[0], "SignatureHeaderKey", false) != 0)
                return false;

            Console.WriteLine($"Received signature key: {receivedSignature[0]}");
            Console.WriteLine($"Received signature value: {receivedSignature[1]}");

            byte[] secretBytes = Encoding.UTF8.GetBytes(secret);
            string computedSignature;
            using (HMACSHA256 hasher = new HMACSHA256(secretBytes))
            {
                // Payload example {"SubmissionId":"1e6a29ba-5e2a-46ea-a25c-87f16dab756a","NextAction":"Send","CreationDate":"2021-07-15T14:47:58.6815487+01:00","Attempt":1}
                byte[] data = Encoding.UTF8.GetBytes(payload);

                // E.g. 31-DF-96-6E-62-94-EF-FB-EA-77-1D-96-4E-0A-F6-AE-80-81-0B-4F-64-4A-7E-42-28-0B-25-1A-9A-A6-5F-CB
                computedSignature = BitConverter.ToString(hasher.ComputeHash(data));
                Console.WriteLine($"Computed signature: {computedSignature}");
            }

            return computedSignature == receivedSignature[1];
        }
    }
}