using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoverSoft.StorageLibrary.Entities;
using MySqlAzureStorageImporter.Parsers;
using PowerArgs;

namespace MySqlAzureStorageImporter
{
    [ArgExceptionBehavior(ArgExceptionPolicy.StandardExceptionHandling)]
    public class MainProgram
    {
        [ArgRequired]
        [ArgExistingFile]
        public string MySQLDumpFile { get; set; }

        public void Main()
        {
            Console.WriteLine("Parsing...");

            var sqlParser = new MySQLFileParser(this.MySQLDumpFile);
            sqlParser.ParseAndGetTableInfo();


            Console.ReadKey();
        }
    }
}
