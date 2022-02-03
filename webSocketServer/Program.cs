
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
        await next();
    }
});

app.MapGet("/", () => "Hello World!");

app.Run();
