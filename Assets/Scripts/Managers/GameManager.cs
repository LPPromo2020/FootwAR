using System.Collections.Generic;
using UnityEngine;

//Dorian et Romuald
//Instancie l'ensemble des managers necessaires à l'initialisation du jeu

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private List<GameObject> m_listPrefabsLoader;

    void Start()
    {
        m_listPrefabsLoader.ForEach(el => Instantiate(el));
        m_listPrefabsLoader.Clear();
        m_listPrefabsLoader = null; // (Si fonctionne) Dit au C# de détruire la listes de prefabs

        SceneLoader.LoadScene("Connexion");
    }

 }
