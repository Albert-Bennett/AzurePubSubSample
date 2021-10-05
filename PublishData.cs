using Azure.Messaging.WebPubSub;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.WebPubSub;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace test_pubsub
{
    public class PublishData
    {
        [FunctionName("PublishData")]
        public async Task Run([TimerTrigger("*/5 * * * * *")] TimerInfo myTimer, ILogger log,
            [WebPubSub(Hub = "test")] IAsyncCollector<WebPubSubOperation> operations)
        {
            await operations.AddAsync(new SendToAll
            {
                Message = BinaryData.FromString(DateTime.Now.ToLongTimeString()),
                DataType = MessageDataType.Text
            });
        }
    }
}
