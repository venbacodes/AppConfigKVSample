using AppConfigKVSample;
using Microsoft.Extensions.DependencyInjection.Extensions;

var _logger = LoggerExtension.GetConsoleLogger<Program>();

var builder = WebApplication.CreateBuilder(args);

builder.UseAzureAppConfiguration(_logger);

// Add services to the container.
builder.Services.AddRazorPages();

var acDemo = new ACDemo();
builder.Configuration.Bind("ACDemo", acDemo);

builder.Services.AddSingleton(acDemo);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
