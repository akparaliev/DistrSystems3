using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistrSystems3
{

    public interface ICallsToServer
    {
        void Connect(string id);
        void Disconnect(string id);
        List<string> GetTasks(string id);
        void AddTask(IEnumerable<string> ids);
        void BalanceLoad();
    }
}
