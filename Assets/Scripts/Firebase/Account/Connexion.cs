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
            m_tErrorText.text = "Tous les champs ne sont pas renseignés";
        }
        else if (!checkIfIsEmail(m_ifEmail.text))
        {
            m_tErrorText.text = "Veuilez entrer une vraie adresse mail";
        }
        else if (m_ifPassword.text.Length < 6)
        {
            m_tErrorText.text = "Le mot de passe est invalide";
        }
        else
        {
            StartCoroutine(FireBaseManager.Instance.SignIn(m_ifEmail.text, m_ifPassword.text, authError => {
                switch (authError)
                {
                    case AuthError.UserNotFound:
                        m_tErrorText.text = "Aucun utilisateur ne correspond à cette adresse mail";
                        break;
                    case AuthError.WrongPassword:
                        m_tErrorText.text = "Le mot de passe est invalide";
                        break;
                    default:
                        break;
                }
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
