using Epam.EmailManager.Application.Services;
using Epam.EmailManager.Domain.Entities;
using Epam.EmailManager.Infrastructure.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Epam.EmailManager.Controllers
{
    public class EmailController : Controller
    {
        private readonly IEmailServiceRepository<User> _emailService;
        private readonly IUserDetailsRepository<User> _userService;

        public EmailController(IConfiguration configuration)
        {
            _emailService = new EmailService<User>(configuration);
            //Console.WriteLine(configuration["SMTP:Username"]);
            _userService = new UserService<User>(configuration.GetConnectionString("connectionString"));
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost] //restricts method to take only POST requests
        public IActionResult SendEmailToAll()
        {
            try
            {
                var users = _userService.GetAllUsers();
                if (users == null)
                {
                    Console.WriteLine("No users found");
                    TempData["Message"] = "No users Found!";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    Console.WriteLine("Users found: " + users.Count);
                }
                var status = _emailService.sendEmailToAllUsers(users);
                if (!status.Result)
                {
                    TempData["Message"] = "Failed to Send Emails!";
                    return RedirectToAction("Index", "Home");
                }
                TempData["Message"] = "Emails were sent successfully!";
            }
            catch (Exception ex)
            {
                // Handle the exception, log error etc.
                TempData["Message"] = "Failed to send emails: " + ex.Message;
            }
            // Redirects to Home/Index.cshtml
            return RedirectToAction("Index", "Home");
        }
    }
}
