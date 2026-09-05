using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Vasko.Data;

namespace Vasko.Controllers
{
    [Authorize]
    public class ImageController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public ImageController(UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
        {
            _userManager = userManager;
            _env = env;
        }

        public async Task<IActionResult> GetAvatar()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (user != null && user.Avatar != null && user.Avatar.Length > 0)
            {
                return File(user.Avatar, "image/png");
            }

            // Путь к аватару по умолчанию
            var path = Path.Combine(_env.WebRootPath, "images", "5449511.png");

            if (!System.IO.File.Exists(path))
            {
                return NotFound();
            }

            // Используем PhysicalFile для отправки файла с диска
            return PhysicalFile(path, "image/png");
        }
    }
}