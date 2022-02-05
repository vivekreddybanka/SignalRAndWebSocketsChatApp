using Microsoft.AspNetCore.Builder;

namespace webSocketServer.Middleware {
    public static class WebSocketServerMiddlewareExtension {
        public static IApplicationBuilder UserWebSocketServer(this IApplicationBuilder buider) {
            return buider.UseMiddleware<WebSocketServerMiddleware>();
        }
    }
}