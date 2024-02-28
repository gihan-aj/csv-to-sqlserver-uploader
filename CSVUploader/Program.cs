using CSVUploader;
using Microsoft.Extensions.Configuration;

public class Program    
{
    public static void Main(string[] args)
    {
        string appDirectory = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", ".."));

        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(appDirectory)
            .AddJsonFile("appsetting.json")
            .Build();

        string? connectionString = configuration.GetConnectionString("PowerReadingsDBConnectionString");

        //var filePath = Path.GetFullPath((Path.Combine(appDirectory, "QI01.csv")));
        var filePath = Path.Combine(appDirectory, "QI01.csv");

        var dataTable = "PowerConsumption";

        try
        {
            Console.WriteLine("Program started...");

            var data =  new CSVReader(filePath).GetCSVData();

            new DataUploader(connectionString, dataTable, data).UploadToSql();


        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
       
    }
}
