using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class switchInputField : MonoBehaviour
{
    [SerializeField] InputField m_ifPassword;

    public void switchField()
    {
        m_ifPassword.interactable = !m_ifPassword.interactable;
    }
}
