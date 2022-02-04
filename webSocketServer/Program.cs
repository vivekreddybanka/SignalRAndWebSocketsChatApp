   
using System.Net.WebSockets;
using System.Threading;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
// add the websockets
app.UseWebSockets();
app.Use(async (context, next) => {
    if (context.WebSockets.IsWebSocketRequest) {
        WebSocket websocket =  await context.WebSockets.AcceptWebSocketAsync();
        Console.WriteLine("Websocket Connected");
        await ReceiveMessage(websocket,async (result, buffer) => {

            if (result.MessageType == WebSocketMessageType.Text) {
                Console.WriteLine("Message received ");
            }
            else if (result.MessageType == WebSocketMessageType.Close) {
                Console.WriteLine("received Close Message");
                return;
            }
        });
    } 
    else {
        Console.WriteLine("Hello from the 2nd request");
        WriteRequestParam(context);
        await next();
    }
});

app.MapGet("/", () => "Hello World!");

app.Run();

void WriteRequestParam(HttpContext context) {
    Console.WriteLine("Request Method" + context.Request.Method);
    Console.WriteLine("Request Protocal" + context.Request.Protocol);
    if (context.Request.Headers != null) {
        foreach( var hdr in context.Request.Headers){
            Console.WriteLine("-->" + hdr.Key + " : " +hdr.Value);
        }
    }
}

async Task ReceiveMessage(WebSocket socket, Action<WebSocketReceiveResult, byte [] > handleMessage) {
     var buffer = new byte[1024 * 4];
     while(socket.State == WebSocketState.Open) {
         var result = await socket.ReceiveAsync(buffer: new ArraySegment<byte>(buffer),
         cancellationToken: CancellationToken.None);
         handleMessage(result,buffer);
     }
}