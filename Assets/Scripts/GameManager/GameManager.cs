using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private List<GameObject> m_listPrefabsLoader;

    void Start()
    {
        m_listPrefabsLoader.ForEach(el => Instantiate(el));
        m_listPrefabsLoader.Clear();
        m_listPrefabsLoader = null;//potentiel source d'erreur

        SceneLoader.LoadScene("Connexion");
    }

 }
