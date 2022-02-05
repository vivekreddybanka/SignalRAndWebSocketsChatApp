using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace webSocketServer.Middleware
{
    public class WebSocketServerMiddleware 
    {

        private readonly RequestDelegate _next;
        private readonly WebsocketServerConnectionManager _manager;

        public WebSocketServerMiddleware(RequestDelegate next, WebsocketServerConnectionManager manager) {
            _next = next;
            _manager = manager;
        }
        public async Task InvokeAsync(HttpContext context ) {
            if (context.WebSockets.IsWebSocketRequest) {
                    WebSocket websocket =  await context.WebSockets.AcceptWebSocketAsync();
                    Console.WriteLine("Websocket Connected");
                    // added the new websocket request to the manager
                    string ConniD = _manager.AddSocket(websocket);

                    await ReceiveMessage(websocket,async (result, buffer) => {

                        if (result.MessageType == WebSocketMessageType.Text) {
                            Console.WriteLine("Message received ");
                            Console.WriteLine($"Message: { Encoding.UTF8.GetString(buffer,0, result.Count)}");
                        }
                        else if (result.MessageType == WebSocketMessageType.Close) {
                            Console.WriteLine("received Close Message");
                            return;
                        }
                    });
                } 
                else {
                    Console.WriteLine("Hello from the 2nd request");
                    await _next(context);
                }
        }

        private async Task ReceiveMessage(WebSocket socket, Action<WebSocketReceiveResult, byte [] > handleMessage) {
            var buffer = new byte[1024 * 4];
            while(socket.State == WebSocketState.Open) {
                var result = await socket.ReceiveAsync(buffer: new ArraySegment<byte>(buffer),
                cancellationToken: CancellationToken.None);
                handleMessage(result,buffer);
            }
        }
    }
}