using LoginProjectApp.DataAccess.abstracts;
using LoginProjectApp.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Cloud.Firestore;


namespace LoginProjectApp.DataAccess.concretes
{
    public class FirebaseAccess : IUserDao
    {

        public static void FirebaseConnect()
        {
            //veritabanına bağlanma 
            string path = AppDomain.CurrentDomain.BaseDirectory + @"loginproject-319714-firebase-adminsdk-cnmg6-207ae21fd8.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);

        }

        public void AddUser(string email, string password)
        {
            FirebaseConnect();

            FirestoreDb db = FirestoreDb.Create("loginproject-319714");

            if (db != null)
            {
                Console.WriteLine("bağlantı başarılı");
            }
            else
            {
                Console.WriteLine("bağlantı başarısız");
            }

            
            //firestor veri ekleme yapılır. Id ekler
            DocumentReference docRef = db.Collection("login").Document();
            Dictionary<string, object> user = new Dictionary<string, object>
             {
                 {"Email",email },
                 {"Password",password },
                 {"Token","" }
             };
            docRef.SetAsync(user);

            string deneme = docRef.Id;

            //Console.WriteLine("id değeri: " + deneme);
        }

        public async Task<bool> CheckEmailAndPassword(string email, string password)
        {
            bool result = false;

            FirebaseConnect();

            FirestoreDb db = FirestoreDb.Create("loginproject-319714");

            if (db != null)
            {
                Console.WriteLine("bağlantı başarılı");
            }
            else
            {
                Console.WriteLine("bağlantı başarısız");
            }


            //Where işlemi ile email ve password için filtreleme yapar.
            CollectionReference usersRef = db.Collection("login");
            Query query = usersRef
                .WhereEqualTo("Email", email)            
                .WhereEqualTo("Password", password);      
            
            QuerySnapshot querySnapshot = await query.GetSnapshotAsync();


            if (querySnapshot.Count == 1)
            {
                //true döner ise token oluşacak ve frontende geri gönderilecek
                result = true;
            }
            else
            {
                result = false;
            }

            return result;
        }

        public async void AddToken(User user)
        {
            //gönderilen email ve password değerlerine bakarak veritabanında idsini alır ve o idye göre token değerini günceller.
            FirebaseConnect();

            FirestoreDb db = FirestoreDb.Create("loginproject-319714");

            if (db != null)
            {
                Console.WriteLine("bağlantı başarılı");
            }
            else
            {
                Console.WriteLine("bağlantı başarısız");
            }


            CollectionReference usersRef = db.Collection("login");
            Query query = usersRef
                .WhereEqualTo("Email", user.Email)             
                .WhereEqualTo("Password", user.Password);      

            QuerySnapshot querySnapshot = await query.GetSnapshotAsync();

            DocumentSnapshot documentSnapshot = querySnapshot.Documents[0];

            //id değeri yazdırılır.
            //Console.WriteLine("id değeri : " + documentSnapshot.Id);

            DocumentReference docref = db.Collection("login").Document(documentSnapshot.Id);
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"Token", user.Token }
            };

            DocumentSnapshot snap = await docref.GetSnapshotAsync();
            if (snap.Exists)
            {
                await docref.UpdateAsync(data);
            }


        }

        public async Task<bool> CheckEmail(string email)
        {
            FirebaseConnect();

            FirestoreDb db = FirestoreDb.Create("loginproject-319714");

            if (db != null)
            {
                Console.WriteLine("bağlantı başarılı");
            }
            else
            {
                Console.WriteLine("bağlantı başarısız");
            }

            CollectionReference usersRef = db.Collection("login");
            Query query = usersRef
                .WhereEqualTo("Email", email);

            QuerySnapshot querySnapshot = await query.GetSnapshotAsync();

            if(querySnapshot.Count == 1)
            {
                //veritabanında vardır.
                Console.WriteLine("email değeri veritabanında vardır");
                return true;
            }
            else
            {
                //veritabanında yoktur.
                Console.WriteLine("email değeri veritabanında yoktur");
                return false;
            }

        }

    }
}
