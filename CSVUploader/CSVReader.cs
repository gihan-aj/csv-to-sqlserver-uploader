using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CSVUploader
{
    public class CSVReader
    {  
        private readonly string _filePath;

        public CSVReader( string filePath )
        {
            _filePath = filePath; 
        }

        public CSVData GetCSVData()
        {
            if (string.IsNullOrEmpty(_filePath))
            {
                throw new ArgumentNullException("File path is required");
            }

            if(!File.Exists(_filePath))
            {
                throw new FileNotFoundException();
            }

            var csvData = new CSVData();
            
            try
            {
                using (var reader = new StreamReader(_filePath))
                {
                    // Read headers 
                    var headers = reader.ReadLine().Split(',').ToList();

                    // Rest of the data
                    List<List<string>> data = new List<List<string>>();
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        data.Add(line.Split(',').ToList());
                    }

                    csvData.Headers = headers;
                    csvData.Data = data;

                }

                return csvData;
            }
            catch
            {
                throw;
            }
            
        }

       
    }
}
