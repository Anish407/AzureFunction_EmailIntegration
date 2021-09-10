using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;
using Azure.Storage.Blobs;
using Azure.Identity;
using Azure;
using Microsoft.Azure.Storage.Blob;

namespace ProcessAttachements
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Rootobject data = JsonConvert.DeserializeObject<Rootobject>(requestBody);
            try
            {
                var uri = new Uri("https://processstrg.blob.core.windows.net/attachment");
                // Get a credential and create a client object for the blob container.
                BlobContainerClient containerClient = new BlobContainerClient(uri,
                                                                                new DefaultAzureCredential());

                // Upload Email Content
                await UploadBlobAsync(data.body, containerClient, data.subject);

                //Upload all attachments
                foreach (var item in data.attachments)
                {
                    string data1 = Encoding.UTF8.GetString(item.ContentBytes);
                    await UploadBlobAsync(string.Empty, containerClient, item.Name,item.ContentBytes);
                }
            }
            catch (RequestFailedException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
            catch (Exception e)
            {

            }




            return new OkObjectResult(requestBody);

        

            static async Task UploadBlobAsync(string blobContent, BlobContainerClient containerClient, string blobName, byte[] data=null)
            {
                BlobClient blobClient = containerClient.GetBlobClient(blobName);
                var response = blobClient.Exists();
                if (!response)
                {

                    var content = data ==null ?  Encoding.UTF8.GetBytes(blobContent): data;
                    var ms = new MemoryStream();
                    ms.Position = 0;
                    ms.Write(content);
                    await blobClient.UploadAsync(new MemoryStream(content));
                }
             
            }
        }
    }
}
