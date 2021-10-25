
using System;
using System.Data;
using System.IO;

using CsvHelper.Configuration;
using System.Globalization;
using CsvHelper;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;

namespace csvreader1
{ 
    
    public class mycsv
    {
        System.Data.DataTable dataTable = new System.Data.DataTable();

        //Connection String
        //copy datatable to sql
        string ConnectionString = null;
        SqlConnection cnn;

        
        public void readfile()
        {
            
            string filePath = "/exhibitA1.csv";

            System.Data.DataTable readdataTable = new System.Data.DataTable();
            // Check if file exists.
            if (File.Exists(filePath))
            {

                CsvConfiguration csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
                { 
                    HasHeaderRecord = true,
                    Delimiter = ","
                    
                };

                // Read column headers from file
                CsvReader csv = new CsvReader(File.OpenText(filePath), csvConfiguration);
                csv.Read();
                csv.ReadHeader();

                List<string> headers = csv.HeaderRecord.ToList();

                List<int> integers = new List<int> { 1, 2, 3 };
                integers.ForEach(Console.WriteLine);
                headers.ForEach(Console.WriteLine);
                Console.ReadLine();

                // Read csv into datatable
                //System.Data.DataTable dataTable = new System.Data.DataTable();

                foreach (string header in headers)
                {
                    readdataTable.Columns.Add(new System.Data.DataColumn(header));
                  
                   
                }
                Console.WriteLine(readdataTable.Columns[0]);
               

                // Check all required columns are present
                if (!headers.Exists(x => x == "PLAY_ID"))
                {
                    throw new ArgumentException("PLAY_ID field not present in input file.");
                }
                else if (!headers.Exists(x => x == "SONG_ID"))
                {
                    throw new ArgumentException("SONG_ID field not present in input file.");
                }
                else if (!headers.Exists(x => x == "CLIENT_ID"))
                {
                    throw new ArgumentException("CLIENT_ID field not present in input file.");
                }
                else if (!headers.Exists(x => x == "PLAY_TS"))
                {
                    throw new ArgumentException("PLAY_TS field not present in input file.");
                }
                // Import csv
                while (csv.Read())
                {
                    System.Data.DataRow row = readdataTable.NewRow();

                    foreach (System.Data.DataColumn column in readdataTable.Columns)
                    {
                        row[column.ColumnName] = csv.GetField(column.DataType, column.ColumnName);
                        //Console.WriteLine(row[column.ColumnName]);
                        //Console.ReadLine();
                      }
                    
                    readdataTable.Rows.Add(row);
                }
                

               // Make sure all required columns are populated
                if (readdataTable.Select("PLAY_ID = '' OR PLAY_ID = null").Count() > 0)
                {
                    throw new ArgumentException("Output contains null ua_channel values.");
                }

                if (readdataTable.Select("SONG_ID = '' OR SONG_ID = null").Count() > 0)
                {
                    throw new ArgumentException("Output contains null ua_device_type values.");
                }

                if (readdataTable.Select("CLIENT_ID = '' OR CLIENT_ID = null ").Count() > 0)
                {
                    throw new ArgumentException("Output contains null or invalid message_type values.");
                }
                if (readdataTable.Select("PLAY_TS = '' OR PLAY_TS = null ").Count() > 0)
                {
                    throw new ArgumentException("Output contains null or invalid message_type values.");
                }
            }

            //add connection string
            ConnectionString = @"Data Source=xxx;Initial Catalog=xxx;User ID=xxx;Password=xxx";
            cnn = new SqlConnection(ConnectionString);

           // cnn.Open();

            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(cnn))
            {
                bulkCopy.DestinationTableName = "dbo.ReadTable";

                bulkCopy.ColumnMappings.Add("PLAY_ID", "play_id");
                bulkCopy.ColumnMappings.Add("SONG_ID", "song_id");
                bulkCopy.ColumnMappings.Add("CLIENT_ID", "client_id");
                bulkCopy.ColumnMappings.Add("PLAY_TS", "play_ts");
                try
                {
                    cnn.Open();
                    // Write from the source to the destination.
                    bulkCopy.WriteToServer(readdataTable);
                }
                catch (Exception ex)
                {
                    cnn.Open();
                }
            }
            cnn.Close();

        }

        //Read data from sql
        public void ReadData()
        {
                   
            //copy datatable to sql
            ConnectionString = @"Data Source=xxx;Initial Catalog=xxx;User ID=xxx;Password=xxx";
            cnn = new SqlConnection(ConnectionString);

            var selectQuery = "WITH t AS(select count(DISTINCT song_id) as DISTINCT_PLAY_COUNT, count(DISTINCT client_id) as DISTINCT_CLIENT_COUNT from dbo.ReadTable  where play_ts = '2016-10-08 00:00:00.000' GROUP BY client_id )SELECT DISTINCT * FROM t; ";
            try
            {
                cnn.Open();
                var command = new SqlCommand(selectQuery, cnn);

                using (var adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(dataTable);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                throw ex;
            }
            finally
            {
                if (cnn.State == ConnectionState.Open)
                    cnn.Close();
            }
            foreach (DataRow dataRow in dataTable.Rows)
            {
                foreach (var item in dataRow.ItemArray)
                {
                    Console.WriteLine(item);
                    Console.ReadLine();
                }
            }
            WriteToFile(dataTable, false, ",");


        }

        //Write to csv file
        public static void WriteToFile(System.Data.DataTable dataSource, bool firstRowIsColumnHeader = false, string seperator = ";")
        {

            string fileOutputPath = @"C:\\OutputDataFile.csv";
            var sw = new StreamWriter(fileOutputPath, false);

            int icolcount = dataSource.Columns.Count;

            if (!firstRowIsColumnHeader)
            {
                for (int i = 0; i < icolcount; i++)
                {
                    sw.Write(dataSource.Columns[i]);
                    if (i < icolcount - 1)
                        sw.Write(seperator);
                }

                sw.Write(sw.NewLine);
            }

            foreach (DataRow drow in dataSource.Rows)
            {
                for (int i = 0; i < icolcount; i++)
                {
                    if (!Convert.IsDBNull(drow[i]))
                        sw.Write(drow[i].ToString());
                    if (i < icolcount - 1)
                        sw.Write(seperator);
                }
                sw.Write(sw.NewLine);
            }
            sw.Close();
        }
                              
        public static void Main()
        {
            mycsv newfile = new mycsv();
           //newfile.readfile();
            newfile.ReadData();
        }

    }
    
    
    }