using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Firebase;
using Firebase.Auth;
using Firebase.Database;

using System.Threading.Tasks;

/**
    MARC, JULIE, LOUIS, ROMUALD
 */

public class FireBaseManager : MonoBehaviour
{
    private FirebaseAuth m_faAuth;
    private FirebaseUser m_fuUser;
    private DatabaseReference m_drRootReference;
    private User m_uUser;

    public enum GameStatue { VICTORY = 1, DEFEAT = 0 };

    private void Start()
    {
        FirebaseApp.DefaultInstance.Options.DatabaseUrl = new System.Uri("BASE DE DONNEE");

        m_faAuth = FirebaseAuth.DefaultInstance;
        m_drRootReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    private void SignUp(string email, string password)
    {
        m_faAuth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
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

            m_fuUser = task.Result;

            m_drRootReference.Child("players").Child(m_fuUser.UserId).SetRawJsonValueAsync("{nbDefeats:0, nbVictorys:0}").ContinueWith(task2 => {
                if (task2.IsCanceled || task2.IsFaulted)
                {
                    Debug.LogError(task.Exception);
                    return;
                }

                Debug.Log("[FireBaseManager] Création du compte effectué et de ces données");

                CreateUser();
            });
        });
    }

    private void SignIn(string email, string password)
    {
        m_faAuth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
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

            m_fuUser = task.Result;

            // Fonction de connection

            CreateUser();
        });
    }

    private void CreateUser()
    {
        m_drRootReference.Child("players").Child(m_fuUser.UserId).GetValueAsync().ContinueWith(task => {
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

            DataSnapshot result = task.Result;

            m_uUser = new User(
                m_fuUser.UserId,
                m_fuUser.DisplayName,
                m_fuUser.Email,
                (int) result.Child("nbDefeats").Value,
                (int) result.Child("nbVictorys").Value
            );

            Debug.Log($"[FireBaseManager] {m_uUser.PSEUDO()}: v -> {m_uUser.nbVictorys}, d -> {m_uUser.nbDefeats}");
        }); 
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
}
