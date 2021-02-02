using Firebase.Auth;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

/**
    MARC, JULIE, LOUIS, ROMUALD
 */
public class Connexion : MonoBehaviour
{
    [SerializeField] InputField m_ifEmail;
    [SerializeField] InputField m_ifPassword;
    [SerializeField] Text m_tErrorText;
    public void playerConnexion()
    {
        m_tErrorText.text = "";
        if (m_ifEmail.text == "" || m_ifPassword.text == "")
        {
            NotificationsManager.Instance.AddNotification("Attention", "Tous les champs ne sont pas renseignés", 2);
        }
        else if (!checkIfIsEmail(m_ifEmail.text))
        {
            NotificationsManager.Instance.AddNotification("Attention", "Veuilez entrer une adresse mail valide", 2);
        }
        else if (m_ifPassword.text.Length < 6)
        {
            NotificationsManager.Instance.AddNotification("Attention", "Mot de passe de moins de 6 caractéres", 2);
        }
        else
        {            
            StartCoroutine(UserManager.Instance.Connection(m_ifEmail.text, m_ifPassword.text, (connect, authError) =>
            {
                switch (authError)
                {
                    case AuthError.UserNotFound:
                        NotificationsManager.Instance.AddNotification("Erreur", "Aucun utilisateur avec cette email n'existe", 2);
                        break;
                    case AuthError.WrongPassword:
                        NotificationsManager.Instance.AddNotification("Erreur", "Mot de passe invalide", 2);
                        break;
                    case AuthError.Failure:
                        NotificationsManager.Instance.AddNotification("Erreur", "Erreur dans la connexion du compte", 2);
                        break;
                }

                if (connect) SceneLoader.LoadScene("MainMenu");               
            }));            
        }
    }

    bool checkIfIsEmail(string email)
    {
        Regex regex = new Regex(@"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*"
+ "@"
+ @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$");
        Match match = regex.Match(email);
        return match.Success;
    }
}
