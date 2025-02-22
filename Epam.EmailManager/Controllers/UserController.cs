using Epam.EmailManager.Application.Services;
using Epam.EmailManager.Domain.Entities;
using Epam.EmailManager.Infrastructure.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Epam.EmailManager.Controllers
{
    public class UserController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IUserDetailsRepository<User> _userService;
        private readonly IEmailServiceRepository<int> _emailService;
        public UserController(IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("connectionString");
            _userService = new UserService<User>(connectionString);
            _emailService = new EmailService<int>(configuration);
        }


        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult UserRegister()
        {
            return View();
        }
        [HttpPost]
        public IActionResult RegisterUserAndSendMail(User user)
        {
            try
            {

                // Register User: get id
                int userId = _userService.AddUser(user);
                if (userId == -1)
                {
                    TempData["Message"] = "Failed to register user!";
                    return RedirectToAction("Index", "Home");
                }

                TempData["Message"] = "User registered successfully!";

                // Send Welcome Mail to id

                try
                {
                    Console.WriteLine("Sending email to user with id: " + userId);
                    var status = _emailService.sendEmailToUserWithId(userId);
                    if (!status.Result)
                    {
                        TempData["Message"] = "Failed to Send Emails!";
                        Console.WriteLine("Failed to send email to user with id: " + userId);
                        return RedirectToAction("Index", "Home");
                    }
                    TempData["Message"] = "Emails were sent successfully!";
                }
                catch (Exception ex)
                {
                    // Handle the exception, log error etc.
                    TempData["Message"] = "Failed to send emails: " + ex.Message;
                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Failed to send emails: " + ex.Message;
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
