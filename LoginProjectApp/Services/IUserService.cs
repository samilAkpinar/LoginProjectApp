using LoginProjectApp.ViewModel;
using LoginProjectApp.Helpers;
using System;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using LoginProjectApp.DataAccess.concretes;

namespace LoginProjectApp.Services
{
    public interface IUserService
    {
        void AddUser(string email, string password);
        User Authenticate(string email, string password);
        
    }

    public class UserService : IUserService
    {

        private readonly AppSettings _appSettings;
        private FirebaseAccess _firebaseAccess;
        public UserService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
            _firebaseAccess = new FirebaseAccess();
        }

           
        public void FirebaseConnect()
        {
            //veritabanına bağlanma 
            string path = AppDomain.CurrentDomain.BaseDirectory + @"loginproject-319714-firebase-adminsdk-cnmg6-207ae21fd8.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
        }


        public User Authenticate(string email, string password)
        {
            //gelen email ve password değerleri veribanına sorgulanır.
            var state = _firebaseAccess.CheckEmailAndPassword(email, password).Result;

            if (!state)
            {
                Console.WriteLine("The value does not exist in the database");
                return null;
            }

            User user = new User();
            user.Email = email;
            user.Password = password;
            
                        
            
            //kullanıcı var ise jwt token üretilir.
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                   new Claim(ClaimTypes.Email, user.Password)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);

            //Token değeri firebase'e eklenir 
            _firebaseAccess.AddToken(user);

            //şifre null olarak gönderilir.
            user.Password = null;

            return user;
        }

        

        //kullanıcı firebase'e ekleme yapılır.
        public void AddUser(string email, string password)
        {
            _firebaseAccess.AddUser(email, password);
        }

       
    }
}
