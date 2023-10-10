using Dropbox.Sign.Api;
using Microsoft.EntityFrameworkCore;
using OpenAI_API;
using SendGrid;
using webapi;
using webapi.Models;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("Dropbox");
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.AddSingleton((services) =>
{
    var api = new OpenAIAPI(configuration["AppSettings:OpenAIKey"]);
    api.Chat.DefaultChatRequestArgs.Model = "gpt-3.5-turbo-16k";
    return api;
});
builder.Services.AddSingleton((services) =>
{
    var config = new Dropbox.Sign.Client.Configuration { Username = configuration["AppSettings:DropboxSignKey"] };
    return new TemplateApi(config);
});

builder.Services.AddSingleton((services) =>
{
    var config = new Dropbox.Sign.Client.Configuration { Username = configuration["AppSettings:DropboxSignKey"] };
    return new SignatureRequestApi(config);
});

builder.Services.AddSingleton((services) =>
{
    var config = new Dropbox.Sign.Client.Configuration { Username = configuration["AppSettings:DropboxSignKey"] };
    return new EmbeddedApi(config);
});

builder.Services.AddSingleton((x) =>
{
    SendGridClient client = new SendGridClient(new SendGridClientOptions { ApiKey = configuration["AppSettings:SendGridKey"] });
    return client;
});

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
