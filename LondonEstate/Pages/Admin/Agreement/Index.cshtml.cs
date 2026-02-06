using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace LondonEstate.Pages.Admin.Agreement;

[Authorize]
public class IndexModel : PageModel
{
    [BindProperty]
    public string CompanyName { get; set; } = "London Estate & Letting Agents Ltd";
    
    [BindProperty]
    public string SortCode { get; set; } = "30-99-50";

    [BindProperty]
    public string Account { get; set; } = "26105560";

    [BindProperty]
    public string Rent { get; set; } = string.Empty;
    
    [BindProperty]
    public string Deposit { get; set; } = "100";
    
    [BindProperty]
    public string GuestName { get; set; } = string.Empty;
    
    [BindProperty]
    public string OwnerName { get; set; } = "Sina Haghighat Parasat";
    
    [BindProperty]
    public DateTime Today { get; set; } = DateTime.Now;
    
    [BindProperty]
    public DateTime CheckInDate { get; set; } = DateTime.Now;
    
    [BindProperty]
    public DateTime CheckOutDate { get; set; } = DateTime.Now.AddDays(1);
    
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
            
            // Generate the PDF
            var pdfBytes = GeneratePdf();
            
            // Return the PDF as a downloadable file
            return File(pdfBytes, "application/pdf", $"GuestAgreement_{GuestName}_{DateTime.Now:yyyyMMdd}.pdf");
        }
        catch (Exception ex)
        {
            Message = $"Error generating PDF: {ex.Message}";
            return Page();
        }
    }

    private byte[] GeneratePdf()
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(50);
                page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Times New Roman"));

                page.Content().Column(column =>
                {
                    column.Spacing(5);

                    // Company Name (Centered, Bold, Large)
                    column.Item().AlignCenter().Text(CompanyName)
                        .FontSize(16)
                        .Bold();

                    //column.Item().PaddingTop(5);

                    // Title
                    column.Item().AlignCenter().Text($"Guest Agreement – {Today:dd/MM/yyyy}")
                        .FontSize(14)
                        .Bold();

                    column.Item().PaddingTop(15);

                    // Payment Details
                    column.Item().Text($"Sort Code: {SortCode}").Bold();
                    column.Item().Text($"Account Number: {Account}").Bold();
                    column.Item().Text($"Accommodation Nightly Charge: £{Rent}").Bold();

                    column.Item().PaddingTop(10);

                    column.Item().Text("*Let me know once the payment has been made. Looking forward to welcoming you!");

                    column.Item().PaddingTop(15);

                    // Guest Information
                    column.Item().Text($"Guest Name: {GuestName}").Bold();
                    column.Item().Text($"Check-in Date: {CheckInDate:dd/MM/yyyy} – 3:00 PM");
                    column.Item().Text($"Check-out Date: {CheckOutDate:dd/MM/yyyy} – 11:00 AM");

                    column.Item().PaddingTop(20);
                    
                    // Separator Line
                    column.Item().LineHorizontal(1).LineColor(Colors.Grey.Medium);
                    
                    column.Item().PaddingTop(10);

                    // Section 1: House Rules
                    column.Item().Text("1. House Rules")
                        .FontSize(12)
                        .Bold();

                    column.Item().PaddingTop(5);

                    column.Item().Text("To ensure a pleasant stay for all guests and neighbours, please adhere to the following rules:");

                    column.Item().PaddingTop(10);

                    AddRuleItem(column, "No Parties or Events:", 
                        "Parties, gatherings, or events are strictly prohibited. Any such activity will result in immediate eviction without a refund.");

                    AddRuleItem(column, "No Smoking:", 
                        "Smoking is not permitted inside the property. Evidence of smoking indoors will lead to forfeiture of the security deposit.");

                    AddRuleItem(column, "Registered Guests Only:", 
                        "Only the individuals listed in the booking are allowed to stay overnight. Unauthorized guests may result in immediate eviction and loss of the security deposit.");

                    AddRuleItem(column, "Noise Levels:", 
                        "Please keep noise to a minimum between 10:00 PM and 8:00 AM. The property is equipped with noise sensors to monitor sound levels.");

                    AddRuleItem(column, "Smoke Detectors:", 
                        "The property is equipped with smoke detectors for your safety. Tampering with these devices is prohibited and may result in penalties.");

                    column.Item().PaddingTop(10);
                    
                    // Separator Line
                    column.Item().LineHorizontal(1).LineColor(Colors.Grey.Medium);
                    
                    column.Item().PaddingTop(10);

                    // Section 2: Security Deposit
                    column.Item().Text("2. Security Deposit")
                        .FontSize(12)
                        .Bold();

                    column.Item().PaddingTop(5);

                    column.Item().Text($"A security deposit of £{Deposit} is required and will be held to cover any potential damages or violations of this agreement. The deposit will be refunded within 7 days after check-out, provided:");

                    column.Item().PaddingTop(5);

                    column.Item().PaddingLeft(20).Text("• No damage is done to the property or its contents.");
                    column.Item().PaddingLeft(20).Text("• No violations of the house rules occur.");
                    column.Item().PaddingLeft(20).Text("• All keys are returned, and the property is left in good condition.");

                    column.Item().PaddingTop(10);
                    
                    // Separator Line
                    column.Item().LineHorizontal(1).LineColor(Colors.Grey.Medium);
                    
                    column.Item().PaddingTop(10);

                    // Section 3: Consequences of Violations
                    column.Item().Text("3. Consequences of Violations")
                        .FontSize(12)
                        .Bold();

                    column.Item().PaddingTop(5);

                    column.Item().Text("Failure to comply with the house rules may result in:");

                    column.Item().PaddingTop(5);

                    column.Item().PaddingLeft(20).Text("• Immediate eviction from the property without a refund.");
                    column.Item().PaddingLeft(20).Text("• Forfeiture of the security deposit.");
                    column.Item().PaddingLeft(20).Text("• Additional charges for damages or extra cleaning.");

                    column.Item().PaddingTop(10);
                    
                    // Separator Line
                    column.Item().LineHorizontal(1).LineColor(Colors.Grey.Medium);
                    
                    column.Item().PaddingTop(10);

                    // Section 4: Acknowledgment and Agreement
                    column.Item().Text("4. Acknowledgment and Agreement")
                        .FontSize(12)
                        .Bold();

                    column.Item().PaddingTop(5);

                    column.Item().Text("By signing below, you acknowledge that you have read, understood, and agree to abide by the terms outlined in this Guest Agreement.");

                    column.Item().PaddingTop(20);
                    

                    // Signatures
                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text($"Guest Signature:");
                            col.Item().Text("                  ").FontFamily("Segoe Script").FontSize(14).Italic(); ;
                            col.Item().PaddingTop(5).Text($"Date: {CheckInDate:dd/MM/yyyy}");
                        });

                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text($"Host Signature:"); 
                            col.Item().Text(OwnerName).FontFamily("Segoe Script").FontSize(14).Italic(); 
                            col.Item().PaddingTop(5).Text($"Date: {CheckOutDate:dd/MM/yyyy}");
                        });
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
            col.Item().Text(title).Bold();
            col.Item().Text(description);
        });
    }
}
