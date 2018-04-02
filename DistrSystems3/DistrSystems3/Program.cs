using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistrSystems3
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                new Server().Run();
            }

            else
            {
                switch (args[0].ToLower())
                {
                    case "c":
                        new Client().Run();
                        break;
                    case "s":
                        new Server().Run();
                        break;
                    default:
                        new Server().Run();
                        break;
                }
            }
        }
    }
}
