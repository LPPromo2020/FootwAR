using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/* Auteur : Dorian
    Permet de controler tout les sons du jeu
*/


public class AudioManager : Singleton<AudioManager>
{
    public AudioSource m_asMainMenu;
    [SerializeField]
    private AudioClip m_audioClipMusicToPlay;
    private void Start()
    {
        m_asMainMenu = GetComponent<AudioSource>();
        m_asMainMenu.Play();
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "InGameScene" )
        {
            m_asMainMenu.Stop();
        }
    }
}
