using ExportExcel;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Text;

var builder = new ConfigurationBuilder();
builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

IConfiguration config = builder.Build();

var configQueries = config.GetSection("Queries").GetChildren().ToArray().Select(p => new { Name = p["Name"], Value = p["Value"] }).ToArray();
var commandTextBuilder = new StringBuilder();
foreach (var configQuery in configQueries)
{
    commandTextBuilder.Append(configQuery.Value).AppendLine(";");
}

var worksheetNames = configQueries.Select(x => x.Name).ToArray();

var configParameters = config.GetSection("Parameters").GetChildren().ToArray().Select(p => new { Name = p["Name"], Value = p["Value"] }).ToArray();

var connectionString = config.GetConnectionString("PrimaryDb");
using var connection = new SqlConnection(connectionString);
connection.Open();
using var command = connection.CreateCommand();
command.CommandType = System.Data.CommandType.Text;
command.CommandText = commandTextBuilder.ToString();
foreach (var configParameter in configParameters)
{
    command.Parameters.Add(new SqlParameter(configParameter.Name, configParameter.Value));
}
using var reader = await command.ExecuteReaderAsync();

var directory = config["Export:Directory"];
var prefix = config["Export:FileNamePrefix"];
var filename = $"{DateTime.UtcNow:yyyyMMdd-HHmmss}.xlsx";
if (!string.IsNullOrEmpty(prefix))
{
    filename = $"{prefix}-{filename}";
}
var path = filename;
if (!string.IsNullOrEmpty(directory))
{
    path = Path.Combine(directory, filename);
    if (!Directory.Exists(directory))
    {
        Directory.CreateDirectory(directory);
    }
}
var excelFile = new ExcelFile(path);
var stopwatch = new Stopwatch();
stopwatch.Start();
await excelFile.Create(reader, worksheetNames);
stopwatch.Stop();
Console.WriteLine($"Excel file generated in {stopwatch.Elapsed.TotalMilliseconds}ms");
