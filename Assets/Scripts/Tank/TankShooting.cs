using UnityEngine;
using UnityEngine.UI;

public class TankShooting : MonoBehaviour
{
    public int m_PlayerNumber = 1; //(Degisken)     
    public Rigidbody m_Shell; //RigidBody bileseni           
    public Transform m_FireTransform; //Transform bileseni   
    public Slider m_AimSlider; //Slider bileseni          
    public AudioSource m_ShootingAudio; //Audio Soruce bileseni 
    public AudioClip m_ChargingClip; //Audio Clip bileseni   
    public AudioClip m_FireClip; //Audio Clip bileseni        
    public float m_MinLaunchForce = 15f; //(Degisken)
    public float m_MaxLaunchForce = 30f; //(Degisken)
    public float m_MaxChargeTime = 0.75f; //(Degisken)

    
    private string m_FireButton; //(Degisken)       
    private float m_CurrentLaunchForce; //(Degisken) 
    private float m_ChargeSpeed; //(Degisken)        
    private bool m_Fired;  //(Degisken)              


    private void OnEnable() 
    {
        m_CurrentLaunchForce = m_MinLaunchForce; //m_CurrentLaunchForce degiskenine m_MinLaunchForce degeri atanmis
        m_AimSlider.value = m_MinLaunchForce; //m_AimSlider degisken degeri m_MinLaunchForce degeri atanmis
    }


    private void Start()
    {
        m_FireButton = "Fire" + m_PlayerNumber; 

        m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime; //m_ChargeSpeed degiskeni icerisinde m_MaxLaunchForce degerinden m_MinLaunchForce degerinin cikarilmis halinin m_MaxChargeTime degiskenine bolunmus hali atanmis
    }
    

    private void Update()
    {
        // Track the current state of the fire button and make decisions based on the current launch force.
        m_AimSlider.value = m_MinLaunchForce;

        if(m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired)
        {
            m_CurrentLaunchForce = m_MaxLaunchForce;
            Fire();
        }else if(Input.GetButtonDown(m_FireButton))
        {
            m_Fired = false;
            m_CurrentLaunchForce = m_MinLaunchForce;

            m_ShootingAudio.clip = m_ChargingClip;
            m_ShootingAudio.Play();
        }else if(Input.GetButton(m_FireButton) && !m_Fired)
        {
            m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;
            m_AimSlider.value = m_CurrentLaunchForce;
        }else if(Input.GetButtonUp(m_FireButton) && !m_Fired)
        {
            Fire();
        }
    }


    private void Fire()
    {
        // Instantiate and launch the shell.
        m_Fired = true;

        Rigidbody shellInstance = Instantiate (m_Shell, m_FireTransform.position, m_FireTransform.rotation);

        shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward;

        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();

        m_CurrentLaunchForce = m_MinLaunchForce;
    }
}