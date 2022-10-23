using UnityEngine;
using System.Threading.Tasks;

using Firebase.Auth;

namespace Poly.DB
{
    public class LoginManager : MonoBehaviour
    {
        private FirebaseAuth auth;

        private bool isLoggedIn;

        // get, set
        /// <summary>
        /// current Firebase user <br/><br/>
        /// <para>
        /// [CAUTION] <br/>
        /// current user (isLoggedIn == true) <br/>
        /// null (isLoggedIn == false) <br/>
        /// </para>
        /// </summary>
        public FirebaseUser User { get { return (isLoggedIn) ? auth.CurrentUser : null; } } // read only
        public bool IsLoggedIn   { get { return isLoggedIn; } } // read only

        /// <summary>
        /// log in <br/><br/>
        /// <para>
        /// return = <br/>
        /// verified user (isLoggedIn == true) <br/>
        /// unverified user (!= null) (isLoggedIn == false) <br/>
        /// null (isLoggedIn == false) <br/>
        /// </para>
        /// </summary>
        public async Task<FirebaseUser> LogIn(string email, string password)
        {
            if (isLoggedIn) { return auth.CurrentUser; }

            auth.SignOut(); // ensure auth.CurrentUser == null
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

                FirebaseUser newUser = task.Result;
                if (newUser.IsEmailVerified)
                {
                    isLoggedIn = true;
                    Debug.LogFormat("User signed in successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);
                }
                else
                {
                    Debug.LogError("Email not verified.");
                }
            });

            return auth.CurrentUser;
        }

        public void LogOut()
        {
            if (!isLoggedIn) { return; }

            Debug.LogFormat("Bye, {0}.", auth.CurrentUser.DisplayName);
            auth.SignOut();
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
            isLoggedIn = false;
        }

        private void OnApplicationQuit()
        {
            LogOut();
        }
    }
}