using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using LoginProjectApp.ViewModel;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Google.Cloud.Firestore;


namespace LoginProjectApp.Controllers
{
    [ApiController] //web api isteklerine cevap verir
    [Route("/login")] 
    public class AuthorizationController : ControllerBase
    {
        bool result;

        [AllowAnonymous]
        [HttpGet]
        public async Task<bool> LoginAsync()
        {
            
             
            Login login = new Login();
            login.Email = "samilakpinar6@gmail.com";
            login.Password = "123456";

            Login[] logins = { login };


            //veritabanına bağlanma 
            string path = AppDomain.CurrentDomain.BaseDirectory + @"loginproject-319714-firebase-adminsdk-cnmg6-207ae21fd8.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);

            FirestoreDb db = FirestoreDb.Create("loginproject-319714");

            if (db != null)
            {
                Console.WriteLine("bağlantı başarılı");
            }
            else
            {
                Console.WriteLine("bağlantı başarısız");
            }

            DocumentReference docRef = db.Collection("login").Document("user");
            Dictionary<string, object> user = new Dictionary<string, object>
            {
                {"Email",login.Email },
                {"Password",login.Password }
            };
             docRef.SetAsync(user);


            //veri tabanını okuma
            CollectionReference usersRef = db.Collection("login");
            QuerySnapshot snapshot = await usersRef.GetSnapshotAsync();
            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                Console.WriteLine("User: {0}", document.Id);
                Dictionary<string, object> documentDictionary = document.ToDictionary();
                Console.WriteLine("Email: {0}", documentDictionary["Email"]);
                Console.WriteLine("Password: {0}", documentDictionary["Password"]);
                
                if(documentDictionary["Email"].Equals("samilakpinar6@gmail.com") && documentDictionary["Password"].Equals("123456"))
                {
                    Console.WriteLine("var");
                    result = true;
                }
                else
                {
                    Console.WriteLine("yok");
                    result = false;
                }
         
               
            }

            return result;

        }


    }
}
