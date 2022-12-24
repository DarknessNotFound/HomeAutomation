using Microsoft.Data.Sqlite;
using Authentication;
using System.IO;

string target = Directory.GetCurrentDirectory() + "/data";
if (!Directory.Exists(target))
{
    Directory.CreateDirectory(target);
}

// Change the current directory.
Environment.CurrentDirectory = (target);

//Setup the database.
List<string> TablesSql = new List<string>();
#region 

TablesSql.Add($"""
CREATE TABLE IF NOT EXISTS {Logs.Error.Table}(
   Id INTEGER PRIMARY KEY AUTOINCREMENT,
   Program        INTEGER   NOT NULL,
   File           TEXT      NULL,
   Function       TEXT      NULL,
   Message        TEXT      NULL
);
""");
#endregion
Authentication.ApiKey.InitialSetup();
Authentication.ApiKey.PrintAllKeys();

using (var connection = new SqliteConnection(Constants.Conn))
{
    connection.Open();
    var command = connection.CreateCommand();
    foreach(string Table in TablesSql)
    {
        command.CommandText = Table;
        command.ExecuteNonQuery();
    }
}

var TestError = new Logs.Error("Program.cs", "Main", "This is a test message.");
TestError.Insert(1);

//API stuff
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<Authentication.ApiKeyMiddleware>();
app.MapGet("/", () => "Logging pinged successful!");

app.MapGet("/Errors", () => {
    return Logs.Error.GetAllErrors().Select(error => error.ToRec()).ToArray();
});

app.MapApiKeyEndpoints();

app.Run();
