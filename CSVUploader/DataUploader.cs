using LoggerLib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSVUploader
{
    public class DataUploader
    {
        private readonly string _connectinString;
        private readonly string _dbTable;
        private readonly CSVData _data;
        private readonly IFileLogger _logger;

        public DataUploader(string connectinString, string dbTable, CSVData data, IFileLogger logger)
        {
            _connectinString = connectinString;
            _dbTable = dbTable;
            _data = data;
            _logger = logger;
        }

        public void UploadToSql()
        {
            using (var conn = new SqlConnection(_connectinString))
            {
                conn.Open();

                // Check if table exists and create if not
                if(!TableExists(conn, _dbTable))
                {
                    CreateTable(conn, _data.Headers, _dbTable);
                    _logger.LogInfo("DB Table created successfully");
                }

                // Get table schema
                var schema = GetTableSchema(conn, _dbTable);

                // Sql bulk copy
                using (var bulkCopy = new SqlBulkCopy(conn))
                {
                    bulkCopy.DestinationTableName = _dbTable;

                    // Map data to table columns
                    //foreach (var column in schema)
                    //{
                    //    bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping(column.Namespace, column.Namespace));
                    //}

                    var dataTable = new DataTable();
                    dataTable.Columns.AddRange(schema.Select(c => new DataColumn(c.Namespace, c.DataType)).ToArray());
                    
                    foreach(var row in _data.Data)
                    {
                        dataTable.Rows.Add(row.ToArray());
                    }

                    bulkCopy.WriteToServer(dataTable);
                    _logger.LogInfo("DB Table bulk copy successfull");
                }
            }
        }
        
        private bool TableExists(SqlConnection connection, string tableName)
        {
            var sql = $"SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{tableName}'";
            using(var command = new SqlCommand(sql, connection))
            {
                // check the first column of the first raw
                return command.ExecuteScalar() != null;
            }
        }

        private void CreateTable(SqlConnection connection, List<string> headers, string tableName)
        {
            // Build statement dynamically
            var sql = $"CREATE TABLE {tableName} (";

            for(int i = 0; i < headers.Count; i++)
            {
                if(i == 0)
                {
                    sql += $"[{headers[i]}] datetime PRIMARY KEY,";
                }
                else
                {
                    sql += $"[{headers[i]}] float(53),";
                }
            }

            sql = sql.TrimEnd(',') + ")";

            Console.WriteLine(sql);

            // Execute
            using(var command = new SqlCommand(sql,connection))
            {
                command.ExecuteNonQuery();
            }
        }

        private List<DataColumn> GetTableSchema(SqlConnection connection, string tableName)
        {
            var schema = new List<DataColumn>();
            var sql = $"SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{tableName}'";
            using(var command = new SqlCommand(sql, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        //Console.WriteLine(reader.GetFieldType(7));
                        schema.Add(new DataColumn(reader.GetString(3), reader.GetFieldType(7)));
                    }
                }
            }
            return schema;
        }
       
    }
}
