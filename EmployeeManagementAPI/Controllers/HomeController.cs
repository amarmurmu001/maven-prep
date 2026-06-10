using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementAPI.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
