using InvoicePdfGenerationQueue.Processor.Function.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace InvoicePdfGenerationQueue.Processor.Function.Services;

public class PdfService
{
    public MemoryStream GenerateInvoice(InvoiceRequest invoice)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var stream = new MemoryStream();

        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(25);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Element(c => ComposeHeader(c, invoice));
                page.Content().Element(c => ComposeContent(c, invoice));
                page.Footer().AlignCenter().Text("This is a computer generated GST Invoice");
            });

        }).GeneratePdf(stream);

        stream.Position = 0;
        return stream;
    }
    private void ComposeHeader(IContainer container, InvoiceRequest invoice)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(col =>
            {
                col.Item().Text("ABC TECHNOLOGIES PVT LTD")
                    .FontSize(16).Bold();

                col.Item().Text("GSTIN: 27ABCDE1234F1Z5");
                col.Item().Text("Place of Supply: Maharashtra");
                col.Item().Text("Email: support@abc.com");
            });

            row.ConstantItem(100)
                .Height(60)
                .Element(e =>
                {
                    var logoPath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "Templates",
                        "companylogo.png"
                    );

                    if (File.Exists(logoPath))
                    {
                        e.Image(logoPath);
                    }
                    else
                    {
                        // Prevent function crash if image missing
                        e.AlignCenter().AlignMiddle()
                         .Text("Logo")
                         .FontSize(10);
                    }
                });
        });
    }

      private void ComposeContent(IContainer container, InvoiceRequest invoice)
    {
        decimal subtotal = invoice.Items.Sum(x => x.Price * x.Quantity);
        decimal cgst = subtotal * 0.09m;
        decimal sgst = subtotal * 0.09m;
        decimal grandTotal = subtotal + cgst + sgst;

        container.Column(column =>
        {
            column.Spacing(5);

            // Invoice Info
            column.Item().Row(row =>
            {
                row.RelativeItem().Text($"Invoice No: {invoice.InvoiceNo}").Bold();
                row.RelativeItem().AlignRight()
                    .Text($"Date: {DateTime.Now:dd-MM-yyyy}");
            });

            column.Item().Text($"Bill To: {invoice.CustomerName}");

            column.Item().PaddingVertical(10);

            // Item Table
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3);
                    columns.ConstantColumn(70);
                    columns.ConstantColumn(50);
                    columns.ConstantColumn(80);
                    columns.ConstantColumn(80);
                });

                table.Header(header =>
                {
                    header.Cell().Text("Item").Bold();
                    header.Cell().Text("HSN/SAC").Bold();
                    header.Cell().Text("Qty").Bold();
                    header.Cell().Text("Rate").Bold();
                    header.Cell().Text("Amount").Bold();
                });

                foreach (var item in invoice.Items)
                {
                    table.Cell().Text(item.ItemName);
                    table.Cell().Text(item.HSNCode);
                    table.Cell().Text(item.Quantity.ToString());
                    table.Cell().Text($"₹{item.Price:N2}");
                    table.Cell().Text($"₹{item.Price * item.Quantity:N2}");
                }
            });

            column.Item().PaddingTop(10);

            // GST Summary
            column.Item().AlignRight().Column(totals =>
            {
                totals.Item().Text($"Taxable Amount: ₹{subtotal:N2}");
                totals.Item().Text($"CGST (9%): ₹{cgst:N2}");
                totals.Item().Text($"SGST (9%): ₹{sgst:N2}");
                totals.Item().Text($"Grand Total: ₹{grandTotal:N2}")
                    .FontSize(12).Bold();
            });

            column.Item().PaddingTop(10);
            column.Item().Text($"Amount in Words: {NumberToWords((int)grandTotal)} Only")
                .Italic();

            column.Item().PaddingTop(20);

            column.Item().Row(row =>
            {
                row.RelativeItem().Text("Bank Details:\nA/C No: 1234567890\nIFSC: HDFC0000123");

                row.RelativeItem().AlignRight().Column(sig =>
                {
                    sig.Item().Text("For ZyberPlus Technologies Pvt Ltd");
                    sig.Item().Height(50);
                    sig.Item().Text("Authorized Signatory").Bold();
                });
            });

            // Page Break for Page 2
            column.Item().PageBreak();

            column.Item().Text("Terms & Conditions").FontSize(14).Bold();

            column.Item().Text("1. Goods once sold will not be taken back.");
            column.Item().Text("2. Payment due within 15 days.");
            column.Item().Text("3. Subject to Mumbai jurisdiction.");
        });
    }

    // Convert number to words (basic)
    private string NumberToWords(int number)
    {
        if (number == 0) return "Zero";

        var units = new[]
        {
            "", "One", "Two", "Three", "Four", "Five", "Six",
            "Seven", "Eight", "Nine", "Ten", "Eleven",
            "Twelve", "Thirteen", "Fourteen", "Fifteen",
            "Sixteen", "Seventeen", "Eighteen", "Nineteen"
        };

        var tens = new[]
        {
            "", "", "Twenty", "Thirty", "Forty",
            "Fifty", "Sixty", "Seventy", "Eighty", "Ninety"
        };

        if (number < 20)
            return units[number];

        if (number < 100)
            return tens[number / 10] + " " + units[number % 10];

        if (number < 1000)
            return units[number / 100] + " Hundred " + NumberToWords(number % 100);

        if (number < 100000)
            return NumberToWords(number / 1000) + " Thousand " + NumberToWords(number % 1000);

        return number.ToString();
    }
}
