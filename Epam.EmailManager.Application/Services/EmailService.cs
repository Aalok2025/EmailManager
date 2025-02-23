using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Epam.EmailManager.Domain.Entities;
using Epam.EmailManager.Infrastructure.Repository;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MailKit.Net.Smtp;
using MimeKit;
using Email.EmailManager.Middleware.Log;

namespace Epam.EmailManager.Application.Services
{
    public class EmailService<T> :IEmailServiceRepository <T>
    {
        public event EmailSentHandler OnEmailSent;
        private readonly IConfiguration _emailConfiguration;
        private readonly IEmailLogRepository<string> _emailLogService = new EmailLogService(); 
        IUserDetailsRepository<User> _userDetailsRepository;
        public EmailService(){}
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
                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress("ShoppingWebsite", "NoReply@shopping.in"));
                    message.To.Add(new MailboxAddress(user.Name, user.Email));
                    message.Subject = "SALE STARTS SOON!";

                    // 2. Set the body of the message
                    message.Body = new TextPart("plain")
                    {
                        Text = $"Hey {user.Name}! We are back with our offer. Check our latest sale before it breaks!"
                    };

                    // 3. Setup and use the SMTP client
                    using var client = new SmtpClient();
                    try
                    {
                        Console.WriteLine("Host: " + _emailConfiguration["SMTP:Host"]);
                        Console.WriteLine("Port: " + _emailConfiguration["SMTP:Port"]);
                        Console.WriteLine("Username: " + _emailConfiguration["SMTP:Username"]);
                        client.Connect(

                            _emailConfiguration["SMTP:Host"],
                            int.Parse(_emailConfiguration["SMTP:Port"]),
                            SecureSocketOptions.StartTls);

                        client.Authenticate(
                            _emailConfiguration["SMTP:Username"],
                            _emailConfiguration["SMTP:Password"]);

                        await client.SendAsync(message);
                        // Log the email by invoking event which triggers subscribed methods with same signature
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

        public async Task<bool> sendEmailToUserWithId(T id)
        {
            _userDetailsRepository = new UserService<User>(_emailConfiguration.GetConnectionString("ConnectionString"));
            int idInt = Convert.ToInt32(id);
            Console.WriteLine("in SendEmailTo user" + idInt);
            var user = _userDetailsRepository.GetUserById(idInt);
            if(user==null)
            {
                Console.WriteLine("No user found with this id");
                return false;
            }
            try
            {
                // 1. Create a new message
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("ShoppingWebsite", "NoReply@shopping.in"));
                message.To.Add(new MailboxAddress(user.Name, user.Email));
                message.Subject = "Welcome User!";

                // 2. Set the body of the message
                message.Body = new TextPart("plain")
                {
                    Text = $"Hey {user.Name}! You have successfully Registered yourself!"
                };

                // 3. Setup and use the SMTP client
                using var client = new SmtpClient();
                try
                {
                    client.Connect(

                            _emailConfiguration["SMTP:Host"],
                            int.Parse(_emailConfiguration["SMTP:Port"]),
                            SecureSocketOptions.StartTls);

                    client.Authenticate(
                        _emailConfiguration["SMTP:Username"],
                        _emailConfiguration["SMTP:Password"]);

                    await client.SendAsync(message);
                    // Log the email
                    _emailLogService.LogEmailDetails(user.Email, message.Subject, message.Body.ToString());
                    OnEmailSent?.Invoke(user.Email, message.Subject, message.Body.ToString());
                    return true;
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
            //send mail
            return true;
        }
    }
}


