using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LondonEstate.Pages.Admin.Users
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public IndexModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public List<UserViewModel> Users { get; set; } = new();

        public class UserViewModel
        {
            public string Id { get; set; } = default!;
            public string Email { get; set; } = default!;
            public string UserName { get; set; } = default!;
            public bool EmailConfirmed { get; set; }
        }

        public async Task OnGetAsync()
        {
            var users = _userManager.Users.ToList();
            var userViewModels = new List<UserViewModel>();

            foreach (var u in users)
            {
                userViewModels.Add(new UserViewModel
                {
                    Id = u.Id,
                    Email = u.Email ?? string.Empty,
                    UserName = u.UserName ?? string.Empty,
                    EmailConfirmed = await _userManager.IsEmailConfirmedAsync(u)
                });
            }

            Users = userViewModels;
        }
    }
}