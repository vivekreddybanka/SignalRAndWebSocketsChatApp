   
using System.Net.WebSockets;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
// add the websockets
app.UseWebSockets();
app.Use(async (context, next) => {
    if (context.WebSockets.IsWebSocketRequest) {
        WebSocket websocket =  await context.WebSockets.AcceptWebSocketAsync();
        Console.WriteLine("Websocket Connected");
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