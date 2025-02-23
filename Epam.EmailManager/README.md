﻿Explain onion architecture and Step by step complete implementation of an asp.net core mvc application 
using onion architecture with Controller -> Application -> Domain (Entity / Repository / EF db context) 
for the below scenario - Compose an email and when sent to Action method, 
it should send the email to all register email addresses in database using Google smtp. 
Provide all required steps to setup smtp and required dependencies.

✅ Controllers handle API calls & UI interactions.
✅ Application Layer implements business logic.
✅ Domain Layer executes database operations via ADO.NET.
✅ Infrastructure Layer defines repository interfaces.
✅ Middleware Layer logs email transactions.

Epam.EmailManager.Application
│── Epam.EmailManager (MVC Application)
│   ├── Controllers/
│   │   ├── EmailController.cs
│   │   ├── UserController.cs
│   │
│   ├── Models/  (EF Core Models)
│   │   ├── UserDetails.cs
│   │
│   ├── Views/
│   │   ├── Home/
│   │   │   ├── Index.cshtml
│   │   ├── User/
│   │   │   ├── UserRegister.cshtml
│   │
│   ├── appsettings.json (Stores connection strings)
│
│── Epam.EmailManager.Application (.NET Core Library)
│   ├── Services/
│   │   ├── EmailService.cs
│   │   ├── UserServices.cs
│
│── Epam.EmailManager.Domain (.NET Core Library)
│   ├── Entities/
│   │   ├── User.cs
│
│── Epam.EmailManager.Infrastructure (.NET Core Library)
│   ├── Repository/
│   │   ├── IEmailService.cs
│   │   ├── IUserDetailsRepository.cs
│   │   ├── IEmailLogRepository.cs
│
│── Epam.EmailManager.Middleware (.NET Core Library)
│   ├── Log/
│   │   ├── EmailLogService.cs
│
│── Epam.EmailManager.sln (Solution File)


	1. Database first Approach
	2. Epam.EmailManager/View/Home/Index.cshtml -> button [Send sale-alert mail to all users in db] -> Epam.EmailManager/Controllers/EmailController.cs -> Epam.EmailManager.Infrastructure/Repository/IEmailServiceRepository.cs ->  Epam.EmailManager.Application/Services/EmailService.cs ->  [ Epam.EmailManager.Infrastructure/Repository/IUserDetailsRepository.cs ->  Epam.EmailManager.Application/Services/UserService.cs ->  Epam.EmailManager.Domain/DataAccess/UserRepository.cs -> Get All Users ] ->[ Send Mail to all ] + [ IEmailLogRepository.cs -> EmailLogService.cs ->  Log Email sent ]
	2. Epam.EmailManager/View/Home/Index.cshtml -> button [Register new user + Send a Welcome mail] -> Epam.EmailManager/Views/User/UserRegister.cshtml -> Epam.EmailManager/Controllers/Controllers/UserController -> (Register user to db : returns int id) -> Epam.EmailManager.Infrastructure/Repository/IUserDetailsRepository.cs ->  Epam.EmailManager.Application/Services/UserService.cs -> (Add User : return id) 
																																																					-> (Send Welcome mail to user with id=id) -> Epam.EmailManager.Infrastructure/Repository/IEmailServiceRepository.cs -> Epam.EmailManager.Application/Services/EmailService.cs -> (Send welcome mail to user with id)  + [ IEmailLogRepository.cs -> EmailLogService.cs ->  Log Email sent ]
			

Epam.EmailManager.Application
│── Epam.EmailManager.Application.Presentation (MVC Application)a
-> takes care of connection string stored in application.json and handles dependency injection which is shared across solution
-> Models (EF Core Models) 
userDetails.cs( created by database first approach)

