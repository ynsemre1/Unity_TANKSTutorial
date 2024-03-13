using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float m_DampTime = 0.2f; //Damp Zamani (Degisken)                 
    public float m_ScreenEdgeBuffer = 4f; //Ekran Kenar Degeri (Degisken)         
    public float m_MinSize = 6.5f; //Minimum Ekran Degeri (Degisken)                
    public Transform[] m_Targets; //Dusmanlarin Transform Bileseni Alinir (Bilesen)


    private Camera m_Camera; //Camera Bileseni (Bilesen)                    
    private float m_ZoomSpeed; //Zoom Degeri (Degisken)                     
    private Vector3 m_MoveVelocity; //Hareket Degeri (Degisken)                
    private Vector3 m_DesiredPosition; //İstenilen Pozisyon (Vector)               


    private void Awake() //Proje baslamadan once objenin içerisinde ki kamera objesini m_Camera bilesenine atar 
    {
        m_Camera = GetComponentInChildren<Camera>(); //m_Camera ya eklenen objenin icerisinde ki kamera objesini m_Camera bilesenine atar
    }


    private void FixedUpdate() //Proje basladıgı anda Move() Zoom() fonksiyonlarini calistirir
    {
        //FixedUpdate fonksiyonu projenin fizik vb. ayarlarinin calistirilmasi icin onemli bir fonksiyondur 

        Move();
        Zoom();
    }


    private void Move()
    {
        FindAveragePosition(); //m_DesiredPosition degiskeni kullanilmasi için Move() fonksiyonuna FindAveragePosition() fonksiyonu eklendi

        //Kamera hareketlerinin daha yumusak olması icin scriptin eklendigi objenin pozisyonlarına m_DesiredPosition yani hedef pozisyon degeri eklendi
        transform.position = Vector3.SmoothDamp(transform.position, m_DesiredPosition, ref m_MoveVelocity, m_DampTime);
    }


    private void FindAveragePosition() //Tankların position degerlerini alıp, tank sayısına böldü ve orta Vector degerini buldu
    {
        Vector3 averagePos = new Vector3(); //Vector3 degiskeni olusturuldu
        int numTargets = 0; //numTargets degiskeni 0 atandi

        for (int i = 0; i < m_Targets.Length; i++) //Döngü m_Target objelerinin sayisi kadar döner
        {
            if (!m_Targets[i].gameObject.activeSelf) //Objenin aktif olup olmadigini taradi
                continue; //Aktif obje olmadiginda dongu disina cik

            averagePos += m_Targets[i].position; //Vector3 degerine i indeks değerindeki pozisyonlar ile toplandi
            numTargets++; //Num targetsi arttırdı
        }

        if (numTargets > 0) //numTargets 0 dan buyukse yani bir veya birden fazla ise 
            averagePos /= numTargets; //avaragePos degerini numTargets degerine bol

        averagePos.y = transform.position.y; //avaragePos (y degerini), scriptin atıldıgı objenin (y degerine) esitle

        m_DesiredPosition = averagePos; //Tanımlı (Vector3) degerini avaragePos degerine esitle
    }


    private void Zoom() //Kameranin yakinlastirma ve uzaklastirmasini yumusak olmasını saglayan fonksiyon
    {
        float requiredSize = FindRequiredSize(); //requiredSize degiskenine FindRequiredSize() fonskiyonu atanmis

        //orthographicSize kamera bakış açısına bağlı olarak görünen alanın yüksekliğini belirtir.
        //Mathf.SmoothDamp yumusak gecisler icin kullanılan bir fonksiyondur
        //requiredSize hedef kamera boyutu bu degere ulasmaya calisilacaktir
        //Mathf.SmoothDamp(mevcut deger (gecis yapicak deger), ulasilmak istenilen deger, ref gecis hizi, gecis hizi);
        m_Camera.orthographicSize = Mathf.SmoothDamp(m_Camera.orthographicSize, requiredSize, ref m_ZoomSpeed, m_DampTime);
    }


    private float FindRequiredSize()
    {
        //InverseTransformPoint bir Transform nesnesinde belirtilen bir noktanin Dunya kordinatlarinda donusunu gerceklestirir.
        Vector3 desiredLocalPos = transform.InverseTransformPoint(m_DesiredPosition); //desiredLocalPos degiskenine m_DesiredPosition un Dunya kordinatları donusunu atar

        float size = 0f; //Float degiskeni olusturuldu

        for (int i = 0; i < m_Targets.Length; i++) //m_Targets desikeninin degeri kadar doner
        {
            if (!m_Targets[i].gameObject.activeSelf) //m_Targets objelerinin indeks numarası ile aktif olup olmadigini test eder
                continue; //Eger aktif olmayan bir obje olur ise döngüyü  bitirir

            Vector3 targetLocalPos = transform.InverseTransformPoint(m_Targets[i].position); //m_Targets positionlarinin Dunya kordinatlarinda dongusunun transform degerlerini alip targetLocalPos degiskenine atar

            Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos; //Her dongude targetLocalPos dan desiredLocalPos degiskenini cikartip desiredPosToTarget desikenine atar

            size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.y)); //desiredPosToTarget vektorunun y(dikey) bileseninin mutlak degerini alir

            size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.x) / m_Camera.aspect); //desiredPosToTarget vektorunun x(yatay) bileseninin mutlak degerini alir ve kameranin en boy oranina boler ve genisligi elde eder
        }
        
        size += m_ScreenEdgeBuffer; //size degiskeni ile m_ScreenEdgeBuffer degiskenini toplandi

        size = Mathf.Max(size, m_MinSize); //mevcut size degiskeni ile m_MinSize degerini karsilastirip en yuksek olanini size degiskenine atar

        return size; //size degerini dondurur
    }


    public void SetStartPositionAndSize()
    {
        FindAveragePosition();

        transform.position = m_DesiredPosition; //sciptin eklendigi objenin pozisyon degerlerine m_DesiredPosition Vector3 un de ki degerleri atar

        m_Camera.orthographicSize = FindRequiredSize(); //FindRequiredSize fonksiyonunda ki size degiskenini buraya atar
    }
}