using System.Text.Json;
using CleaningServiceLab6.Models;
using CleaningServiceLab6.Services;
using Microsoft.AspNetCore.Mvc;

namespace CleaningServiceLab6.Controllers;

public class AdminController(DbService dbService) : Controller
{
    private DbService _dbService = dbService;
    
    public async Task<IActionResult> Index()
    {
        var users = await dbService.GetAllUsers();
        var positions = await dbService.GetAllEmployeesPositions();
        ViewData["users"] = users;
        ViewData["positions"] = positions;
        return View();
    }
    
    [HttpPost]
    [Route("[controller]/update")]
    public async Task<IActionResult> UpdateUser([FromBody] JsonElement data)
    {
        var userId = data.GetProperty("userId").GetInt32();
        var action = data.GetProperty("action").GetBoolean();
        var role = data.GetProperty("role").GetString();
        
        Console.WriteLine(userId + " " + action + " " + role);
        await dbService.UpdateUser(userId, action, role);
        return Ok();
    }
    
    public async Task<IActionResult> DeleteUser(int userId)
    {
        await _dbService.DeleteUser(userId);
        return RedirectToAction("Index", "Admin");
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateEmployeeHandler(EmployeeViewModel employee)
    {
        await _dbService.CreateEmployee(employee);
        return RedirectToAction("Index", "Admin");
    }
}