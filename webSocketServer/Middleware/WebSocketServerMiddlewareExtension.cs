using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace webSocketServer.Middleware {
    public static class WebSocketServerMiddlewareExtension {
        public static IApplicationBuilder UserWebSocketServer(this IApplicationBuilder buider) {
            return buider.UseMiddleware<WebSocketServerMiddleware>();
        }

        public static IServiceCollection AddWebSocketManager(this IServiceCollection services) {
            services.AddSingleton<WebsocketServerConnectionManager>();
            return services;
        }
    }
}