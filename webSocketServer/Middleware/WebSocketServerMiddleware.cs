using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Newtonsoft.Json;

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

                    // call the message send method to send to the client socket

                    await SendConnIDAsync(websocket, ConniD);

                    await ReceiveMessage(websocket,async (result, buffer) => {

                        if (result.MessageType == WebSocketMessageType.Text) {
                            Console.WriteLine("Message received ");
                            Console.WriteLine($"Message: { Encoding.UTF8.GetString(buffer,0, result.Count)}");
                            await RouteJsonMessageAsync(Encoding.UTF8.GetString(buffer,0, result.Count));
                            return;
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
        private async Task SendConnIDAsync(WebSocket socket, string ConnID) {
             
             var buffer = Encoding.UTF8.GetBytes("ConnID: " + ConnID);
             await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        private async Task ReceiveMessage(WebSocket socket, Action<WebSocketReceiveResult, byte [] > handleMessage) {
            var buffer = new byte[1024 * 4];
            while(socket.State == WebSocketState.Open) {
                var result = await socket.ReceiveAsync(buffer: new ArraySegment<byte>(buffer),
                cancellationToken: CancellationToken.None);
                handleMessage(result,buffer);
            }
        }

        public async Task RouteJsonMessageAsync(string Message){

            var RouteObject = JsonConvert.DeserializeObject<dynamic>(Message);
            if (Guid.TryParse(RouteObject.To.ToString(), out Guid guidpoutput))
            {
                Console.WriteLine("Targeted");
                var sock = _manager.GetAllSockets().FirstOrDefault( s=> s.Key == RouteObject.To.ToString());
                if (sock.Value != null) {
                    if (sock.Value.State == WebSocketState.Open) {
                        await sock.Value.SendAsync(Encoding.UTF8.GetBytes(RouteObject.Message.ToString()), WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                } else{
                    Console.WriteLine("Invalid Client Websocket ID");
                }
            }
            else {
                Console.WriteLine("BroadCast Message");
                foreach(var soc in _manager.GetAllSockets()) {
                    if (soc.Value.State == WebSocketState.Open) {
                        await soc.Value.SendAsync(Encoding.UTF8.GetBytes(RouteObject.Message.ToString()), WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                }
            }
        }
    }
}