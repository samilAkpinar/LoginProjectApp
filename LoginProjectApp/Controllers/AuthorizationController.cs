using Microsoft.AspNetCore.Mvc;
using LoginProjectApp.ViewModel;
using Microsoft.AspNetCore.Authorization;
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
        

        [AllowAnonymous]
        [HttpGet()] 
        public bool Get()
        {
            bool result = true;

            //veritabanına veri ekleme
            //_userService.AddUser("samilakpinar", "123456789");

            return result;

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
                return "email veya şifre hatalı";
            }
            //frontend'e token değeri döndürülür.
            return user.Token; 
        }



    }
}
