using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySqlAzureStorageImporter.Entities
{
    public class MySQLFieldInfo
    {
        public string FieldName { get; set; }

        public Type FieldType { get; set; }
    }
}
