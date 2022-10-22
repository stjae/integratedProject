using UnityEngine;
using System.Threading.Tasks;

using Firebase.Auth;

namespace Poly.DB
{
    public class UserManagement
    {
        /// <summary>
        /// create user with email and password <br/><br/>
        /// <para>
        /// return = <br/>
        /// FirebaseUser instance (success) <br/>
        /// null (failed) <br/>
        /// </para>
        /// </summary>
        public static async Task<FirebaseUser> CreateUser(string email, string password)
        {
            if(string.IsNullOrEmpty(email))    { Debug.LogError("An email address must be provided."); return null; }
            if(string.IsNullOrEmpty(password)) { Debug.LogError("A password must be provided.");       return null; }

            FirebaseAuth auth = FirebaseAuth.DefaultInstance;
            FirebaseUser newUser = null;

            await auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                    return;
                }

                // Firebase user has been created.
                newUser = task.Result;
                Debug.LogFormat("newUser == auth.currUser : {0}", newUser == auth.CurrentUser);
                Debug.LogFormat("Firebase user created successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);
            });

            return newUser;
        }

        public static async Task UpdateUserProfile(FirebaseUser user, UserProfile profile)
        {
            if (user == null || profile == null) { return; }

            await user.UpdateUserProfileAsync(profile).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("UpdateUserProfileAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("UpdateUserProfileAsync encountered an error: " + task.Exception);
                    return;
                }

                Debug.Log("User profile updated successfully.");
            });
        }

        public static async Task SendEmailVerification(FirebaseUser user)
        {
            if (user != null)
            {
                await user.SendEmailVerificationAsync().ContinueWith(task =>
                {
                    if (task.IsCanceled)
                    {
                        Debug.LogError("SendEmailVerificationAsync was canceled.");
                        return;
                    }
                    if (task.IsFaulted)
                    {
                        Debug.LogError("SendEmailVerificationAsync encountered an error: " + task.Exception);
                        return;
                    }

                    Debug.Log("Email sent successfully.");
                });
            }
        }

        /// <summary>
        /// check if the given email is already in use <br/><br/>
        /// <para>
        /// return = <br/>
        /// true (existing email) <br/>
        /// false (new or empty or invalid email) <br/>
        /// </para>
        /// </summary>
        public static async Task<bool> CheckIfEmailExists(string email)
        {
            if (string.IsNullOrEmpty(email)) { Debug.LogError("An email address must be provided."); return false; }

            // FetchProvidersForEmailAsync() crashes with badly formatted email address.
            const string emailRegex = "^([0-9a-zA-Z]+)@([0-9a-zA-Z]+)(\\.[0-9a-zA-Z]+){1,}$";
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(emailRegex);
            if(!regex.IsMatch(email)) { Debug.LogError("Invalid email"); return false; }

            FirebaseAuth auth = FirebaseAuth.DefaultInstance;
            bool isInUse = false;

            await auth.FetchProvidersForEmailAsync(email).ContinueWith((authTask) =>
            {
                if (authTask.IsCanceled)
                {
                    Debug.Log("Provider fetch canceled.");
                }
                else if (authTask.IsFaulted)
                {
                    Debug.Log("Provider fetch encountered an error.");
                    Debug.Log(authTask.Exception.ToString());
                }
                else if (authTask.IsCompleted)
                {
                    foreach (string provider in authTask.Result)
                    {
                        isInUse = true;
                        return;
                    }
                }
            });

            return isInUse;
        }
    }
}