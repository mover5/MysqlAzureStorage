using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoverSoft.Common.Extensions;
using MySqlAzureStorageImporter.Entities;

namespace MySqlAzureStorageImporter.Parsers
{
    public class MySQLFileParser
    {
        private IEnumerable<string> FileLines { get; set; }

        public MySQLFileParser(string filePath)
        {
            this.FileLines = File.ReadLines(filePath);
        }

        public Dictionary<string, MySQLTableInfo> ParseAndGetTableInfo()
        {
            var response = new Dictionary<string, MySQLTableInfo>();

            // Loop through each line and find the "CREATE TABLE `X` (" line
            // That is the table name. Keep going and find ") ENGINE". this is the end. Everything in the middle is a field
            string currentTableName = null;
            foreach (var sqlLine in this.FileLines)
            {
                var trimmedLine = sqlLine.Trim(' ');

                if (currentTableName == null)
                {
                    if (trimmedLine.StartsWithInsensitively("create table `"))
                    {
                        // Use a simple split to find the table name
                        currentTableName = trimmedLine.Split('`')[1].ToLowerInvariant();
                        response[currentTableName] = new MySQLTableInfo
                        {
                            Name = currentTableName,
                            Fields = new List<MySQLFieldInfo>(),
                            RowKeys = new List<RowKey>()
                        };

                        Console.WriteLine("Found table {0}", currentTableName);
                    }
                }
                else
                {
                    // If we get here, the sqlLine is either a field or the end line.
                    if (trimmedLine.StartsWithInsensitively(") engine"))
                    {
                        // This is the end
                        currentTableName = null;
                        Console.WriteLine();
                    }
                    else if (trimmedLine.StartsWithInsensitively("primary key"))
                    {
                        // This line defines a key
                        // PRIMARY KEY (`EnrollmentID`),
                        response[currentTableName].RowKeys.Add(new RowKey
                        {
                            Identifier = "PK",
                            FieldNames = trimmedLine.Split('(')[1].Split(')')[0].Replace("`", string.Empty).Split(',')
                        });

                        Console.WriteLine("PK Key with fields {0}", trimmedLine.Split('(')[1].Split(')')[0].Replace("`", string.Empty));
                    }
                    else if (trimmedLine.StartsWithInsensitively("key"))
                    {
                        // This line defines a key
                        // KEY `Enroll_ind1` (`BandID`,`Year`)
                        response[currentTableName].RowKeys.Add(new RowKey
                        {
                            Identifier = trimmedLine.Split('`')[1].ToUpperInvariant(),
                            FieldNames = trimmedLine.Split('(')[1].Split(')')[0].Replace("`", string.Empty).Split(',')
                        });

                        Console.WriteLine("{0} Key with fields {1}", trimmedLine.Split('`')[1].ToUpperInvariant(), trimmedLine.Split('(')[1].Split(')')[0].Replace("`", string.Empty));
                    }
                    else
                    {
                        var lineSplit = trimmedLine.Split(' ');
                        var fieldName = lineSplit[0].Split('`')[1];
                        var fieldType = lineSplit[1];

                        response[currentTableName].Fields.Add(new MySQLFieldInfo
                        {
                            FieldName = fieldName,
                            FieldType = this.ParseMySQLType(fieldType, !trimmedLine.ContainsInsensitively("not null"))
                        });

                        Console.WriteLine("Field named {0} with type {1}", fieldName, fieldType);
                    }
                }
            }

            return response;
        }

        private Type ParseMySQLType(string mysqlType, bool nullable)
        {
            if (mysqlType.StartsWithInsensitively("int") ||
                mysqlType.StartsWithInsensitively("year"))
            {
                if (nullable)
                {
                    return typeof(int?);
                }

                return typeof(int);
            }
            else if (mysqlType.StartsWithInsensitively("varchar") ||
                     mysqlType.StartsWithInsensitively("longtext") ||
                     mysqlType.StartsWithInsensitively("mediumtext"))
            {
                return typeof(string);
            }
            else if (mysqlType.StartsWithInsensitively("date") ||
                    mysqlType.StartsWithInsensitively("time"))
            {
                if (nullable)
                {
                    return typeof(DateTime?);
                }

                return typeof(DateTime);
            }
            else if (mysqlType.StartsWithInsensitively("decimal"))
            {
                if (nullable)
                {
                    return typeof(decimal?);
                }

                return typeof(decimal);
            }

            Console.WriteLine("Unknown mysql type {0}", mysqlType);
            throw new ArgumentException("mysqlType not found: " + mysqlType);
        }
    }
}
