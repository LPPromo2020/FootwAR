/*
Nom de l’auteur : Rémy CRESPE
    Principe du script : Controle la voiture
    Utilisation : Tu met le prefab et il mis dedans
    */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarController : MonoBehaviour
{
    private float m_fHorizontalInput;
    private float m_fVecticalInput;
    private float m_fSteeringAngle;
    protected Joystick m_jJoystick;
    protected JumpButton m_jbJumpButton;
    protected bool m_bJump; 
    public bool m_bIsGrounded;

    public WheelCollider m_WcFrontRightW, m_WcFrontLeftW;
    public WheelCollider m_WcBackRightW, m_WcBackLeftW; 
    public Transform m_TFrontRightW, m_TFrontLeftW;
    public Transform m_TBackRightW, m_TBackLeftW;
    public float m_fMaxStreerAngle = 30;
    public float m_fMotorForce = 50;
    
    public Transform m_tGroundCheck;

    private void Start()
    {
        m_jJoystick = FindObjectOfType<Joystick>();
        m_jbJumpButton = FindObjectOfType<JumpButton>();
        m_bIsGrounded = true;
    }

    private void Update()
    {
        var rigidbody = GetComponent<Rigidbody>();
        if (m_tGroundCheck.position.y < 0 && !m_bJump)
        {
            m_bIsGrounded = true;
            m_bJump = false;
        }

        if ((!m_bJump && m_jbJumpButton.m_bPressed) && m_bIsGrounded)
        {
            m_bJump = true;
            rigidbody.velocity += Vector3.up * 6f;
            m_bIsGrounded = false;

        }

        if (m_bJump && !m_jbJumpButton.m_bPressed)
        {
            m_bJump = false;
        }

    }

    //Simple Jump
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ground")
        {
            m_bIsGrounded = true;
        }
    }

    //Double Jump
    //void OnCollisionStay()
    //{
    //    m_bIsGrounded = true;
    //}

    public void GetInput()
    {
        m_fHorizontalInput = m_jJoystick.Horizontal;
        m_fVecticalInput = m_jJoystick.Vertical;
        //m_fVecticalInput = 2;
    }

    private void Steer()
    {
        m_fSteeringAngle = m_fMaxStreerAngle * m_fHorizontalInput;
        m_WcFrontLeftW.steerAngle = m_fSteeringAngle;
        m_WcFrontRightW.steerAngle = m_fSteeringAngle;
    }

    private void Accelerate()
    {
        m_WcFrontRightW.motorTorque = m_fVecticalInput * m_fMotorForce;
        m_WcFrontLeftW.motorTorque = m_fVecticalInput * m_fMotorForce;
    }

    private void UpdateWheelPoses()
    {
        UpdateWheelPoses(m_WcFrontLeftW, m_TFrontLeftW);
        UpdateWheelPoses(m_WcBackLeftW, m_TBackLeftW);
        UpdateWheelPoses(m_WcFrontRightW, m_TFrontRightW);
        UpdateWheelPoses(m_WcBackLeftW, m_TBackLeftW);
    }

    private void UpdateWheelPoses(WheelCollider wheelCollider_p, Transform transform_p)
    {
        Vector3 pos_l = transform_p.position;
        Quaternion quat_l = transform_p.rotation;

        wheelCollider_p.GetWorldPose(out pos_l, out quat_l);
        transform_p.rotation = quat_l;
        transform_p.position = pos_l;
    }

    private void FixedUpdate()
    {
        GetInput();
        Steer();
        Accelerate();
        UpdateWheelPoses();
    }
}
