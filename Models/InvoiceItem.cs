using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoicePdfGenerationQueue.Processor.Function.Models
{
    public class InvoiceItem
    {
        public string ItemName { get; set; }
        public string HSNCode { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
