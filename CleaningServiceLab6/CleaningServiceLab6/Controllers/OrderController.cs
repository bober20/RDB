using System.Security.Claims;
using CleaningServiceLab6.Services;
using Entities;
using Microsoft.AspNetCore.Mvc;

namespace CleaningServiceLab6.Controllers;

public class OrderController(DbService dbService) : Controller
{
    public async Task<IActionResult> CreateHandler(Order order)
    {
        await dbService.CreateOrder(order);
        return RedirectToAction("Index", "Order");
    }

    // public IActionResult CreateHandler()
    // {
    //     return View();
    // }

    public async Task<IActionResult> AddService(int orderId, int serviceId)
    {
        await dbService.AddServiceToOrder(orderId, serviceId);
        return RedirectToAction("Index", "Order");
    }

    public async Task<IActionResult> RemoveService(int orderId, int serviceId)
    {
        await dbService.RemoveServiceFromOrder(orderId, serviceId);
        return RedirectToAction("Index", "Order");
    }

    public async Task<IActionResult> DeleteOrder(int order)
    {
        await dbService.DeleteOrder(order);

        return RedirectToAction("Index", "Order");
    }

    public async Task<IActionResult> Index()
    {
        var userId = HttpContext.User.Claims
            .FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier, StringComparison.OrdinalIgnoreCase))?
            .Value;

        List<Order> orders = await dbService.GetAllOrders(int.Parse(userId));
        var services = await dbService.GetAllServices();
        var employees = await dbService.GetAllEmployees();
        ViewData["orders"] = orders;
        ViewData["services"] = services;
        ViewData["employees"] = employees;
        ViewData["bonuses"] = await dbService.GetAllBonuses();

        ViewData["clientId"] = await dbService.GetClientId(Convert.ToInt32(userId));

        return View();
    }
}