using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistrSystems3
{
    class ClientComms : MarshalByRefObject, ICallsToServer
    {
        private readonly Dictionary<string,List<string>> _clients;
        private readonly List<string> _freeTasks;
        public ClientComms()
        {
            _clients = new Dictionary<string, List<string>>();
            _freeTasks = new List<string>();
        }

      

        public void Connect(string id)
        {
            lock (_clients)
            {
                if (!_clients.ContainsKey(id))
                {
                    _clients.Add(id,new List<string>());
                }
            }
        }

     

        public void Disconnect(string id)
        {
            lock (_clients)
            {
                if (_clients.ContainsKey(id))
                {
                    var list = new string[_clients.Count];
                    _clients[id].CopyTo(list);
                    _clients.Remove(id);
                    AddTask(list);
                
                }
            }
        }

        public List<string> GetTasks(string id)
        {
            lock (_clients)
            {
                if (_clients.ContainsKey(id))
                {
                    return _clients[id];
                }
                else return new List<string>();
            }
        }

        public void AddTask(IEnumerable<string> ids)
        {
            lock (_freeTasks)
            {
                _freeTasks.AddRange(ids);
            }
            DistributeTasks();
        }
        private void DistributeTasks()
        {
            lock (_freeTasks)
            {
                while (_freeTasks.Any())
                {
                    lock (_clients)
                    {
                        var minTaskCountClients = from x in _clients where x.Value.Count == _clients.Min(v => v.Value.Count) select x.Key;
                        if (minTaskCountClients.Any())
                        {
                            _clients[minTaskCountClients.First()].Add(_freeTasks.First());
                            _freeTasks.Remove(_freeTasks.First());
                        }
                    }
                }
            }
        }
        public void BalanceLoad()
        {
            
            lock (_clients)
            {
                foreach(var c in _clients)
                {
                    lock (_freeTasks)
                    {
                        _freeTasks.AddRange(c.Value);
                        c.Value.RemoveRange(0, c.Value.Count);
                    }
                }
            }
            DistributeTasks();
        }
    }
}
