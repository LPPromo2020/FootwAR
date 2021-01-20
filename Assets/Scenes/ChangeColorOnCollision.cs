using System;
using UnityEngine;
using UnityEngine.UI;

//Cléry Chassagnon
//Script qui gère la physic de la balle avec le joueur et les GameObjects
//Gère aussi l'UI de modification in game
public class ChangeColorOnCollision : MonoBehaviour
{
    public float m_fCollisionForce = 100f;
    public GameObject m_GOBouncinessSlider;
    public PhysicMaterial m_GOBouncyMaterial;
    public GameObject m_GOMassSlider;
    public GameObject m_GOCollisionSlider;
    public GameObject m_GOFallForceSlider;

    private int m_iHitNumber = 1;

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            GetComponent<Rigidbody>().AddForce(new Vector3(other.relativeVelocity.x * (m_fCollisionForce / m_iHitNumber), other.relativeVelocity.y * (m_fCollisionForce / m_iHitNumber), other.relativeVelocity.z * (m_fCollisionForce / m_iHitNumber)));

            m_iHitNumber++;

            print(m_iHitNumber);
        }
        else
        {
            m_iHitNumber = 1;
        }
        Color randomlySelectColor = GetRandomColor();

        GetComponent<Renderer>().material.color = randomlySelectColor;
    }

    private Color GetRandomColor()
    {
        return new Color(
                UnityEngine.Random.Range(0f, 1f),
                UnityEngine.Random.Range(0f, 1f),
                UnityEngine.Random.Range(0f, 1f));
    }


    public void BouncinessChange()
    {
        m_GOBouncyMaterial.bounciness = m_GOBouncinessSlider.GetComponent<Slider>().value;

        m_GOBouncinessSlider.transform.GetChild(4).GetComponent<Text>().text = m_GOBouncyMaterial.bounciness.ToString();
    }
    
    public void MassChange()
    {
        GetComponent<Rigidbody>().mass = m_GOMassSlider.GetComponent<Slider>().value;

        m_GOMassSlider.transform.GetChild(4).GetComponent<Text>().text = GetComponent<Rigidbody>().mass.ToString();
    }

    public void CollisionChange()
    {
        m_fCollisionForce = m_GOCollisionSlider.GetComponent<Slider>().value;

        m_GOCollisionSlider.transform.GetChild(4).GetComponent<Text>().text = m_fCollisionForce.ToString();
    }

    public void FallForceChange()
    {
        GetComponent<ConstantForce>().force = new Vector3(0, m_GOFallForceSlider.GetComponent<Slider>().value, 0);

        m_GOFallForceSlider.transform.GetChild(4).GetComponent<Text>().text = GetComponent<ConstantForce>().force.y.ToString();
    }
}
