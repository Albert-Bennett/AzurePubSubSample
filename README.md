**About the Sample**
What the sample that I am going to go through is going to do is:
1. Allow the user to subscribe to a hub.
2. Push data to our hub so that subscribers can see it via a http trigger function in the function app.
3. Display the data from our hub.

**Step 1**
Firstly we need to create the Web PubSub Service. There is not much to it... see image below of what settings I used to set it up:
![image](https://dev-to-uploads.s3.amazonaws.com/uploads/articles/7fo1tt20klaupkgjzwib.png) And that's about it for our Azure Services. You don't need to tweak anything on the new service, it's good to go as is well.. for our purpose it is. You can set up a function app with an app service plan as well if you wanted to deploy the code later on but... I didn't want to for this sample.
<br/>
**Step 2**
Next onto the project setup. What we need to do first is to create an C# Function App project with a http trigger:![image](https://dev-to-uploads.s3.amazonaws.com/uploads/articles/e21gitgeyc90usx3bhdv.png) This new HTTP trigger with become out subscribe function. 
From here you need to install the following nuget packages:
1. Azure.Messaging.WebPubSub
2. Microsoft.Azure.WebJobs.Extensions.WebPubSub

These packages are in beta so you'll need to search for the preview packages. What these lets us do is to push messages to our hub and allows users to subscribe to a hub. Finally you'll need to add a connection string from your PubSub service to the local.settings.json file of your function app. You can find the connection string to the Web PubSub service by going to **Keys > Connection String (under the 'Primary')**. See screen shot below:
![pubsub connection string](https://dev-to-uploads.s3.amazonaws.com/uploads/articles/g4a6az01f4rf5m12fi04.PNG) I have left a sample of what my settings file looks like below after adding the connection string:
```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "WebPubSubConnectionString": "[your connection string]"
  }
}
```
<br/>
**Step 3**
The code for this is very straight forward.
We need a function to push data to our hub and a function to let users subscribe to our hub.
Here is the code to get users to subscribe to our hub:
```csharp
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
```
This function essentially establishes a connection to our hub and it returns a json response. Below is a sample response from the endpoint:
```json
{
    "baseUrl": "[base URL]",
    "url": "[base URL with access token]",
    "accessToken": "[the access token]"
}
``` 
The main value we want to make use of is the 'url' field. The value of this is our connection to the hub in the PubSub Service.
And here is the code to push data to our hub.
```csharp
    public class PublishData
    {
        [FunctionName("PublishData")]
        public async Task Run([TimerTrigger("*/10 * * * * *")] TimerInfo myTimer, ILogger log,
            [WebPubSub(Hub = "test")] IAsyncCollector<WebPubSubOperation> operations)
        {
            await operations.AddAsync(new SendToAll
            {
                Message = BinaryData.FromString(DateTime.Now.ToShortTimeString()),
                DataType = MessageDataType.Text
            });
        }
    }
```
Essentially what this is doing is sending the data to all subscribers of the hub defined in the binding of our function in our case 'test'.
<br/>
**Notes**
1. This service is currently in preview so I'd expect some aspects of the code to change over time. I thought that it would be worth pointing it out.
