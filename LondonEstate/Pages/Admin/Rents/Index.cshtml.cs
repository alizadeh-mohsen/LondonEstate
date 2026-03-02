using LondonEstate.Models;
using LondonEstate.Utils.Extensions;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LondonEstate.Pages.Admin.Rents
{
    public class IndexModel : PageModel
    {
        private readonly Data.ApplicationDbContext _context;

        public string TotalRentAmount { get; set; }

        public IndexModel(Data.ApplicationDbContext context)
        {
            _context = context;
        }

        // Replaced plain Rent list with a view model that includes computed warning info
        public IList<RentListItem> RentItems { get; set; } = new List<RentListItem>();

        // View model representing a rent row with computed warning/sort information
        public sealed record RentListItem
        {
            public Rent Rent { get; init; } = default!;
            public RentHistory? LastSettlement { get; init; }
            public int DaysPast { get; init; }
            public string? WarningClass { get; init; }
            public string? WarningText { get; init; }
            public int SortKey { get; init; }
        }

        public async Task OnGetAsync()
        {
            var rents = await _context.Rent.ToListAsync();

            TotalRentAmount = rents.Sum(r => r.RentAmount).ToUkCurrencyString();

            // Load histories for the rents we have to avoid N+1 queries
            var rentIds = rents.Select(r => r.Id).ToList();
            var histories = await _context.Set<RentHistory>()
                                          .Where(h => rentIds.Contains(h.RentId))
                                          .OrderByDescending(h => h.PaidDate)
                                          .ToListAsync();

            // Build a lookup of latest history per rent
            var latestLookup = histories
                .GroupBy(h => h.RentId)
                .ToDictionary(g => g.Key, g => g.First());

            var todayUtc = DateTime.UtcNow.Date;

            var items = new List<RentListItem>(rents.Count);
            foreach (var rent in rents)
            {
                latestLookup.TryGetValue(rent.Id, out var last);

                // Determine whether rent has been paid in the current month (simple rule)
                var paidThisPeriod = last is not null &&
                                     last.PaidDate.ToUniversalTime().Year == todayUtc.Year &&
                                     last.PaidDate.ToUniversalTime().Month == todayUtc.Month;

                // Calculate days difference between today and due day-of-month
                var daysPast = todayUtc.Day - rent.DueDate; // >0 means past due, 0 due today, <0 not yet due

                int sortKey;
                string? warningClass = null;
                string? warningText = null;

                if (paidThisPeriod)
                {
                    // Paid this period -> lowest priority in ordering (put at bottom)
                    sortKey = int.MinValue / 2;
                }
                else
                {
                    // Not paid this period -> decide warning
                    sortKey = daysPast; // larger positive => more late => higher priority
                    if (daysPast > 0)
                    {
                        warningClass = "bg-danger text-white";
                        warningText = $"Past due ({daysPast} day{(daysPast == 1 ? "" : "s")})";
                    }
                    else if (daysPast == 0)
                    {
                        warningClass = "bg-warning text-dark";
                        warningText = "Due today";
                    }
                    else
                    {
                        warningClass = null;
                        warningText = null;
                    }
                }

                items.Add(new RentListItem
                {
                    Rent = rent,
                    LastSettlement = last,
                    DaysPast = daysPast,
                    WarningClass = warningClass,
                    WarningText = warningText,
                    SortKey = sortKey
                });
            }

            // Order by SortKey descending (biggest late first), then by address
            RentItems = items.OrderByDescending(i => i.SortKey)
                             .ThenBy(i => i.Rent.Address, StringComparer.OrdinalIgnoreCase)
                             .ToList();
        }
    }
}