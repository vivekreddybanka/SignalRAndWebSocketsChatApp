using System;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SignalRserver
{
    public class ChatHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            Console.WriteLine("Connected Established " + Context.ConnectionId);
            Clients.Client(Context.ConnectionId).SendAsync("ReceiveClientID", Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public async Task SendMessageAsync(string Message)
        {
            var routeob = JsonConvert.DeserializeObject<dynamic>(Message);
            string toClient = routeob.To.ToString();
            Console.WriteLine("Message Recived On: " + Context.ConnectionId);

            if (routeob.To.ToString() == string.Empty)
            {
                Console.WriteLine("Broadcast");
                await Clients.All.SendAsync("ReceiveMessage", Message);
            }
            else
            {
                Console.WriteLine("Targeted on: " + toClient);
                await Clients.Client(toClient).SendAsync("ReceiveMessage", Message);

            }
        }

    }

}