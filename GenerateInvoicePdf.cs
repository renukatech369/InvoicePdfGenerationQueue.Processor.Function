using InvoicePdfGenerationQueue.Processor.Function.Models;
using InvoicePdfGenerationQueue.Processor.Function.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text.Json;


namespace InvoicePdfGenerationQueue.Processor.Function
{
    public class GenerateInvoicePdf
    {
        private readonly PdfService _pdfService;
       // private readonly InvoiceTemplateService _templateService;
        private readonly BlobStorageService _blobStorageService;

        public GenerateInvoicePdf(
            PdfService pdfService,
            BlobStorageService blobStorageService)
        {
            _pdfService = pdfService;         
            _blobStorageService = blobStorageService;
        }


        [FunctionName("GenerateInvoicePdf")]
        public async Task Run([ServiceBusTrigger("p1-invoicepdf", Connection = "QueueCon")] string message, ILogger log)
        {
            log.LogInformation("Service Bus message received");
            
            var invoice = JsonConvert.DeserializeObject<InvoiceRequest>(message);

            using var pdfStream = _pdfService.GenerateInvoice(invoice);

            await _blobStorageService.UploadPdfAsync(
                $"Invoice_{invoice.InvoiceNo}.pdf",
                pdfStream
            );

            log.LogInformation("Invoice PDF generated and uploaded successfully");
        }
    }
        
   
}

