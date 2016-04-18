using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySqlAzureStorageImporter.Entities
{
    public class RowKey
    {
        public string Identifier { get; set; }
        public string[] FieldNames { get; set; }
    }
}
