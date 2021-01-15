using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/* Auteur : Dorian
    Permet de controler tout les sons du jeu
*/


public class AudioManager : Singleton<AudioManager>
{
    public float volume = 0.5f;
    public List<AudioClip> m_audioClipsList;
    public AudioSource m_asMainMenu;
    [SerializeField]
    private AudioClip m_audioClipMusicToPlay;
    [SerializeField]
    private AudioClip m_audioClipMusicInGame;
    private bool m_boolIsPlaying;

    private void Start()
    {
        m_asMainMenu = GetComponent<AudioSource>();
        m_asMainMenu.PlayOneShot(m_audioClipMusicToPlay);
    }

    private void Update()
    {
        if (m_asMainMenu.isPlaying == false)
        {
            if (SceneManager.GetActiveScene().name == "InGameScene")
            {
                m_asMainMenu.PlayOneShot(m_audioClipMusicToPlay);
            }
            else
            {
                m_asMainMenu.PlayOneShot(m_audioClipMusicToPlay);
            }

        }
    }
}
