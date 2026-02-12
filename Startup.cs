using InvoicePdfGenerationQueue.Processor.Function.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(InvoicePdfGenerationQueue.Processor.Function.Startup))]
namespace InvoicePdfGenerationQueue.Processor.Function
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<PdfService>();            
            builder.Services.AddSingleton(sp =>
                new BlobStorageService(
                    Environment.GetEnvironmentVariable("BlobStorageConnection"),
                    Environment.GetEnvironmentVariable("InvoiceContainerName")
                ));
        }
    }
}
