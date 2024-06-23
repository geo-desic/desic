using ExportExcel;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Text;

var builder = new ConfigurationBuilder();
builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

IConfiguration config = builder.Build();

var configQueries = config.GetSection("Queries").GetChildren().ToArray().Select(p => new { Name = p["Name"], Value = p["Value"] }).ToArray();
var commandTextBuilder = new StringBuilder();
foreach (var configQuery in configQueries)
{
    commandTextBuilder.Append(configQuery.Value);
    commandTextBuilder.AppendLine(";");
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

var filename = $"{config["ExportFileNamePrefix"]}-{DateTime.UtcNow:yyyyMMdd-HHmmss}.xlsx";
var excelFile = new ExcelFile(filename);
await excelFile.Create(reader, worksheetNames);
