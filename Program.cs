using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebDataTransfer.Logics;
using WebDataTransfer.Models.DAL.book_excel;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});
// Add services to the container.
builder.Services.AddScoped<ExcelFloder>();
builder.Services.AddControllers();

builder.Services.AddDbContextPool<bookContext>(options =>
{
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptionsAction: sqlOption =>
        {
            sqlOption.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
        });
}, poolSize: 128);


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

app.UseCors("AllowAllOrigins");

app.MapControllers();

app.Run();
