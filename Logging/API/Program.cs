using Microsoft.Data.Sqlite;
using System.IO;

string target = Directory.GetCurrentDirectory() + "/data";
Console.WriteLine("The current directory is {0}", Directory.GetCurrentDirectory());
if (!Directory.Exists(target))
{
    Directory.CreateDirectory(target);
}

// Change the current directory.
Environment.CurrentDirectory = (target);
Console.WriteLine("Now, the current directory is {0}", Directory.GetCurrentDirectory());

//Setup the database.
List<string> TablesSql = new List<string>();
#region 
TablesSql.Add("""
CREATE TABLE IF NOT EXISTS ApiKeys(
   Id INTEGER PRIMARY KEY AUTOINCREMENT,
   Name     TEXT     NOT NULL,
   KEY      TEXT     NOT NULL,
   IsDeleted BOOLEAN NOT NULL CHECK (IsDeleted IN (0, 1)) DEFAULT 0
);
""");

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
using (var connection = new SqliteConnection(Constants.Conn))
{
    connection.Open();
    var command = connection.CreateCommand();
    foreach(string Table in TablesSql)
    {
        command.CommandText = Table;
        command.ExecuteNonQuery();
    }

    Console.WriteLine("Admin API Key: NOT IMPLEMENTED");
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

app.MapGet("/", () => "Logging pinged successful!");

app.MapGet("/Errors", () => {
    return Logs.Error.GetAllErrors().Select(error => error.ToRec()).ToArray();
});

app.Run();
