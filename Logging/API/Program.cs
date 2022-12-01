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

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

var TestErrors = new[] 
{
    new Logs.Error("Program.cs", "TestErrors", "Error 1"),
    new Logs.Error("Program.cs", "TestErrors", "Error 2"),
    new Logs.Error("Program.cs", "TestErrors", "Error 3"),
    new Logs.Error("Program.cs", "TestErrors", "Error 4")
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.MapGet("/Error/GetAll", () => 
{ 
    var result = Enumerable.Range(0, TestErrors.Length).Select(index =>
        TestErrors[index].ToRec()
        // new ErrorRec
        // (
        //     TestErrors[index].Id,
        //     TestErrors[index].File,
        //     TestErrors[index].Function,
        //     TestErrors[index].Message
        // )
    ).ToArray();
    return result; 
})
.WithName("GetAllErrors")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

