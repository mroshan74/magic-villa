// using Serilog;
// using MagicVilla_VillaApi.Logging;

using MagicVilla_VillaApi.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Log using serilog, using dependency injection
// Log.Logger =
//     new LoggerConfiguration()
//         .MinimumLevel.Debug()
//         .WriteTo.File("log/villaLogs.txt", rollingInterval: RollingInterval.Day)
//         .CreateLogger();

// builder.Host.UseSerilog();

// Add database connection
var dbString = builder.Configuration.GetConnectionString("DefaultSQLConnection");
builder.Services.AddDbContext<ApplicationDbContext>(option =>
{
    option.UseMySql(dbString, ServerVersion.AutoDetect(dbString));
});

builder.Services
    .AddControllers(option =>
    {
        option.ReturnHttpNotAcceptable = true;
    })
    .AddNewtonsoftJson()
    .AddXmlDataContractSerializerFormatters(); //allow XML format

// Register service for custom implementation of logger via DI
// builder.Services.AddSingleton<ILogging, Logging>();

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

app.UseAuthorization();

app.MapControllers();

app.Run();