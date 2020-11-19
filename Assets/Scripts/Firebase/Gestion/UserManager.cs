using System;
using System.Collections;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine;

public class UserManager : Singleton<UserManager>
{
    private FirebaseUser m_fuUser;
    private string m_sConnectionNameFunction;

    public IEnumerator CreateAccount(string pseudo, string email, string password, Action<AuthError> callback = null) {
        Task<FirebaseUser> createUser = FireBaseManager.Instance.Auth.CreateUserWithEmailAndPasswordAsync(email, password);
        while (!createUser.IsCompleted) yield return null;
               
        m_fuUser = createUser.Result;

        Task updateUserPseudo = m_fuUser.UpdateUserProfileAsync(new UserProfile { DisplayName = pseudo });
        while (!updateUserPseudo.IsCompleted) yield return null;

        Task adduserInfo = FireBaseManager.Instance.Database.Child("players").Child(m_fuUser.UserId).SetRawJsonValueAsync("{\"nbDefeats\":0,\"nbVictorys\":0}");
        Debug.Log(m_fuUser.DisplayName);
    }
    
    public IEnumerator Connection(string email, string password, Action<bool, AuthError> callback)
    {
        Debug.Log(gameObject.name);

        if (email == "" || password == "")
        {
            Debug.LogWarning($"[{GetType()}] Une des information est vide");
            yield break;
        }
        
        Debug.Log($"[{GetType()}] email: {email}, password: {password}");
        
        AuthError error = AuthError.None;
        Task thread = FireBaseManager.Instance.Auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogWarning($"[{GetType()}] Annulation de la requete");
            }
            
            if (task.IsFaulted)
            {
                error = FireBaseManager.Instance.GetAuthError(task.Exception);
                return;
            }

            m_fuUser = task.Result;
        });

        while (!thread.IsCompleted) yield return null;

        callback(ConnectionEnd(error), error);
    }

    private bool ConnectionEnd(AuthError error)
    {
        if (error != AuthError.None)
        {
            switch (error)
            {
                case AuthError.WrongPassword:
                    Debug.LogError($"[{GetType()}] Mauvais mot de passe");
                    break;
                case AuthError.UserNotFound:
                    Debug.LogError($"[{GetType()}] Compte non trouvé");
                    break;
            }

            return false;
        }
        
        Debug.Log($"[{GetType()}] Connexion réussie");
        return true;
    }
    
    public void Disconnect()
    {
        m_fuUser = null;
        FireBaseManager.Instance.Auth.SignOut();
    }
}