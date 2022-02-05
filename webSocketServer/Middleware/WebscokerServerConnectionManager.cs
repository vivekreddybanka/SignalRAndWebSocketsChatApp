
using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace webSocketServer.Middleware
{
    public class WebsocketServerConnectionManager {

        private ConcurrentDictionary<string,WebSocket> _sockets = new ConcurrentDictionary<string, WebSocket>();

        public ConcurrentDictionary<string,WebSocket> GetAllSockets() {
            return _sockets;
        }

        public string AddSocket(WebSocket socket) {
            string ConniD = Guid.NewGuid().ToString();
            _sockets.TryAdd(ConniD,socket);
            Console.WriteLine("Connection added" + ConniD);
            return ConniD;
        }

    }
}