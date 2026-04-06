using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;

public class FBCode : MonoBehaviour
{
    private FirebaseAuth auth;
    private DatabaseReference dbRef;
    private bool firebaseReady = false;

    [Header("Login Fields")]
    public TMP_InputField loginEmail;
    public TMP_InputField loginPassword;

    [Header("Signup Fields")]
    public TMP_InputField signupUsername;
    public TMP_InputField signupEmail;
    public TMP_InputField signupPassword;

    void Awake()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;

                dbRef = FirebaseDatabase
                    .GetInstance("https://test-780bb-default-rtdb.firebaseio.com/")
                    .RootReference;

                firebaseReady = true;
                Debug.Log("Firebase READY ✅");
            }
            else
            {
                Debug.LogError("Firebase NOT ready ❌: " + task.Result);
            }
        });
    }

    // -------------------
    // SIGN UP
    // -------------------
    public void SignUp()
    {
        if (!firebaseReady)
        {
            Debug.LogWarning("Firebase is not ready yet!");
            return;
        }

        if (signupUsername == null || signupEmail == null || signupPassword == null)
        {
            Debug.LogError("Signup fields not assigned!");
            return;
        }

        string email = signupEmail.text.Trim();
        string password = signupPassword.text.Trim();
        string username = signupUsername.text.Trim();

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(username))
        {
            Debug.LogWarning("Fill all signup fields!");
            return;
        }

        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Signup Failed: " + task.Exception);
                return;
            }

            FirebaseUser newUser = task.Result.User;
            Debug.Log("Signup Success: " + newUser.Email);

            // Save data
            UserData2 userData = new UserData2(username, email);

            dbRef.Child("Users").Child(newUser.UserId)
                .SetRawJsonValueAsync(JsonUtility.ToJson(userData))
                .ContinueWithOnMainThread(dbTask =>
                {
                    if (dbTask.IsCompleted)
                        Debug.Log("User data saved in DB ✅");
                    else
                        Debug.LogError("DB Save Failed: " + dbTask.Exception);
                });
        });
    }

    // -------------------
    // LOGIN
    // -------------------
    public void Login()
    {
        if (!firebaseReady)
        {
            Debug.LogWarning("Firebase is not ready yet!");
            return;
        }

        if (loginEmail == null || loginPassword == null)
        {
            Debug.LogError("Login fields not assigned!");
            return;
        }

        string email = loginEmail.text.Trim();
        string password = loginPassword.text.Trim();

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            Debug.LogWarning("Fill all login fields!");
            return;
        }

        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Login Failed: " + task.Exception);
                return;
            }

            FirebaseUser user = task.Result.User;
            Debug.Log("Login Success: " + user.Email);

            // Retrieve data
            dbRef.Child("Users").Child(user.UserId).GetValueAsync()
                .ContinueWithOnMainThread(dbTask =>
                {
                    if (dbTask.IsFaulted)
                    {
                        Debug.LogError("DB Read Failed: " + dbTask.Exception);
                        return;
                    }

                    DataSnapshot snapshot = dbTask.Result;

                    if (snapshot.Exists)
                    {
                        string username = snapshot.Child("username").Value.ToString();
                        string emailRetrieved = snapshot.Child("email").Value.ToString();

                        Debug.Log("Username: " + username);
                        Debug.Log("Email: " + emailRetrieved);
                    }
                    else
                    {
                        Debug.LogWarning("No user data found.");
                    }
                });
        });
    }

    // -------------------
    // LOGOUT
    // -------------------
    public void Logout()
    {
        if (!firebaseReady)
        {
            Debug.LogWarning("Firebase is not ready yet!");
            return;
        }

        auth.SignOut();
        Debug.Log("Logged Out");
    }
}