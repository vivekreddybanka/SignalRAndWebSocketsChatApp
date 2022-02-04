   
using System.Net.WebSockets;
using System.Threading;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
// add the websockets
app.UseWebSockets();
app.Use(async (context, next) => {
    
});

app.MapGet("/", () => "Hello World!");

app.Run();
