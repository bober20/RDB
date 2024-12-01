using System.Security.Claims;
using CleaningServiceLab6.Services;
using Entities;
using Microsoft.AspNetCore.Mvc;

namespace CleaningServiceLab6.Controllers;

public class QuestionController(DbService dbService) : Controller
{
    // GET
    public async Task<IActionResult> EmployeeQuestions()
    {
        var questions = await dbService.GetAllQuestions();
        var answers = await dbService.GetAllAnswers();
        
        ViewData["questions"] = questions;
        ViewData["answers"] = answers;
        
        return View(new Answer());
    }
    
    public async Task<IActionResult> ClientQuestions()
    {
        var questions = await dbService.GetAllQuestions();
        var answers = await dbService.GetAllAnswers();
        
        ViewData["questions"] = questions;
        ViewData["answers"] = answers;
        
        return View(new Question());
    }
    
    public async Task<IActionResult> CreateQuestion(Question question)
    {
        await dbService.AddPosition(question);
        
        return RedirectToAction("ClientQuestions", "Question");
    }
    
    public async Task<IActionResult> CreateAnswer(Answer answer)
    {
        var userId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        if (userId == 0)
        {
            return RedirectToAction("EmployeeQuestions", "Question");
        }
        answer.EmployeeId = await dbService.GetEmployeeId(userId);
        await dbService.AddAnswer(answer);
        
        return RedirectToAction("EmployeeQuestions", "Question");
    }
}