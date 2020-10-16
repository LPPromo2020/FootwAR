using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateRoom : MonoBehaviour
{
    [SerializeField] private InputField m_ifNameRoom;
    [SerializeField] private InputField m_ifPassword;

    [SerializeField] private Toggle m_tPasswordUsed;

    public void Create()
    {
        if (m_ifPassword.text == "" || m_ifNameRoom.text == "")
        {
            Debug.LogError("Une des informations est vide");
            return;
        }
        
        string password = m_tPasswordUsed.isOn ? m_ifPassword.text : "";
        RoomManager.Instance.CreateRoom(m_ifNameRoom.text, password, 4);
    }
}
