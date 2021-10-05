using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.WebPubSub;
using Microsoft.Extensions.Logging;

namespace test_pubsub
{
    public static class Subscribe
    {
        [FunctionName("Subscribe")]
        public static WebPubSubConnection Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [WebPubSubConnection(Hub ="test")] WebPubSubConnection connection,ILogger log)
        {
            return connection;
        }
    }
}
