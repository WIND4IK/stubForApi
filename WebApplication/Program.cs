using WebApplication;
using WebApplication.Models;

var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders().AddConsole().AddFile("Logs/log-{Date}.txt");

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(x =>
{
    x.JsonSerializerOptions.Converters.Add(new IntConverter());
    x.JsonSerializerOptions.Converters.Add(new StringConverter());
    x.JsonSerializerOptions.Converters.Add(new DateTimeConverterUsingDateTimeParse());
    x.JsonSerializerOptions.Converters.Add(new InheritedTypeJsonConverter<BellaBlock>());
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
