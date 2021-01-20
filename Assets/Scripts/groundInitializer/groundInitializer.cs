/*
 * Nom de l’auteur : Yannis Querenet
 * Principe du script : Le script sert à initialiser un sol en réalité augmentée
 * Utilisation : Le script doit être lancé après le démarrage d'une partie pour définir un terrain.
 * A mettre dans le ARSessionOrigin :
 * - "goPlanePrefab" correspond au prefab "groundVisualizer", 
 * - "mGroundColor" correspond au material utilisé.
 * - "mStadePrefab" correspond au prefab du stade utilisé (uniquement pour la fonction CreateGround).
 * - "tTestDebug" utilisé pour le débug et les messages d'erreur en AR.
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
    public GameObject m_GOStadePrefab; // Prefab du stade
    public Text m_tTestDebug; // Texte de debug.
    private bool m_isAlreadyGroundExist = false; // Si un terrain a déjà été posé. A faux par défaut.
    public ARTrackedImageManager m_timImageManager; // Le Tracked Image Manager
    private List<Vector3> m_lImagePositionList; // Liste des vector3 de position, utilisée pour créer le terrain AR.
    [SerializeField]
    private float m_fArenaScale = 0.06f;


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
        GameObject gameGround = Instantiate(m_GOStadePrefab);
        gameGround.transform.position = m_lARRayCastHit[0].pose.position;
        gameGround.transform.localScale = new Vector3(m_fArenaScale, m_fArenaScale, m_fArenaScale);
        m_isAlreadyGroundExist = true;
    }
    /*Fonction permettant de créer un sol avec des un terrain random
    * => Actuellement non fonctionnel. En cours.
    */
    void CreateRandomGround()
    {
        /*
        float x;
        float y;
        float z;

        GameObject randomGameGround = new GameObject();
        randomGameGround.transform.position = m_lARRayCastHit[0].pose.position;
        randomGameGround.AddComponent<LineRenderer>();
        randomGameGround.GetComponent<LineRenderer>().positionCount = 4;
        randomGameGround.GetComponent<LineRenderer>().loop = true;
        randomGameGround.GetComponent<LineRenderer>().useWorldSpace = true;
        x = m_lARRayCastHit[0].pose.position.x + Random.Range(-1.0f, 1.0f);
        y = m_lARRayCastHit[0].pose.position.y;
        z = m_lARRayCastHit[0].pose.position.z + Random.Range(-1.0f, 1.0f);
        randomGameGround.GetComponent<LineRenderer>().SetPosition(0, new Vector3(x + 1.5f, y, z + 0.8f));
        randomGameGround.GetComponent<LineRenderer>().SetPosition(1, new Vector3(x + 1.5f, y, z - 0.8f));
        randomGameGround.GetComponent<LineRenderer>().SetPosition(2, new Vector3(x - 1.5f, y, z - 0.8f));
        randomGameGround.GetComponent<LineRenderer>().SetPosition(3, new Vector3(x - 1.5f, y, z + 0.8f));
        */
        Vector3[] vertices = new Vector3[4];
       

        float x = m_lARRayCastHit[0].pose.position.x;
        float y = m_lARRayCastHit[0].pose.position.y;
        float z = m_lARRayCastHit[0].pose.position.z;

        vertices[0] = new Vector3(x - Random.Range(0.8f, 2.2f), y, z + Random.Range(0.2f, 1.3f));
        vertices[1] = new Vector3(x + Random.Range(0.8f, 2.2f), y, z + Random.Range(0.2f, 1.3f));
        vertices[2] = new Vector3(x - Random.Range(0.8f, 2.2f), y, z - Random.Range(0.2f, 1.3f));
        vertices[3] = new Vector3(x + Random.Range(0.8f, 2.2f), y, z - Random.Range(0.2f, 1.3f));

 
        CreateMesh(m_lARRayCastHit[0].pose.position, vertices, setUv(), setTriangles());
  
    }
    int[] setTriangles() {
        int[] triangles = new int[6];
        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;
        triangles[3] = 2;
        triangles[4] = 1;
        triangles[5] = 3;
        return triangles;
    }
    Vector2[] setUv()
    {
        Vector2[] uv = new Vector2[4];
        uv[0] = new Vector2(0, 1);
        uv[1] = new Vector2(1, 1);
        uv[2] = new Vector2(0, 0);
        uv[3] = new Vector2(1, 0);
        return uv;

    }
    /*Fonction créant le mesh du sol*/
    void CreateMesh(Vector3 screenHit, Vector3[] vertices, Vector2[] uv, int[] triangles)
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        GameObject ground = new GameObject("Mesh", typeof(MeshFilter), typeof(MeshRenderer));
        ground.transform.position = screenHit;
        ground.GetComponent<MeshFilter>().mesh = mesh;
        ground.GetComponent<MeshRenderer>().material = m_mGroundColor;
       // m_tTestDebug.text = ground.transform.position.ToString();
    }
    /* Fonction faisant la moyenne des 4 point selectionnés via le AR*/
    void Egalise()
    {
        float total_y = 0.0f;
        int m_paperCounter = 0;
        foreach (var trackedImage in m_timImageManager.trackables)
        {
            total_y += trackedImage.transform.position.y;
            Vector3 vect = trackedImage.transform.position;
           m_lImagePositionList.Add(vect);
        }
        float average_y = (total_y / m_timImageManager.trackables.count);

        for (int i = 0; i < m_lImagePositionList.Count; i++)
        {
            m_lImagePositionList[m_paperCounter] = new Vector3(m_lImagePositionList[m_paperCounter].x, average_y, m_lImagePositionList[m_paperCounter].z);
            m_paperCounter++;
        }
        // https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@3.0/manual/anchor-manager.html
    }
    /*Fonction de test pour insérer un délais*/
    IEnumerator DelayAffiche(ARTrackedImage newImage, string text)
    {
        yield return new WaitForEndOfFrame();
        m_tTestDebug.text = $"Image: {newImage.referenceImage.name} is at " + $"{text}";
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
        m_tTestDebug.text = debug;
    }

    void OnEnable() => m_timImageManager.trackedImagesChanged += OnChanged;

    void OnDisable() => m_timImageManager.trackedImagesChanged -= OnChanged;

    /*Fonction OnChanged utilisée lors du scan des images définissant les points du terrain*/
    void OnChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        /*Appelé à chaque image ajoutée*/
        foreach (var newImage in eventArgs.added)
        {
            GameObject testARImage = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testARImage.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            testARImage.transform.position = newImage.transform.position;
            m_tTestDebug.text += "ajout d'un cube " + "-" + newImage.transform.position.ToString() + m_timImageManager.trackables.count + "--";
        }
        /*Appelé à chaque image actualisé*/
        foreach (var updatedImage in eventArgs.updated)
        {

            if (m_timImageManager.trackables.count == 4)
            {
                Egalise();
            }
        }
        /*Appelé à chaque image perdu*/
        foreach (var removedImage in eventArgs.removed)
        {
            m_tTestDebug.text = "L'image" + removedImage.referenceImage.name + "a été perdue";
        }
    }

}