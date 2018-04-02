using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace DistrSystems3
{
    public class Client
    {
        private bool _stop;
        private List<string> tasks;
        private ICallsToServer GetServerInstance()
        {
            tasks = new List<string>();
            Console.WriteLine("type host");
            string host = Console.ReadLine();

            TcpChannel m_TcpChan = new TcpChannel(0);
            ChannelServices.RegisterChannel(m_TcpChan, false);

            // Create the object for calling into the server
            ICallsToServer m_RemoteObject = (ICallsToServer)
                Activator.GetObject(typeof(ICallsToServer),
                $"tcp://{host}:123/RemoteServer");
            return m_RemoteObject;
        }
        public void RunMain()
        {
            var m_RemoteObject = GetServerInstance();
            var guid = Guid.NewGuid().ToString();
            var t = Task.Run(() => GetTasks(guid, m_RemoteObject));
            Console.WriteLine($"your id: {guid}");
            Console.WriteLine(" type 'c' for connect");
            Console.WriteLine(" type 'd' for disconnect");

            Console.WriteLine(" type 'a' for add task");
            Console.WriteLine(" type 'v' for balance the load");


            while (!_stop)
            {
                var result = Console.ReadLine();
                switch (result[0])
                {
                    case 'c':
                        m_RemoteObject.Connect(guid);
                        break;
                    case 'd':
                        m_RemoteObject.Disconnect(guid);
                        break;
                    case 'a':
                        m_RemoteObject.AddTask(new List<string>() { Guid.NewGuid().ToString() });
                        break;

                    case 'b':
                        m_RemoteObject.BalanceLoad();
                        break;
                    default:
                        break;
                }
            }

        }

        public void Run()
        {
            var m_RemoteObject = GetServerInstance();
            var guid = Guid.NewGuid().ToString();
            var t = Task.Run(() => GetTasks(guid, m_RemoteObject));
            Console.WriteLine($"your id: {guid}");
            Console.WriteLine(" type 'c' for connect");
            Console.WriteLine(" type 'd' for disconnect");
           


            while (!_stop)
            {
                var result = Console.ReadLine();
                switch (result[0])
                {
                    case 'c':
                        m_RemoteObject.Connect(guid);
                        break;
                    case 'd':
                        m_RemoteObject.Disconnect(guid);
                        break;
                    default:
                        break;
                }
            }

        }
        private void GetTasks(string id, ICallsToServer m_RemoteObject)
        {
            while (true)
            {
                Task.Delay(5000);
                var tasksFromServer = m_RemoteObject.GetTasks(id);
                var addedTasks = tasksFromServer.Where(x => !tasks.Any(y=>y==x)).ToList();
                tasks.AddRange(addedTasks);
                OutputList("added tasks", addedTasks);
                var deletedTasks = tasks.Where(x => !tasksFromServer.Any(y=>y==x)).ToList();
                for (var i = 0; i < deletedTasks.Count; i++)
                {
                    tasks.Remove(deletedTasks[i]);
                }
                OutputList("deleted tasks", deletedTasks);
            }
    
        }
        private void OutputList(string message, IEnumerable<string> list)
        {
            if (list.Count() > 0)
            {
                Console.WriteLine(message+$" at {DateTime.Now}");
                foreach(var x in list)
                {
                    Console.WriteLine(x);
                }
                Console.WriteLine("-----------");
            }
            
        }
    }
}
