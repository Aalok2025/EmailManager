Epam.EmailManager.Application  
├── Epam.EmailManager (MVC Application)  
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
├── Epam.EmailManager.Application (.NET Core Library)  
│   ├── Services/  
│   │   ├── EmailService.cs  
│   │   ├── UserServices.cs  
│  
├── Epam.EmailManager.Domain (.NET Core Library)  
│   ├── Entities/  
│   │   ├── User.cs  
│  
├── Epam.EmailManager.Infrastructure (.NET Core Library)  
│   ├── Repository/  
│   │   ├── IEmailService.cs  
│   │   ├── IUserDetailsRepository.cs  
│   │   ├── IEmailLogRepository.cs  
│  
├── Epam.EmailManager.Middleware (.NET Core Library)  
│   ├── Log/  
│   │   ├── EmailLogService.cs  
│  
└── Epam.EmailManager.sln (Solution File)  

	1. Database first Approach
	2. Epam.EmailManager/View/Home/Index.cshtml -> button [Send sale-alert mail to all users in db] -> Epam.EmailManager/Controllers/EmailController.cs -> Epam.EmailManager.Infrastructure/Repository/IEmailServiceRepository.cs ->  Epam.EmailManager.Application/Services/EmailService.cs ->  [ Epam.EmailManager.Infrastructure/Repository/IUserDetailsRepository.cs ->  Epam.EmailManager.Application/Services/UserService.cs ->  Epam.EmailManager.Domain/DataAccess/UserRepository.cs -> Get All Users ] ->[ Send Mail to all ] + [ IEmailLogRepository.cs -> EmailLogService.cs ->  Log Email sent ]
	2. Epam.EmailManager/View/Home/Index.cshtml -> button [Register new user + Send a Welcome mail] -> Epam.EmailManager/Views/User/UserRegister.cshtml -> Epam.EmailManager/Controllers/Controllers/UserController -> (Register user to db : returns int id) -> Epam.EmailManager.Infrastructure/Repository/IUserDetailsRepository.cs ->  Epam.EmailManager.Application/Services/UserService.cs -> (Add User : return id) 
																																																					-> (Send Welcome mail to user with id=id) -> Epam.EmailManager.Infrastructure/Repository/IEmailServiceRepository.cs -> Epam.EmailManager.Application/Services/EmailService.cs -> (Send welcome mail to user with id)  + [ IEmailLogRepository.cs -> EmailLogService.cs ->  Log Email sent ]
Technology Stack

Frontend: ASP.NET Core MVC

Backend: .NET Core (C#)

Database: SQL Server

Data Access: ADO.NET

Email Service: SMTP

Logging: Custom EmailLogService
