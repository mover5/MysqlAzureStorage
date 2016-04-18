using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySqlAzureStorageImporter.Entities
{
    public class MySQLTableInfo
    {
        public List<MySQLFieldInfo> Fields { get; set; }

        public string Name { get; set; }

        public List<RowKey> RowKeys { get; set; }
    }
}
