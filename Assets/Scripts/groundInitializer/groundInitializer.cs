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
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class groundInitializer : MonoBehaviour
{
    public ARRaycastManager m_rmRaycastManager; // Le Raycast Manager
    public ARPlaneManager m_pmPlaneManager; // Le Plane Manager
    public List<ARRaycastHit> m_lARRayCastHit; // Liste de point trouvés par le raycastmanager
    public GameObject m_goPlanePrefab; // Prefab du plan
    public Material m_mGroundColor; // Materiel de couleur du terrain
    public GameObject m_stade; // Prefab du stade
    public Text debug;
    private bool m_isAlreadyGroundExist = false;
    private const float CUBE_X = 1.5f;
    private const float CUBE_Y = 0.01f;
    private const float CUBE_Z = 0.8f;
    private const float RANDOM_SIDE_VALUE = 0.2f;

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
        
        if (Input.GetMouseButtonDown(0) && !m_isAlreadyGroundExist)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (m_rmRaycastManager.Raycast(ray, m_lARRayCastHit, UnityEngine.XR.ARSubsystems.TrackableType.PlaneEstimated))
            {
                CreateRandomGround();
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
        GameObject gameGround = Instantiate(m_stade);
        gameGround.transform.position = m_lARRayCastHit[0].pose.position;
        gameGround.transform.localScale = new Vector3(6f,6f,6f);
        m_isAlreadyGroundExist = true;
    }
    void CreateRandomGround()
    {
    float x;
    float y;
    float z;

    GameObject randomGameGround = new GameObject();
        randomGameGround.transform.position = m_lARRayCastHit[0].pose.position;
        randomGameGround.AddComponent<LineRenderer>();
        randomGameGround.GetComponent<LineRenderer>().positionCount = 4;
        randomGameGround.GetComponent<LineRenderer>().loop = true;
        randomGameGround.GetComponent<LineRenderer>().alignment = LineAlignment.TransformZ;
        x = m_lARRayCastHit[0].pose.position.x + Random.Range(-RANDOM_SIDE_VALUE, RANDOM_SIDE_VALUE);
        y = CUBE_Y;
        z = m_lARRayCastHit[0].pose.position.z + Random.Range(-RANDOM_SIDE_VALUE, RANDOM_SIDE_VALUE);
        randomGameGround.GetComponent<LineRenderer>().SetPosition(0, new Vector3(x + CUBE_X, y, z + CUBE_Z));
        randomGameGround.GetComponent<LineRenderer>().SetPosition(1, new Vector3(x + CUBE_X, y, z - CUBE_Z));
        randomGameGround.GetComponent<LineRenderer>().SetPosition(2, new Vector3(x - CUBE_X, y, z - CUBE_Z));
        randomGameGround.GetComponent<LineRenderer>().SetPosition(3, new Vector3(x - CUBE_X, y, z + CUBE_Z));
        randomGameGround.GetComponent<LineRenderer>().material = m_mGroundColor;
        debug.text = randomGameGround.GetComponent<LineRenderer>().GetPosition(0).x.ToString(); 
        // TODO tester pourquoi le setposition ne marche pas : tester coordonnées points
    }
}