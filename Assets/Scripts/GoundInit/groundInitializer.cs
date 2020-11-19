/*
 * Nom de l’auteur : Yannis Querenet
 * Principe du script : Le script sert à initialiser un sol en réalité augmentée
 * Utilisation : Le script doit être lancé après le démarrage d'une partie pour définir un terrain.
 * A mettre dans le ARSessionOrigin :
 * - "goPlanePrefab" correspond au prefab "groundVisualizer", 
 * - "mGroundColor" correspond au material utilisé.
*/


using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class groundInitializer : MonoBehaviour
{
    public ARRaycastManager m_rmRaycastManager; // Le Raycast Manager
    public ARPlaneManager m_pmPlaneManager; // Le Plane Manager
    public List<ARRaycastHit> m_lARRayCastHit; // Liste de point trouvés par le raycastmanager
    public GameObject m_goPlanePrefab; // Prefab du plan
    public Material m_mGroundColor; // Materiel de couleur du terrain

    // Start is called before the first frame update
    void Start()
    {
        m_rmRaycastManager = FindObjectOfType<ARRaycastManager>();
        m_pmPlaneManager = FindObjectOfType<ARPlaneManager>();
        m_lARRayCastHit = new List<ARRaycastHit>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (m_rmRaycastManager.Raycast(ray, m_lARRayCastHit, UnityEngine.XR.ARSubsystems.TrackableType.PlaneEstimated))
            {
                CreateGround();
                HidePlaneManager();
            }
        }

    }
    void HidePlaneManager() // Fonction cache les plans détectés, appelée après avoir posé le terrain.
    {
        foreach (ARPlane arPlane in GameObject.FindObjectsOfType<ARPlane>())
        {
            arPlane.GetComponent<MeshRenderer>().material.color = (new Color(0, 0, 0, 0));
        }
        m_goPlanePrefab.GetComponent<MeshRenderer>().material.color = (new Color(0, 0, 0, 0));

    }

    void CreateGround() // Fonction qui permet de créer le terrain.
    {
        GameObject gameGround = GameObject.CreatePrimitive(PrimitiveType.Cube);
        gameGround.transform.position = m_lARRayCastHit[0].pose.position;
        gameGround.transform.localScale = new Vector3(3f + (Random.value * 2), 0.01f, 5f + (Random.value * 2));
        gameGround.GetComponent<MeshRenderer>().material = m_mGroundColor;

    }
}