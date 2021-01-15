/*
 * Nom de l’auteur : Yannis Querenet
 * Principe du script : Le script sert à initialiser un sol en réalité augmentée
 * Utilisation : Le script doit être lancé après le démarrage d'une partie pour définir un terrain.
 * A mettre dans le ARSessionOrigin :
 * - "goPlanePrefab" correspond au prefab "groundVisualizer", 
 * - "mGroundColor" correspond au material utilisé.
*/

using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System.Collections;

public class groundInitializer : MonoBehaviour
{
    private ARRaycastManager m_rmRaycastManager; // Le Raycast Manager
    private ARPlaneManager m_pmPlaneManager; // Le Plane Manager
    public List<ARRaycastHit> m_lARRayCastHit; // Liste de point trouvés par le raycastmanager
    public GameObject m_goPlanePrefab; // Prefab du plan
    public Material m_mGroundColor; // Materiel de couleur du terrain
    public GameObject m_stade; // Prefab du stade
    public Text testDebug;
    private bool m_isAlreadyGroundExist = false;
    private const float CUBE_X = 1.5f;
    private const float CUBE_Y = 0.01f;
    private const float CUBE_Z = 0.8f;
    private const float RANDOM_SIDE_VALUE = 0.2f;
    public ARTrackedImageManager m_timImageManager; // Le Tracked Image Manager
    private List<Vector3> m_lImagePositionList;
    private int m_paperCounter = 0;
    private int count = 0;

    // Start is called before the first frame update
    void Start()
    {
        m_rmRaycastManager = FindObjectOfType<ARRaycastManager>();
        m_pmPlaneManager = FindObjectOfType<ARPlaneManager>();
        m_lARRayCastHit = new List<ARRaycastHit>();
        m_lImagePositionList = new List<Vector3>();
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
    /*Fonction permettant de créer un sol avec des un terrain random
    * => Actuellement non fonctionnel. En cours.
    */
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
        testDebug.text = randomGameGround.GetComponent<LineRenderer>().GetPosition(0).x.ToString(); 
        // TODO tester pourquoi le setposition ne marche pas : tester coordonnées points
    }
    /* Fonction faisant la moyenne des 4 point selectionnés via le AR*/
    void Egalise()
    {
        string debug = "test ";
        float total_y = 0.0f;
        testDebug.text = debug;
        foreach (var trackedImage in m_timImageManager.trackables)
        {
            total_y += trackedImage.transform.position.y;
            Vector3 vect = trackedImage.transform.position;
           m_lImagePositionList.Add(vect);
            debug += vect.ToString() + "--";
            testDebug.text = debug;
        }
        testDebug.text = debug;
         debug += $"total y : {total_y}" + "---" + m_timImageManager.trackables.count;
         testDebug.text = debug;

        

        
        float average_y = (total_y / m_timImageManager.trackables.count);
        foreach (var position in m_lImagePositionList)
        {
            m_lImagePositionList[m_paperCounter] = new Vector3(position.x, average_y, position.z);
            m_paperCounter++;
        }
        debug += $"Vecteur1: {m_lImagePositionList[0].x} {m_lImagePositionList[0].y} {m_lImagePositionList[1].y} {m_lImagePositionList[2].y} {m_lImagePositionList[0].z} "; 
        testDebug.text = debug;
        // https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@3.0/manual/anchor-manager.html
    }

    IEnumerator DelayAffiche(ARTrackedImage newImage, string text)
    {
        yield return new WaitForEndOfFrame();
        testDebug.text = $"Image: {newImage.referenceImage.name} is at " + $"{text}";
    }

    /*Fonction de test permettant de lister les images*/
    void ListAllImages()
    {
        string debug;
        debug =
            $"There are {m_timImageManager.trackables.count} images being tracked.";

        foreach (var trackedImage in m_timImageManager.trackables)
        {
            debug += $"Image: {trackedImage.referenceImage.name} is at " +
                      $"{trackedImage.transform.position}";
        }
        testDebug.text = debug;
    }

    void OnEnable() => m_timImageManager.trackedImagesChanged += OnChanged;

    void OnDisable() => m_timImageManager.trackedImagesChanged -= OnChanged;

    void OnChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var newImage in eventArgs.added)
        {
            GameObject testARImage = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testARImage.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            testARImage.transform.position = newImage.transform.position;
            count++;
            testDebug.text += "ajout d'un cube " + count + "-" + newImage.transform.position.ToString();
            if (count == 4)
            {
                testDebug.text += "test";
                //Egalise();
            }
            newImage.
            StartCoroutine(DelayAffiche(newImage, newImage.transform.position.ToString()));
        }

        foreach (var updatedImage in eventArgs.updated)
        {
            // Handle updated event

            //StartCoroutine(DelayAffiche(updatedImage, updatedImage.transform.position.ToString()));
        }

        foreach (var removedImage in eventArgs.removed)
        {
            testDebug.text = "LOST";
            // Handle removed event
        }
    }

}