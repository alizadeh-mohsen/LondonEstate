using LondonEstate.Models;
using LondonEstate.Services;
using LondonEstate.Utils.Types;
using LondonEstate.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;
using System.Text.Json;

namespace LondonEstate.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IEstimateRequestService _estimateRequestService;
        private readonly ILogger<IndexModel> _logger;
        private readonly IEmailSender _emailSender;

        public List<CountryEntry> CountryList { get; set; } = new();

        [BindProperty]
        public CustomerViewModel Customer { get; set; } = new();

        [BindProperty]
        public PropertyViewModel Property { get; set; } = new();

        // changed to a collection to support multiple uploads
        [BindProperty]
        public List<IFormFile>? ImageFiles { get; set; }

        // TempData properties used to show a Bootstrap alert after redirect
        [TempData]
        public string? AlertMessage { get; set; }

        [TempData]
        public string? AlertType { get; set; }  // e.g. "success", "danger", "warning", "info"

        public IndexModel(IEmailSender emailSender,
            IEstimateRequestService estimateRequestService, ILogger<IndexModel> logger)
        {
            _estimateRequestService = estimateRequestService;
            _logger = logger;
            _emailSender = emailSender;
        }

        public void OnGet()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data\\countryCodes.json");
            var jsonData = System.IO.File.ReadAllText(filePath);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            CountryList = JsonSerializer.Deserialize<List<CountryEntry>>(jsonData, options) ?? new();

        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var customer = new Customer
            {
                Name = Customer.Name,
                Email = Customer.Email,
                CountryCode = Customer.CountryCode,
                PhoneNumber = Customer.Phone
            };

            var property = new Property
            {
                Address = Property.Address,
                NumberOfBeds = (Utils.Enums.NumberOfBeds)Property.NumberOfBeds!,
                SquareMeter = Property.SquareMeter!.Value,
                Customer = customer,
                CustomerId = customer.Id
            };

            try
            {
                // pass the collection of files to the service which includes validation
                await _estimateRequestService.SubmitEstimateRequest(customer, property, ImageFiles);
                await _emailSender.SendEstimateRequestEmailAsync(customer, property);

                TempData["SuccessMessage"] = "Operation completed successfully!";

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error Submitting estimate");
                // surface validation or other errors to the user
                TempData["ErrorMessage"] = "An error occurred: " + ex.Message;
            }

            return RedirectToPage("/Index");
        }
    }
}

