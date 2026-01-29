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
    public class EstimateRentModel : PageModel
    {
        private readonly IEstimateRequestService _estimateRequestService;
        private readonly ILogError _dbLogger;
        private readonly IEmailSender _emailSender;

        public List<CountryEntry> CountryList { get; set; } = new();

        [BindProperty]
        public CustomerViewModel Customer { get; set; } = new();

        [BindProperty]
        public PropertyViewModel Property { get; set; } = new();

        [BindProperty]
        public List<IFormFile>? ImageFiles { get; set; }

        [TempData]
        public string? AlertMessage { get; set; }

        [TempData]
        public string? AlertType { get; set; }

        public EstimateRentModel(IEmailSender emailSender,
            IEstimateRequestService estimateRequestService, ILogError dbLogger)
        {
            _estimateRequestService = estimateRequestService;
            _dbLogger = dbLogger;
            _emailSender = emailSender;
        }

        public void OnGet()
        {
            FillCountryCodes();
        }

        private void FillCountryCodes()
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
                CustomerId = customer.Id
            };

            try
            {
                if (ImageFiles?.Count > 5)
                {
                    TempData["ErrorMessage"] = "Maximum 5 images allowed";
                    return Page();
                }

                var estimateServiceErrorList = await _estimateRequestService.SubmitEstimateRequest(customer, property, ImageFiles);

                if (estimateServiceErrorList.Count > 0)
                {
                    foreach (var error in estimateServiceErrorList)
                        TempData["ErrorMessage"] = error;
                    return Page();
                }

                await _emailSender.SendEstimateRequestEmailAsync(customer, property);

                //TempData["SuccessMessage"] = "Operation completed successfully!";
            }

            catch (Exception ex)
            {
                Log.Error(ex, "<<<<<<<< Error Submitting estimate >>>>>>>>>");
                await _dbLogger.LogErrorToDb(ex, "index.cshtml.cs");

                TempData["ErrorMessage"] = "Something unexpected happened please try again later";
            }

            return RedirectToPage("/Index");
        }
    }
}

