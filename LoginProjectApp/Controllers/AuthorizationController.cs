using Microsoft.AspNetCore.Mvc;
using LoginProjectApp.Models;
using Microsoft.AspNetCore.Authorization;
using MimeKit;

using MailKit.Security;
using System;

using System.Net;
using MimeKit.Text;
using MailKit.Net.Smtp;
using LoginProjectApp.Services;

namespace LoginProjectApp.Controllers
{
    [Authorize]
    [ApiController] //web api isteklerine cevap verir
    [Route("[controller]s")]
    public class AuthorizationController : ControllerBase
    {

        private IUserService _userService;
        public AuthorizationController(IUserService userService)
        {
            _userService = userService;
            
        }
        
        [AllowAnonymous] //tüm kullanıcılara giriş sağlamaktadır.
        [HttpGet] 
        public IActionResult Get()
        {
            bool result = true;

            //veritabanına veri ekleme
            _userService.AddUser("samilakpinar8@gmail.com", "1234567899");
            
            return Ok(result);

        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public string  Authenticate([FromBody] User users)
        {
            //frontend ten gelen email ve password değerleri veritabanında var ise bir token yaratılır.
            //yaratılan token değeri veritabanına kayıt edilir. ve frontend e geri gönderilir.

            var user = _userService.Authenticate(users.Email, users.Password);
            if (user == null)
            {
                return "false";
            }
            //frontend'e token değeri döndürülür.
            return user.Token; 
        }

        [AllowAnonymous]
        [HttpPost("CheckEmail")]
        public bool ForgottenPassword([FromBody] User user)
        {
            //gelen email değeri veritabanında sorgulanır.
            
            bool state = _userService.ForgottenPassword(user.Email);

           
            if (!state)
            {
                return false;
            }

            //nuget paketi ile email gönderim işlemi buradan gerçekleştirilecektir.
            //create email message
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("samilakpinar8@gmail.com"));
            email.To.Add(MailboxAddress.Parse(user.Email));
            email.Subject = "Test Email Subject";
            email.Body = new TextPart(TextFormat.Html) { Text = "<h1>Example HTML Message Body</h1>" };

            // send email
            using var smtp = new SmtpClient();
            smtp.Connect("smtp.gmail.com", 587, false);
            
                    
            try
            {
                smtp.Authenticate("samilakpinar8@gmail.com", "Youtube1");
                smtp.Send(email);
                smtp.Disconnect(true);
            }
            catch (Exception ex)
            {

                Console.WriteLine("message gitmedi " + ex.Message);
                return false;
            }

            return true;
        }


    }
}
