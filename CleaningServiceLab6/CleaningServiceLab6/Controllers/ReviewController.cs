using System.Security.Claims;
using CleaningServiceLab6.Services;
using Entities;
using Microsoft.AspNetCore.Mvc;

namespace CleaningServiceLab6.Controllers;

public class ReviewController(DbService dbService) : Controller
{
    // GET
    public async Task<IActionResult> Index()
    {
        var reviews = await dbService.GetAllReviews();
        
        ViewData["reviews"] = reviews;
        
        return View(new Review());
    }
    
    public async Task<IActionResult> CreateReview(Review review)
    {
        var userId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        review.ClientId = await dbService.GetClientId(userId);
        
        await dbService.AddReview(review);
        
        return RedirectToAction("Index", "Review");
    }
}