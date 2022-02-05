using System;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SignalRserver
{
    public class ChatHub: Hub {
        public override Task OnConnectedAsync() 
        {
            Console.WriteLine("Connected Established " +  Context.ConnectionID);
        }
    }

}