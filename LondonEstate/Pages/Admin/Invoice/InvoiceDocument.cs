using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace LondonEstate.Pages.Admin.Invoice;
public class InvoiceDocument : IDocument
{
    private readonly string _logoPath;

    // Pass data in here!
    public InvoiceDocument(string logoPath)
    {
        _logoPath = logoPath;
    }

    public void Compose(IDocumentContainer container)
    {
        container.Page(page => {
            page.Header().Image(_logoPath); // Use it here
        });
    }
}