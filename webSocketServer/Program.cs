   
using System.Net.WebSockets;
using System.Threading;
using webSocketServer.Middleware;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
// add the websockets
app.UseWebSockets();
app.UserWebSocketServer();
// app.Use(async (context, next) => {
    
// });

app.MapGet("/", () => "Hello World!");

app.Run();
