using LondonEstate.Models;
using LondonEstate.Services;
using LondonEstate.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LondonEstate.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IEstimateRequestService _estimateRequestService;
        private readonly ILogger<IndexModel> _logger;
        private readonly IEmailSender _emailSender;

        public IndexModel(IEmailSender emailSender,
            IEstimateRequestService estimateRequestService, ILogger<IndexModel> logger)
        {
            _estimateRequestService = estimateRequestService;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public CustomerViewModel Customer { get; set; } = new();

        [BindProperty]
        public PropertyViewModel Property { get; set; } = new();

        [BindProperty]
        public IFormFile? ImageFile { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // create customer
            var customer = new Customer
            {
                Name = Customer.Name,
                Email = Customer.Email,
                CountryCode = Customer.CountryCode,
                PhoneNumber = Customer.Phone
            };


            // create property
            var property = new Property
            {
                Address = Property.Address,
                NumberOfBeds = Property.NumberOfBeds,
                SquareMeter = Property.SquareMeter,
                Customer = customer,
                CustomerId = customer.Id
            };

            try
            {
                await _estimateRequestService.SubmitEstimateRequest(customer, property, ImageFile);
                //await _emailSender.SendAsync(customer, property, savedFilePath);
                await _emailSender.SendEstimateRequestEmailAsync(customer, property);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send submission email.");
            }

            return RedirectToPage("/Index");
        }
    }
}