using CleaningServiceLab6.Services;
using Entities;
using Microsoft.AspNetCore.Mvc;

namespace CleaningServiceLab6.Controllers;

public class ServiceController(DbService dbService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var services = await dbService.GetAllServices();

        ViewData["services"] = services;
        
        return View(new Service());
    }
    
    public async Task<IActionResult> CreateService(Service service)
    {
        await dbService.CreateService(service);
        return RedirectToAction("Index", "Service");
    }
}