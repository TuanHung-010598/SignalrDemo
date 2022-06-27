using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SignalrChat
{
    public class ChatHub : Hub<IClient>
    {
        public async Task Send(string name, string message)
        {
            //await Clients.All.broadcastMessage(name, message);
            var connection = _connections.Find(c => c.ConnectionId == Context.ConnectionId);
            if (connection.GroupJoined == "GroupA")
            {
                await SendToGroupA(message);
            }
            else if(connection.GroupJoined == "GroupB")
            {
                await SendToGroupB(message);
            }
            else
            {
                await Clients.All.BroadcastMessage(name, message);
            }
        }

        public async Task SendToGroupA(string message)
        {
            await Clients.Group("GroupA").GroupAMessage(message);
        }
        public async Task SendToGroupB(string message)
        {
            await Clients.Group("GroupB").GroupBMessage(message);
        }

        public async Task ChangeGroup(string newGroup)
        {
            Connection connection = _connections.Find(c => c.ConnectionId == Context.ConnectionId);

            await Groups.Remove(Context.ConnectionId, connection.GroupJoined);
            await Clients.All.BroadcastMessage(connection.Name,$" leaved group {connection.GroupJoined}");

            _connections[_connections.IndexOf(connection)].GroupJoined = newGroup;
            await Groups.Add(Context.ConnectionId, newGroup);
            await Clients.All.BroadcastMessage(connection.Name, $" joined group {connection.GroupJoined}");
        }

        private static List<Connection> _connections = new List<Connection>();
        
        public override Task OnConnected()
        {
            var name = Context.QueryString["name"];
            var groupJoin = Context.QueryString["group"];
            _connections.Add(new Connection()
            {
                Name = name,
                ConnectionId = Context.ConnectionId,
                GroupJoined = groupJoin
            });
            Clients.All.BroadcastMessage(name, $" join the chat");
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            // detect reason of disconnect
            if(stopCalled)
            {
                Console.WriteLine($"Client {Context.ConnectionId} has disconneced");
            }
            else
            {
                Console.WriteLine($"Client {Context.ConnectionId} time out");
            }
            var connection = _connections.FirstOrDefault(c => c.ConnectionId == Context.ConnectionId);
            _connections.RemoveAt(_connections.IndexOf(connection));
            Clients.All.BroadcastMessage(connection.Name, $" leave the chat");
            return base.OnDisconnected(stopCalled);
        }
    }

    public class Connection
    {
        public string Name { get; set; }
        public string ConnectionId { get; set; }
        public string GroupJoined { get; set; }
    }

    public interface IClient
    {
        Task BroadcastMessage(string name,string message);
        Task GroupAMessage(string message);
        Task GroupBMessage(string message);
    }
}