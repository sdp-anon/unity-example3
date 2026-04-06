using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using TMPro;

public class FirebaseAuthManager : MonoBehaviour
{
    FirebaseAuth auth;

    [Header("Login Fields")]
    public TMP_InputField loginEmail;
    public TMP_InputField loginPassword;

    [Header("Signup Fields")]
    public TMP_InputField signupEmail;
    public TMP_InputField signupPassword;

    void Start()
    {
        InitializeFirebase();
    }

    void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var status = task.Result;

            if (status == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;
                Debug.Log("Firebase Initialized Successfully");
            }
            else
            {
                Debug.LogError("Firebase initialization failed: " + status);
            }
        });
    }

    // ---------------- SIGNUP ----------------
    public void SignUp()
    {
        string email = signupEmail.text.Trim();
        string password = signupPassword.text.Trim();

        if (email == "" || password == "")
        {
            Debug.Log("Email or Password empty");
            return;
        }

        auth.CreateUserWithEmailAndPasswordAsync(email, password)
        .ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("Signup Canceled");
                return;
            }

            if (task.IsFaulted)
            {
                Debug.LogError("Signup Error: " + task.Exception);
                return;
            }

            FirebaseUser newUser = task.Result.User;

            Debug.Log("Signup Success!");
            Debug.Log("User Email: " + newUser.Email);
        });
    }

    // ---------------- LOGIN ----------------
    public void Login()
    {
        string email = loginEmail.text.Trim();
        string password = loginPassword.text.Trim();

        if (email == "" || password == "")
        {
            Debug.Log("Email or Password empty");
            return;
        }

        auth.SignInWithEmailAndPasswordAsync(email, password)
        .ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("Login Canceled");
                return;
            }

            if (task.IsFaulted)
            {
                Debug.LogError("Login Failed: " + task.Exception);
                return;
            }

            FirebaseUser user = task.Result.User;

            Debug.Log("Login Success!");
            Debug.Log("Welcome: " + user.Email);
        });
    }

    // ---------------- LOGOUT ----------------
    public void Logout()
    {
        auth.SignOut();
        Debug.Log("User Logged Out");
    }
}