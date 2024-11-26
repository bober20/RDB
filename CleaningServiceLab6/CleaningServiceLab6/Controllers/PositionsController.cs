using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using CleaningServiceLab6.Models;
using CleaningServiceLab6.Services;
using Entities;

namespace CleaningServiceLab6.Controllers;

public class PositionsController(DbService dbService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var positions = await dbService.GetAllEmployeesPositions();
        var vacancies = await dbService.GetAllVacancies();

        ViewData["positions"] = positions;
        ViewData["vacancies"] = vacancies;
        
        return View(new EmployeePosition());
    }

    public async Task<IActionResult> AddPosition(EmployeePosition position)
    {
        await dbService.AddPosition(position);
        
        return RedirectToAction("Index", "Positions");
    }
    
    public async Task<IActionResult> DeletePosition(int position)
    {
        await dbService.DeletePosition(position);
        
        return RedirectToAction("Index", "Positions");
    }
    
    [HttpPost]
    public async Task<IActionResult> UpdateVacancyNumber([FromBody] JsonElement data)
    {
        var vacancyId = data.GetProperty("vacancyId").GetInt32();
        var vacancyNumber = Convert.ToInt32(data.GetProperty("vacancyNumber").GetString());
        
        await dbService.UpdateVacancyNumber(vacancyId, vacancyNumber);

        return RedirectToAction("Index", "Positions");
    }
    
    [HttpPost]
    public async Task<IActionResult> UpdateVacancyDescription([FromBody] JsonElement data)
    {
        var vacancyId = data.GetProperty("vacancyId").GetInt32();
        var vacancyDescription = data.GetProperty("vacancyDescription").GetString();
        
        await dbService.UpdateVacancyDescription(vacancyId, vacancyDescription);

        return RedirectToAction("Index", "Positions");
    }
}