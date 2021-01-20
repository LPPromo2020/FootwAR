using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Cléry Chassagnon
//Pas vraiment utilise seulement un script de déplacement du joueur
//Fonction update -> déplacement
//Fonction SpeedChange et JumpForceChange sont liées à l'UI afin de changer les variables de
//déplacement du joueur in game
public class PlayerController : MonoBehaviour
{
    public float m_fSpeed = 0.2f;
    public float m_fJumpForce = 1f;
    public GameObject m_GOUiBallController;
    public GameObject m_GOSpeedSlider;
    public GameObject m_GOJumpSlider;
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Z))
        {
            GetComponent<Transform>().position += new Vector3(0, 0, m_fSpeed);
        }
        if (Input.GetKey(KeyCode.S))
        {
            GetComponent<Transform>().position += new Vector3(0, 0, -m_fSpeed);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            GetComponent<Transform>().position += new Vector3(-m_fSpeed, 0, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            GetComponent<Transform>().position += new Vector3(m_fSpeed, 0, 0);
        }
        if (Input.GetKeyDown(KeyCode.Space) && GetComponent<Transform>().position.y <= 1.5)
        {
            GetComponent<Transform>().position += new Vector3(0, m_fJumpForce, 0);
        }
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Return))
        {
            if (m_GOUiBallController.active)
            {
                m_GOUiBallController.SetActive(false);
            }
            else
            {
                m_GOUiBallController.SetActive(true);
            }
        }
    }

    public void SpeedChange()
    {
        float x = m_GOSpeedSlider.GetComponent<Slider>().value;     
        m_fSpeed = x;

        m_GOSpeedSlider.transform.GetChild(4).GetComponent<Text>().text = m_fSpeed.ToString();
    }

    public void JumpForceChange()
    {
        float x = m_GOJumpSlider.GetComponent<Slider>().value;
        m_fJumpForce = x;

        m_GOJumpSlider.transform.GetChild(4).GetComponent<Text>().text = m_fJumpForce.ToString();
    }
}
