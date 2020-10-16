using Firebase.Auth;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

/**
    MARC, JULIE, LOUIS, ROMUALD
 */


public class inscription : MonoBehaviour
{
    [SerializeField] InputField m_ifPseudo;
    [SerializeField] InputField m_ifEmail;
    [SerializeField] InputField m_ifPassword;
    [SerializeField] InputField m_ifPasswordVerif;
    [SerializeField] Text m_tErrorText;
    public void playerInscription()
    {
        m_tErrorText.text = "";
        if (m_ifPseudo.text == "" || m_ifEmail.text == "" || m_ifPassword.text == "" || m_ifPasswordVerif.text == "")
        {
            m_tErrorText.text = "Tous les champs ne sont pas renseignés";
        }
        else if (!checkIfIsEmail(m_ifEmail.text))
        {
            m_tErrorText.text = "Veuilez entrer une vraie adresse mail";
        }
        else if(m_ifPassword.text != m_ifPasswordVerif.text)
        {
            m_tErrorText.text = "Les mots de passe ne sont pas les mêmes";
        }
        else if(m_ifPseudo.text.Length < 3)
        {
            m_tErrorText.text = "Le pseudo doit faire au minimum 3 caractères";
        }
        else if (m_ifPassword.text.Length < 6)
        {
            m_tErrorText.text = "Le mot de passe doit faire au minimum 6 caractères";
        }
        else
        {
            StartCoroutine(FireBaseManager.Instance.SignUp(m_ifPseudo.text, m_ifEmail.text, m_ifPassword.text, authError => {
                switch (authError)
                {
                    case AuthError.EmailAlreadyInUse:
                        m_tErrorText.text = "Un compte utilisant cette adresse mail existe déjà";
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
