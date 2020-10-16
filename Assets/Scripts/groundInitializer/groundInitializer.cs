/*
 * Nom de l’auteur : Yannis Querenet
 * Principe du script : Le script sert à initialiser un sol en réalité augmentée
 * Utilisation : Le script doit être lancé après le démarrage d'une partie pour définir un terrain.
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class groundInitializer : MonoBehaviour
{
    public ARRaycastManager m_rmRaycastManager;
    public ARPlaneManager m_pmPlaneManager;
    public List<ARRaycastHit> m_lARRayCastHit;
    public GameObject planeprefab;
    public const float CUBE_X = 3f;
    public const float CUBE_Y = 0.01f;
    public const float CUBE_Z = 5f;
    public Text debug;
    public Material green;

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
    void HidePlaneManager()
    {
        foreach (ARPlane arplane in GameObject.FindObjectsOfType<ARPlane>())
        {
            arplane.GetComponent<MeshRenderer>().material.color = (new Color(0, 0, 0, 0));
        }
        planeprefab.GetComponent<MeshRenderer>().material.color = (new Color(0, 0, 0, 0));

    }

    void CreateGround()
    {
        GameObject gameGround = GameObject.CreatePrimitive(PrimitiveType.Cube);
        gameGround.transform.position = m_lARRayCastHit[0].pose.position;
        gameGround.transform.localScale = new Vector3(CUBE_X + (Random.value * 2), CUBE_Y, CUBE_Z + (Random.value * 2));
        gameGround.GetComponent<MeshRenderer>().material = green;

    }
}