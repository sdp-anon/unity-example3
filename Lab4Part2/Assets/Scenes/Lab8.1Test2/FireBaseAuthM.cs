using UnityEngine;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;

[System.Serializable]
public partial class UserData
{
    public string username;
    public string email;

    public UserData(string username, string email)
    {
        this.username = username;
        this.email = email;
    }
}

public class FirebaseAuthM : MonoBehaviour
{
    FirebaseAuth auth;
    DatabaseReference dbRef;

    [Header("Login Fields")]
    public TMP_InputField loginEmail;
    public TMP_InputField loginPassword;

    [Header("Signup Fields")]
    public TMP_InputField signupUsername;
    public TMP_InputField signupEmail;
    public TMP_InputField signupPassword;

    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
    }

    // -------------------
    // SIGN UP
    // -------------------
    public void SignUp()
    {
        string email = signupEmail.text.Trim();
        string password = signupPassword.text.Trim();
        string username = signupUsername.text.Trim();

        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Signup Failed: " + task.Exception);
                return;
            }

            FirebaseUser newUser = task.Result.User;
            Debug.Log("Signup Success: " + newUser.Email);

            // Save username + email to Realtime Database
            UserData userData = new UserData(username, email);
            dbRef.Child("Users").Child(newUser.UserId)
                 .SetRawJsonValueAsync(JsonUtility.ToJson(userData))
                 .ContinueWithOnMainThread(dbTask =>
                 {
                     if (dbTask.IsCompleted)
                         Debug.Log("User data saved in DB!");
                     else
                         Debug.LogError("Failed to save user data: " + dbTask.Exception);
                 });
        });
    }

    // -------------------
    // LOGIN
    // -------------------
    public void Login()
    {
        string email = loginEmail.text.Trim();
        string password = loginPassword.text.Trim();

        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Login Failed: " + task.Exception);
                return;
            }

            FirebaseUser user = task.Result.User;
            Debug.Log("Login Success: " + user.Email);

            // Retrieve user data from Realtime Database
            dbRef.Child("Users").Child(user.UserId).GetValueAsync().ContinueWithOnMainThread(dbTask =>
            {
                if (dbTask.IsFaulted)
                {
                    Debug.LogError("Failed to retrieve user data: " + dbTask.Exception);
                    return;
                }

                DataSnapshot snapshot = dbTask.Result;
                string username = snapshot.Child("username").Value.ToString();
                string emailRetrieved = snapshot.Child("email").Value.ToString();

                Debug.Log("Username: " + username);
                Debug.Log("Email: " + emailRetrieved);
            });
        });
    }

    // -------------------
    // LOGOUT
    // -------------------
    public void Logout()
    {
        auth.SignOut();
        Debug.Log("Logged Out");
    }
}