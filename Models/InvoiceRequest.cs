using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoicePdfGenerationQueue.Processor.Function.Models
{
    public class InvoiceRequest
    {
        public string InvoiceNo { get; set; }
        public string CustomerName { get; set; }
        public List<InvoiceItem> Items { get; set; }
    }
}
