using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Firebase;
using Firebase.Auth;
using Firebase.Database;

using System.Threading.Tasks;
using System;

/**
    MARC, JULIE, LOUIS, ROMUALD
 */

public class FireBaseManager : Singleton<FireBaseManager>
{
    private FirebaseAuth m_faAuth;
    private FirebaseUser m_fuUser;
    private DatabaseReference m_drRootReference;
    private User m_uUser;

    public enum GameStatue { VICTORY = 1, DEFEAT = 0 };

    private void Start()
    {
        FirebaseApp.DefaultInstance.Options.DatabaseUrl = new System.Uri("https://footwar-c0754.firebaseio.com/");

        m_faAuth = FirebaseAuth.DefaultInstance;
        m_drRootReference = FirebaseDatabase.DefaultInstance.RootReference;

    }

    public IEnumerator SignUp(string pseudo, string email, string password, Action<AuthError> callback)
    {
        AuthError error = 0;
        Task newtask = m_faAuth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled || task.IsFaulted)
            {
                // Canceled
                Debug.LogError(task.Exception);
                if(error == 0) error = getError(task.Exception);
                return;
            }

            m_fuUser = task.Result;

            Firebase.Auth.UserProfile profile = new Firebase.Auth.UserProfile
            {
                DisplayName = pseudo
            };
            m_fuUser.UpdateUserProfileAsync(profile).ContinueWith(task3 => {
                if (task3.IsCanceled)
                {
                    Debug.LogError("UpdateUserProfileAsync was canceled.");
                    if (error == 0) error = getError(task3.Exception);
                    return;
                }
                if (task3.IsFaulted)
                {
                    Debug.LogError("UpdateUserProfileAsync encountered an error: " + task3.Exception);
                    if (error == 0) error = getError(task3.Exception);
                    return;
                }

                Debug.Log("User profile updated successfully.");
            });


            m_uUser = new User(
                "",
                "",
                "",
                0,
                0);
            string json = JsonUtility.ToJson(m_uUser);
            Debug.Log(json);
            m_drRootReference.Child("players").Child(m_fuUser.UserId).SetRawJsonValueAsync(json).ContinueWith(task2 => {
                if (task2.IsCanceled || task2.IsFaulted)
                {
                    Debug.LogError(task2.Exception);
                    return;
                }

                Debug.Log("[FireBaseManager] Création du compte effectué et de ces données");
            });
        });
        while (!newtask.IsCompleted) yield return null;
        if (error == 0) StartCoroutine(CreateUser());
        callback(error);

    }

    public IEnumerator SignIn(string email, string password, Action<AuthError> callback)
    {
        AuthError error = 0;
        Task newtask = m_faAuth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled)
            {
                // Canceled
                return;
            }

            if (task.IsFaulted)
            {
                Debug.LogError(task.Exception);
                if (error == 0) error = getError(task.Exception);
                return;
            }

            m_fuUser = task.Result;
            
        });
        while (!newtask.IsCompleted) yield return null;
        if(error == 0) StartCoroutine(CreateUser());
        callback(error);
    }

    private IEnumerator CreateUser()
    {
        DataSnapshot result = null;
           Task newtask = m_drRootReference.Child("players").Child(m_fuUser.UserId).GetValueAsync().ContinueWith(task => {
            if (task.IsCanceled)
            {
                // Canceled
                Debug.LogError("CreateUser : Canceled");
                return;
            }
            
            if (task.IsFaulted)
            {
                Debug.LogError(task.Exception);
                return;
            }

            result = task.Result;

           });
        while (!newtask.IsCompleted) yield return null;
        m_uUser = new User(
            m_fuUser.UserId,
            m_fuUser.DisplayName,
            m_fuUser.Email,
            int.Parse(result.Child("nbDefeats").Value.ToString()),
            int.Parse(result.Child("nbVictorys").Value.ToString())
        );

        Debug.Log($"[FireBaseManager] {m_uUser.PSEUDO()}: v -> {m_uUser.nbVictorys}, d -> {m_uUser.nbDefeats}");
    }

    private void SaveUserInformation(GameStatue gameStatue)
    {
        string s;
        int score;

        if (gameStatue == GameStatue.VICTORY)
        {
            m_uUser.nbVictorys++;
            score = m_uUser.nbVictorys;
            s = "nbVictorys";
        }
        else
        {
            m_uUser.nbDefeats++;
            score = m_uUser.nbDefeats;
            s = "nbDefeats";
        }

        m_drRootReference.Child("players").Child(m_uUser.ID()).Child(s).SetValueAsync(score).ContinueWith(task => {
            if (task.IsCanceled)
            {
                // Canceled
                return;
            }
            
            if (task.IsFaulted)
            {
                Debug.LogError(task.Exception);
                return;
            }

            Debug.Log($"[FireBaseManager] Player informations is saved: v -> {m_uUser.nbVictorys}, d -> {m_uUser.nbDefeats}");
        });
    }

    private void CreateRoom()
    {
    }

    private AuthError getError(AggregateException error)
    {
        foreach (Exception exception in error.Flatten().InnerExceptions)
        {
            Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;
            if (firebaseEx != null)
            {
                return (Firebase.Auth.AuthError)firebaseEx.ErrorCode;
            }
        }
        return 0;
    }
}
