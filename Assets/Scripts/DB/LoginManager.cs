using UnityEngine;
using System.Threading.Tasks;

using Firebase.Auth;

namespace Poly.DB
{
    public class LoginManager : MonoBehaviour
    {
        private FirebaseAuth auth;
        private FirebaseUser user;

        private bool isLoggedIn;

        // get, set
        public FirebaseUser User    { get { return user;       } } // read only
        public bool IsLoggedIn      { get { return isLoggedIn; } } // read only

        public async Task SignIn(string email, string password)
        {
            if (isLoggedIn) { return; }

            await auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                    return;
                }

                user = task.Result;
                if(user.IsEmailVerified)
                {
                    isLoggedIn = true;
                    Debug.LogFormat("User signed in successfully: {0} ({1})", user.DisplayName, user.UserId);
                }
                else
                {
                    Debug.LogError("Email not verified.");
                }
            });
        }

        public void SignOut()
        {
            if (!isLoggedIn) { return; }

            Debug.LogFormat("Bye, {0}.", user.DisplayName);
            auth.SignOut();
            user = null;
            isLoggedIn = false;
        }

        // MonoBehaviour
        private void Awake()
        {
            // singleton
            var objs = FindObjectsOfType<LoginManager>();
            if (objs.Length > 1)
            {
                Destroy(gameObject);
            }
            else
            {
                DontDestroyOnLoad(gameObject);
            }

            // init
            auth = FirebaseAuth.DefaultInstance;
            user = null;
            isLoggedIn = false;
        }

        private void OnApplicationQuit()
        {
            SignOut();
        }
    }
}