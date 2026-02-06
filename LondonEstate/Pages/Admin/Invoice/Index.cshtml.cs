using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace LondonEstate.Pages.Admin.Invoice;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IWebHostEnvironment _webHostEnvironment;

    public IndexModel(IWebHostEnvironment env)
    {
        _webHostEnvironment = env;  
    }

    [BindProperty]
    public string CompanyName { get; set; } = "Key Bridge Estate";
    
    //[BindProperty]
    //public string InvoiceNumber { get; set; } 
    
    [BindProperty]
    public string IssuedTo { get; set; } 

    [BindProperty]
    public string Property { get; set; } 

    [BindProperty]
    public string AmountPaid { get; set; }

    [BindProperty]
    public DateTime PaymentDate { get; set; } = DateTime.Today;
    
    [BindProperty]
    public string PaymentMethod { get; set; }
    
    [BindProperty]
    public string IssuedBy { get; set; } = "Key Bridge Estate Limited";
    
    [BindProperty]
    public DateTime Date { get; set; } = DateTime.Now;
    
    [BindProperty]
    public DateTime CheckInDate { get; set; } = DateTime.Now;
    
    [BindProperty]
    public DateTime CheckOutDate { get; set; } = DateTime.Now;

    [BindProperty]
    public string Email { get; set; } = "Office@LondonEstatee.co.uk";

    [BindProperty]
    public string Phone { get; set; } = "+44 73 079 33344";

    public string? Message { get; set; }

    public void OnGet()
    {
        // Set default values for the form
        //Today = DateTime.Now;
        //CheckInDate = DateTime.Now;
        //CheckOutDate = DateTime.Now.AddDays(1);
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            Message = "Please fill in all required fields.";
            return Page();
        }

        try
        {
            // Configure QuestPDF license
            QuestPDF.Settings.License = LicenseType.Community;
            string logoPath = Path.Combine(_webHostEnvironment.WebRootPath, "img", "KeyBridgeEstateLogo.png");

            var model = new InvoiceDocument(logoPath);
            // Generate the PDF
            var pdfBytes = GeneratePdf();
            
            // Return the PDF as a downloadable file
            return File(pdfBytes, "application/pdf", $"Invoice_{DateTime.Now:yyyyMMdd-HHmm}.pdf");
        }
        catch (Exception ex)
        {
            Message = $"Error generating PDF: {ex.Message}";
            return Page();
        }
    }

    private byte[] GeneratePdf()
    {
        var invoiceNumber = GenerateInvoiceNumber();
        string logoPath = Path.Combine(_webHostEnvironment.WebRootPath, "img", "KeyBridgeEstateLogo.png");
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(50);
                page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Times New Roman"));

                page.Header().Row(row =>
                {
                    row.RelativeItem(); 
                    row.ConstantItem(150).Image(logoPath); // This adds your logo
                    row.RelativeItem();
                });

                page.Content().Column(column =>
                {
                    column.Spacing(5);

                    // Company Name (Centered, Bold, Large)
                    column.Item().AlignCenter().Text(CompanyName)
                        .FontSize(16)
                        .Bold();

                    //column.Item().PaddingTop(5);

                    // Title
                    column.Item().AlignCenter().Text($"Invoice NO: {invoiceNumber}")
                        .FontSize(14)
                        .Bold();

                    // Separator Line
                    column.Item().LineHorizontal(1).LineColor(Colors.Grey.Medium);

                    column.Item().PaddingTop(15);

                    //Payment Details
                    column.Item().Row(row =>
                    {
                       row.ConstantItem(100).Text("Issued Date:").SemiBold(); // Fixed width for labels
                        row.RelativeItem().Text(Date.ToString());                // Value takes remaining space
                    });
                    column.Item().Row(row =>
                    {
                       row.ConstantItem(100).Text("Issued To:").SemiBold(); // Fixed width for labels
                        row.RelativeItem().Text(IssuedTo);                // Value takes remaining space
                    });
                    column.Item().Row(row =>
                    {
                       row.ConstantItem(100).Text("Property:").SemiBold(); // Fixed width for labels
                        row.RelativeItem().Text(Property);                // Value takes remaining space
                    });

                    column.Item().PaddingTop(5);

                    column.Item().Text("Stay Details:");
                    column.Item().Row(row =>
                    {
                       row.ConstantItem(100).Text("• Check-In:").SemiBold(); // Fixed width for labels
                        row.RelativeItem().Text(CheckInDate.ToString());                // Value takes remaining space
                    });
                    column.Item().Row(row =>
                    {
                       row.ConstantItem(100).Text("• Check-Out:").SemiBold(); // Fixed width for labels
                        row.RelativeItem().Text(CheckOutDate.ToString());                // Value takes remaining space
                    });
                    column.Item().PaddingTop(15);

                    column.Item().Row(row =>
                    {
                       row.ConstantItem(100).Text("Amount Paid:").SemiBold(); // Fixed width for labels
                        row.RelativeItem().Text(AmountPaid);                // Value takes remaining space
                    });

                    column.Item().Row(row =>
                    {
                       row.ConstantItem(100).Text("Payment Date:").SemiBold(); // Fixed width for labels
                        row.RelativeItem().Text(PaymentDate.ToString());                // Value takes remaining space
                    });

                    column.Item().Row(row =>
                    {
                       row.ConstantItem(100).Text("Payment Method:").SemiBold(); // Fixed width for labels
                        row.RelativeItem().Text(PaymentMethod);                // Value takes remaining space
                    });

                    column.Item().PaddingTop(10);

                    // Guest Information
                    column.Item().Text("Thank you for your payment. Should you require further assistance or documentation, please feel free to contact us.");
                    
                    column.Item().PaddingTop(10);

                    column.Item().Row(row =>
                    {
                       row.ConstantItem(100).Text("Issued By:").SemiBold(); // Fixed width for labels
                        row.RelativeItem().Text(IssuedBy);                // Value takes remaining space
                    });
                    
                    //column.Item().LineHorizontal(1).LineColor(Colors.Grey.Medium);
                    
                    page.Footer().Row(row =>
                    {
                        

                        row.ConstantItem(40).Text("Email:").SemiBold(); // Fixed width for labels
                        row.RelativeItem().Text(Email);                // Value takes remaining space
                        row.ConstantItem(40).Text("Phone:").SemiBold(); // Fixed width for labels
                        row.RelativeItem().Text(Phone);
                    });

                });
            });
        });

        return document.GeneratePdf();
    }

    private void AddRuleItem(ColumnDescriptor column, string title, string description)
    {
        column.Item().PaddingBottom(8).Column(col =>
        {
            column.Item().Text(title).Bold();
            column.Item().Text(description);
        });
    }

    //Inside your PDF generation logic
    //public void Compose(IDocumentContainer container)
    //{
    //    // Combine the web root path with your logo's location
    //    string logoPath = Path.Combine(_webHostEnvironment.WebRootPath, "img", "KeyBridgeEstateLogo.png");

    //    container.Page(page =>
    //    {
    //        page.Header().Row(row =>
    //        {
    //            //// Add the image to a column in the header
    //            //row.RelativeItem().Column(column =>
    //            //{
    //            //    column.Item().Image(logoPath).FitWidth();
    //            //});

    //            //row.RelativeItem().AlignRight().Text("Invoice #12345");

    //            row.ConstantItem(100).Text("150").Image(logoPath);

    //            // Spacer
    //            row.RelativeItem();

    //            // Company Info Column
    //            row.RelativeItem().Column(col =>
    //            {
    //                col.Item().Text(CompanyName).FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);
    //                col.Item().Text("123 Tech Lane, NY");
    //            });
    //        });
    //    });
    ////}

    private string GenerateInvoiceNumber()
    {
        // yyyy: 2026
        // MM: 02
        // dd: 05
        // HH: 15 (24-hour format)
        // mm: 35
        return "KBE-" + DateTime.Now.ToString("yyyyMMdd-HHmm");
    }
}
