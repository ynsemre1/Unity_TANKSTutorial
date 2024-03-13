using UnityEngine;

public class TankMovement : MonoBehaviour
{
    public int m_PlayerNumber = 1; // (Degisken)       
    public float m_Speed = 12f; // (Degisken)           
    public float m_TurnSpeed = 180f; // (Degisken)      
    public AudioSource m_MovementAudio; //AudioSource bileseni  
    public AudioClip m_EngineIdling; //A.Clip bileseni      
    public AudioClip m_EngineDriving; //A.Clip bileseni     
    public float m_PitchRange = 0.2f; // (Degisken)

    
    private string m_MovementAxisName; // (Degisken)    
    private string m_TurnAxisName; // (Degisken)       
    private Rigidbody m_Rigidbody; // Rigidbody bileseni        
    private float m_MovementInputValue; // (Degisken)   
    private float m_TurnInputValue; // (Degisken)       
    private float m_OriginalPitch; // (Degisken)        


    private void Awake() //Start fonksiyonundan önce calisan bir fonksiyondur
    {
        m_Rigidbody = GetComponent<Rigidbody>(); //Scriptin yuklendigi objenin Rigidbody bilesenini m_Rigidbody degiskenine tanimlar
    }


    private void OnEnable () //Obje etkin oldugunda calisicak fonksiyon
    {
        m_Rigidbody.isKinematic = false; //Objenin fizik motoru tarafindan algilanmasini saglar
        m_MovementInputValue = 0f; //Degiskene 0 float degeri tanimlanmis
        m_TurnInputValue = 0f; //Degiskene 0 float degeri tanimlanmis
    }


    private void OnDisable () //Obje kapali oldugunda calisicak fonksiyon
    {
        m_Rigidbody.isKinematic = true; //Objenin fizik motoru tarafindan algilanmamasini saglar
    }


    private void Start()
    {
        m_MovementAxisName = "Vertical" + m_PlayerNumber;
        m_TurnAxisName = "Horizontal" + m_PlayerNumber;

        m_OriginalPitch = m_MovementAudio.pitch; //Ses ayari
    }
    

    private void Update()
    {
        // Store the player's input and make sure the audio for the engine is playing.
        m_MovementInputValue = Input.GetAxis (m_MovementAxisName);
        m_TurnInputValue = Input.GetAxis (m_TurnAxisName);

        EngineAudio();
    }


    private void EngineAudio()
    {
        // Play the correct audio clip based on whether or not the tank is moving and what audio is currently playing.
        if(Mathf.Abs (m_MovementInputValue) < 0.1f && Mathf.Abs (m_TurnInputValue) < 0.1f)
        {
            if(m_MovementAudio.clip = m_EngineDriving) //Unity de ki m_MovementAudio bileşenine m_EngineDriving bileseninde ki ses dosyasi var ise
            {
                m_MovementAudio.clip = m_EngineIdling; //MovementAudio bileşenine m_EngineIdling bileseninde ki ses dosyasini yükle
                m_MovementAudio.pitch = Random.Range (m_OriginalPitch - m_PitchRange , m_OriginalPitch + m_PitchRange); // "m_OriginalPitch - m_PitchRange , m_OriginalPitch + m_PitchRange" degerleri arasindan rastgele bir deger al
                m_MovementAudio.Play(); //Muzigi baslat
            }
        }
        else
        {
            if(m_MovementAudio.clip = m_EngineIdling) //Unity de ki m_MovementAudio bileşenine m_EngineIdling bileseninde ki ses dosyasi var ise
            {
                m_MovementAudio.clip = m_EngineDriving; //MovementAudio bileşenine m_EngineIdling bileseninde ki ses dosyasini yükle
                m_MovementAudio.pitch = Random.Range (m_OriginalPitch - m_PitchRange , m_OriginalPitch + m_PitchRange); // "m_OriginalPitch - m_PitchRange , m_OriginalPitch + m_PitchRange" degerleri arasindan rastgele bir deger al
                m_MovementAudio.Play(); //Muzigi baslat
            }
        }
    }


    private void FixedUpdate()
    {
        // Move and turn the tank.
        Move();
        Turn();
    }


    private void Move()
    {
        // Adjust the position of the tank based on the player's input.
        Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime; //movement Vectorune transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime degiskenleri eklenir bu da Vectorun Z ekseninde bir artis gerceklestirir
        //.MovePosition Rigidbody bilesenlerinin dogrudan degil daha stabil ve yumusak hareket etmesini saglayan Unity fonksiyonudur
        m_Rigidbody.MovePosition(m_Rigidbody.position + movement); //Objenin rigidbody bilesenini movement Vector3 u yonunde yumusak bir sekilde hareket ettiren komut
    }


    private void Turn()
    {
        float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime; //turn degiskeni olusuturulup icerisine "m_TurnInputValue * m_TurnSpeed * Time.deltaTime" degerlerinin carpimi atanmis

        //Hedef obje kendi etrafinda donme hareketi sagladigi icin rotation (Y ekseni) ile donme islemi saglar bunu da Vectorler ile degil Quaternion.Euler ile saglariz
        Quaternion turnRotation = Quaternion.Euler (0f, turn , 0f); 
        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
    }
}