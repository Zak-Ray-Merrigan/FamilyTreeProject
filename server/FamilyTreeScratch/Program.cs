using FamilyTreeLibrary;
using FamilyTreeLibrary.Data;
using FamilyTreeLibrary.Data.Databases;
using FamilyTreeLibrary.Data.Files;
using FamilyTreeLibrary.Data.Models;
using FamilyTreeLibrary.Infrastructure;
using FamilyTreeLibrary.Infrastructure.Resource;
using FamilyTreeLibrary.Logging;
using FamilyTreeLibrary.Models;
using FamilyTreeLibrary.Serialization;
using FamilyTreeLibrary.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddFamilyTreeConfiguration();
        services.AddFamilyTreeVault();
        services.AddFamilyTreeStaticStorage();
    })
    .ConfigureLogging((context, logging) =>
    {
        logging.ClearProviders();
        logging.AddFamilyTreeLogger();
        logging.SetMinimumLevel(LogLevel.Debug);
    }).Build();
IExtendedLogger<Program> logger = host.Services.GetRequiredService<IExtendedLogger<Program>>();
const string BLOB_URI = "https://familytreestaticstorage.blob.core.windows.net/templates/Pfingsten-1754729228/20250419-154400.pdf";
try
{
    FamilyTreeStaticStorage staticStorage = host.Services.GetRequiredService<FamilyTreeStaticStorage>();
    PrintTemplate(BLOB_URI, staticStorage, host.Services.GetRequiredService<IExtendedLogger<TemplateReader>>());
}
catch (Exception ex)
{
    logger.LogCritical(ex, "{name}: {message}\n{stackTrace}", ex.GetType().Name, ex.Message, ex.StackTrace);
    Console.WriteLine($"{ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
}

// static string GetBlobUri(FamilyTreeStaticStorage staticStorage, InheritedFamilyName familyName, IExtendedLogger<TemplateGenerator> logger)
// {
//     TemplateGenerator template = new(logger, familyName.Name, familyName.Id);
//     FileStream templateContent = template.WriteTemplate();
//     return staticStorage.UploadTemplate(templateContent, familyName);
// }

static void PrintTemplate(string blobUri, FamilyTreeStaticStorage storage, IExtendedLogger<TemplateReader> logger)
{
    TemplateReader reader = new(storage, logger);
    Template template = reader.ReadTemplate(blobUri);
    Console.WriteLine(template);
}