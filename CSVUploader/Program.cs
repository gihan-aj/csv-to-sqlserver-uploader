using CSVUploader;
using Microsoft.Extensions.Configuration;
using LoggerLib;

public class Program    
{
    public static void Main(string[] args)
    {
        string appDirectory = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", ".."));

        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(appDirectory)
            .AddJsonFile("appsetting.json")
            .Build();

        IFileLogger logger = new FileLogger();

        string? connectionString = configuration.GetConnectionString("PowerReadingsDBConnectionString");

        //var filePath = Path.GetFullPath((Path.Combine(appDirectory, "QI01.csv")));
        var filePath = Path.Combine(appDirectory, "QI01.csv");

        var dataTable = "PowerConsumption";

        try
        {
            Console.WriteLine("Program started...");

            var data =  new CSVReader(filePath, logger).GetCSVData();

            new DataUploader(connectionString, dataTable, data, logger).UploadToSql();


        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            Console.WriteLine($"Error: {ex.Message}");
        }
       
    }
}