-> Controllers (Handles Requests) 
EmailController.cs
(HttpPost/SendEmailToAll()) this method should use Repository/IEmailService(SendMailToAll()) which is implemented in Service/EmailService which uses ado.net in infrastructure library to get list of all users from db and then we send discount details mail to all users using email smtp.
(HttpPost/SendMailToUser(id) this method should take user inputs from userRegister.cshtml form and with this add this user to db and send a welcome mail. It must use IUserDetailsRepository.cs implemented in UserServices.cs.

-> Views (UI if MVC)
Index.cshtml( has a button which will send mail to all users. another buttton which navigates to another view where user can register itself-> UserRegister)
User/UserRegister.cshtml( here a form takes in inputs name email to add in database table + once submitted we have new user and we send welcome mail so send it to controller.)

│── Epam.EmailManager.Application.Application (.NET Core Library)
-> Services/
EmailService.cs (SendMailToAll() + SendMailToUser(Id))
UserServices.cs (GetUsersFromDatabase() + GetUserById()) -> uses ado.net

│── Epam.EmailManager.Application.Domain (.NET Core Library)
-> Repository/
IEmailService.cs 
IUserDetailsRepository.cs
IEmailLogRepository.cs

│── Epam.EmailManager.Application.Infrastructure (.NET Core Library)
-> uses Ado.net to get database access for the library's other than mvc.
create necessary files, folders and methods. and it gets connection string from mvc dependency injection

|____Epam.EmailManager.Application.Middleware (.Net Core Library)
-> Log/EmailLogService.cs ( with each mail sent out we log to whom mail was sent. )

│── Epam.EmailManager.Application.sln (Solution File)


🔹 Technology Stack
Frontend: ASP.NET Core MVC
Backend: .NET Core (C#)
Database: SQL Server
Data Access: ADO.NET
Email Service: SMTP
Logging: Custom EmailLogService

Epam.EmailManager.Domain/Entities/Delgates.cs : 
namespace Epam.EmailManager.Domain.Entities
{
    public delegate void EmailSentHandler(string recipient, string subject, string body);
}

// This delegate (EmailSentHandler) defines a function signature for handling email-sent events.
// It is used in EmailService<T> to notify when an email is successfully sent.

Epam.EmailManager.Middleware/Log/EmailLogService.cs :
namespace Email.EmailManager.Middleware.Log
{
    public class EmailLogService : IEmailLogRepository<string>
    {
        public void LogEmailDetails(string recipient, string subject, string body)
        {
            Console.WriteLine($"[LOG] Email Sent - To: {recipient}, Subject: {subject} at {DateTime.Now}");
        }
    }
}
Epam.EmailManager.Infrastructure/Repository/IEmailLogRepository.cs : 
namespace Epam.EmailManager.Infrastructure.Repository
{
    public interface IEmailLogRepository<T>
    {
        void LogEmailDetails(T recipient, T subject, T body);
    }
}
Epam.EmailManager.Application/Services/EmailService.cs :
namespace Epam.EmailManager.Application.Services
{
    public class EmailService<T> :IEmailServiceRepository <T>
    {
        public event EmailSentHandler OnEmailSent;
        private readonly IEmailLogRepository<string> _emailLogService = new EmailLogService(); 
        
        public EmailService(IConfiguration configuration)
        {
            _emailConfiguration = configuration;
            // Subscribe the logging method to the event
            OnEmailSent += _emailLogService.LogEmailDetails;
        }
        
        public async Task<bool> sendEmailToAllUsers(List<User> users)
        {
            foreach (var user in users)
            {
                try
                {
                    // 1. Create a new message
                    // 2. Set the body of the message
                    // 3. Setup and use the SMTP client
                    using var client = new SmtpClient();
                    try
                    {
                        client.Connect();
                        client.Authenticate();
                        await client.SendAsync(message);

                        // Trigger the event by invoking so that all subscribers listening to event executes the methods and logs the email sent
                        OnEmailSent?.Invoke(user.Email, message.Subject, message.Body.ToString());
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.ToString());
                        return false;
                    }
                    finally
                    {
                        client.Disconnect(true);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.ToString());   
                    return false;
                }
            }
            return true;
        }
    }