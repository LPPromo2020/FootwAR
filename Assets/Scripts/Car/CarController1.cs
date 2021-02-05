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
    protected AcceletrateButton m_abAccelerateButton;
    protected BrakeButton m_bbBrakeButton;
    protected bool m_bJump; 
    public bool m_bIsGrounded;

    public WheelCollider m_WcFrontRightW, m_WcFrontLeftW;
    public WheelCollider m_WcBackRightW, m_WcBackLeftW; 
    public Transform m_TFrontRightW, m_TFrontLeftW;
    public Transform m_TBackRightW, m_TBackLeftW;
    public float m_fMaxStreerAngle = 30;
    public float m_fMotorForce = 5000;
    public float m_fBrake = 10f;
    
    public Transform m_tGroundCheck;

    public Text DebugTest;

    private void Start()
    {
        m_jJoystick = FindObjectOfType<Joystick>();
        m_jbJumpButton = FindObjectOfType<JumpButton>();
        m_abAccelerateButton = FindObjectOfType<AcceletrateButton>();
        m_bbBrakeButton = FindObjectOfType<BrakeButton>();
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

        //accéleration
        if (m_abAccelerateButton.m_bAccelerate)
        {
            m_fVecticalInput = 10;
            m_WcFrontRightW.brakeTorque = 0;
            m_WcFrontLeftW.brakeTorque = 0;
            m_WcBackLeftW.brakeTorque = 0;
            m_WcBackRightW.brakeTorque = 0;
            m_WcFrontRightW.motorTorque = m_fVecticalInput * m_fMotorForce * Time.deltaTime;
            m_WcFrontLeftW.motorTorque = m_fVecticalInput * m_fMotorForce * Time.deltaTime;
            //m_WcBackLeftW.motorTorque = m_fVecticalInput * m_fMotorForce;
            //m_WcBackRightW.motorTorque = m_fVecticalInput * m_fMotorForce;
        }

        //décéleration 
        if (!m_abAccelerateButton.m_bAccelerate)
        {
            m_WcFrontRightW.motorTorque = 0;
            m_WcFrontLeftW.motorTorque = 0;
            m_WcFrontRightW.brakeTorque = m_fBrake * Time.deltaTime;
            m_WcFrontLeftW.brakeTorque = m_fBrake * Time.deltaTime;
            //print(m_fMotorForce);
            // m_WcFrontRightW.brakeTorque = m_fBrake * m_fMotorForce;
            //m_WcFrontLeftW.brakeTorque = m_fBrake * m_fMotorForce;
            print(m_WcFrontRightW.motorTorque);
        }

        DebugTest.text = m_bbBrakeButton.m_bBrake.ToString();
        //freinage
        if (m_bbBrakeButton.m_bBrake)
        {
            DebugTest.text = m_bbBrakeButton.m_bBrake.ToString();
            m_WcFrontRightW.motorTorque = 0;
            m_WcFrontLeftW.motorTorque = 0;
            m_WcFrontRightW.brakeTorque = 99999;
            m_WcFrontLeftW.brakeTorque = 99999;
            m_WcBackLeftW.brakeTorque = 99999;
            m_WcBackRightW.brakeTorque = 99999;
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

   

    private void UpdateWheelPoses()
    {
        UpdateWheelPoses(m_WcFrontLeftW, m_TFrontLeftW);
        UpdateWheelPoses(m_WcBackLeftW, m_TBackLeftW);
        UpdateWheelPoses(m_WcFrontRightW, m_TFrontRightW);
        UpdateWheelPoses(m_WcBackRightW, m_TBackRightW);
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
        UpdateWheelPoses();
    }
}
