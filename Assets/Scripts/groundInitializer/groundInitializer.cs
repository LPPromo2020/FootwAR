/*
 * Nom de l’auteur : Yannis Querenet
 * Principe du script : Le script sert à initialiser un sol en réalité augmentée
 * Utilisation : Le script doit être lancé après le démarrage d'une partie pour définir un terrain.
 * A mettre dans le ARSessionOrigin :
 * - "goPlanePrefab" correspond au prefab "groundVisualizer", 
 * - "mGroundColor" correspond au material utilisé.
 * - "mStadePrefab" correspond au prefab du stade utilisé (uniquement pour la fonction CreateGround).
 * - "tTestDebug" utilisé pour le débug et les messages d'erreur en AR.
 * - "m_mWallMaterial" correspond au material utilisé pour les murs simples.
 * - "m_mGoalMaterial" correspond au materiel utilisé pour le mur avec les cages (non implémenté actuellement).
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
    public Material m_mWallMaterial; // Material du mur sans cage
    public Material m_mGoalMaterial; // Material du mur avec cage
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

    void CreateGround() // Fonction qui permet de créer le terrain simple.
    {
        GameObject gameGround = Instantiate(m_GOStadePrefab);
        gameGround.transform.position = m_lARRayCastHit[0].pose.position;
        gameGround.transform.localScale = new Vector3(m_fArenaScale, m_fArenaScale, m_fArenaScale);
        m_isAlreadyGroundExist = true;
    }

    /*Fonction permettant de créer un sol avec des un terrain de forme random
    */
    void CreateRandomGround()
    {

        Vector3[] vertices = new Vector3[4];
        Vector3[] verticesWall1 = new Vector3[4];
        Vector3[] verticesWall2 = new Vector3[4];
        Vector3[] verticesWall3 = new Vector3[4];
        Vector3[] verticesWall4 = new Vector3[4];

        float x = m_lARRayCastHit[0].pose.position.x;
        float y = m_lARRayCastHit[0].pose.position.y;
        float z = m_lARRayCastHit[0].pose.position.z;

        vertices[0] = new Vector3(x - Random.Range(0.8f, 2.2f), y, z + Random.Range(0.2f, 1.3f));
        vertices[1] = new Vector3(x + Random.Range(0.8f, 2.2f), y, z + Random.Range(0.2f, 1.3f));
        vertices[2] = new Vector3(x - Random.Range(0.8f, 2.2f), y, z - Random.Range(0.2f, 1.3f));
        vertices[3] = new Vector3(x + Random.Range(0.8f, 2.2f), y, z - Random.Range(0.2f, 1.3f));

        verticesWall1[0] = vertices[0];
        verticesWall1[1] = vertices[2];
        verticesWall1[2] = new Vector3(vertices[0].x, vertices[0].y + 0.3f, vertices[0].z);
        verticesWall1[3] = new Vector3(vertices[2].x, vertices[2].y + 0.3f, vertices[2].z);

        verticesWall2[0] = vertices[2];
        verticesWall2[1] = vertices[3];
        verticesWall2[2] = new Vector3(vertices[2].x, vertices[2].y + 0.3f, vertices[2].z);
        verticesWall2[3] = new Vector3(vertices[3].x, vertices[3].y + 0.3f, vertices[3].z);

        verticesWall3[0] = vertices[3];
        verticesWall3[1] = vertices[1];
        verticesWall3[2] = new Vector3(vertices[3].x, vertices[3].y + 0.3f, vertices[3].z);
        verticesWall3[3] = new Vector3(vertices[1].x, vertices[1].y + 0.3f, vertices[1].z);

        verticesWall4[0] = vertices[1];
        verticesWall4[1] = vertices[0];
        verticesWall4[2] = new Vector3(vertices[1].x, vertices[1].y + 0.3f, vertices[1].z);
        verticesWall4[3] = new Vector3(vertices[0].x, vertices[0].y + 0.3f, vertices[0].z);


        CreateMesh(m_lARRayCastHit[0].pose.position, vertices, setUv(), setTriangles(), m_mGroundColor);
        CreateMesh(m_lARRayCastHit[0].pose.position, verticesWall1, setUv(), setTriangles(), m_mWallMaterial);
        CreateMesh(m_lARRayCastHit[0].pose.position, verticesWall2, setUv(), setTriangles(), m_mWallMaterial);
        CreateMesh(m_lARRayCastHit[0].pose.position, verticesWall3, setUv(), setTriangles(), m_mWallMaterial);
        CreateMesh(m_lARRayCastHit[0].pose.position, verticesWall4, setUv(), setTriangles(), m_mWallMaterial);

        m_isAlreadyGroundExist = true;


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

    /*Fonction créant un mesh*/
    void CreateMesh(Vector3 screenHit, Vector3[] vertices, Vector2[] uv, int[] triangles, Material material)
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        GameObject ground = new GameObject("Mesh", typeof(MeshFilter), typeof(MeshRenderer));
        ground.transform.position = screenHit;
        ground.GetComponent<MeshFilter>().mesh = mesh;
        ground.GetComponent<MeshRenderer>().material = material;


        /*Essai de poser un prefab de mur, ne fonctionne pas
        GameObject wall1 = Instantiate(m_GOWallPrefab);
        wall1.transform.rotation = new Quaternion(wall1.transform.rotation.x, wall1.transform.rotation.y + Mathf.Atan((vertices[0].x - vertices[1].x) / (vertices[0].z - vertices[1].z))-90, wall1.transform.rotation.z, 1);
        wall1.transform.localScale = new Vector3(wall1.transform.localScale.x * vertices[0].x, wall1.transform.localScale.y * vertices[0].y, wall1.transform.localScale.z * vertices[0].z);
        wall1.transform.position = new Vector3((vertices[0].x + vertices[1].x) / 2, vertices[0].y, (vertices[0].z + vertices[1].z) / 2);*/
    }
    /* Fonction faisant la moyenne des 4 point selectionnés via le AR image tracking*/
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
            m_tTestDebug.text =  "Ajout d'un cube " ;
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