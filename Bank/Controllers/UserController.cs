using Bank.Context;
using Bank.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bank.Controllers;

public class UserController : Controller
{
    protected static MyDbContext _context;
    protected readonly ILogger<UserController> _logger;
    // GET
    public UserController(MyDbContext context, ILogger<UserController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }
    // GET: Get User ID by Email
    public static long GetUserIdByEmail(string email)
    {
        if (email == null)
        {
            throw new InvalidOperationException($"the email can't be empty");
        }
        var user = _context.Users.FirstOrDefault(u => u.UserEmail == email);

        if (user == null)
        {
            throw new InvalidOperationException($"No user found with email: {email}");
        }

        return user.UserId;
    }
}