using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArgs;

namespace MySqlAzureStorageImporter
{
    class Program
    {
        static void Main(string[] args)
        {
            Args.InvokeMain<MainProgram>(args);
        }
    }
}
