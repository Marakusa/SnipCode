using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;

namespace SnipCode.Database
{
    public class LogInFirebase
    {
        private FirebaseConfig firebaseConfig = new FirebaseConfig
        {
            AuthSecret = "ZS6S9QXm9nDfoK8eIxnEos5XrxB8SsESbq0TkZyl",
            BasePath = "https://snip-code-default-rtdb.europe-west1.firebasedatabase.app/"
        };
        private FirebaseClient firebaseClient;

        public LogInFirebase()
        {
            firebaseClient = new FirebaseClient(firebaseConfig);

            if (firebaseClient != null)
            {
            }
        }

        private void CreateUser(string email, string password)
        {
            firebaseClient.CreateUser(email, password);

        }
    }
}
