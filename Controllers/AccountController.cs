using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;

namespace ClearStore.Controllers
{
    [AllowAnonymous]
    [Route("account/accessdenied")]
    public class AccountController : Controller
    {
        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}
