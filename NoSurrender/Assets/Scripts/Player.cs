using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

    [Header("PLAYER SETTINGS")]   //Oynanabilir karakter ayarlar�
    [SerializeField] float speed = 0.005f;             
    [SerializeField] float maxSpeed = 50f;    
    [SerializeField] float attackRange = 15f;

    [SerializeField] Rigidbody playerRb;
    [SerializeField] Transform body;
    [SerializeField] GameObject[] weapons;

    [Header("AREA BLAST SETTINGS")]    //Alan sald�r�s� ayarlar�
    [SerializeField] Transform circle;
    [SerializeField] ParticleSystem particle;
    public float blastCharge = 0f;
    public float maxBlastCharge = 15f;   

    [Header("POWER UP SETTINGS")]     //Yerden al�nan silahlar�n ayarlar�     
    public bool powerUpActive = false;
    public GameObject laser;
    public GameObject shotgun;
    [SerializeField] float powerUpDuration = 2f;    

    [Header("ENEMY SETTINGS")]       //D��man bilgileri
    [SerializeField] List<EnemyAI> enemyList = new List<EnemyAI>();    
    [SerializeField] Transform target;

    [Header("UI SETTINGS")]          //Aray�z ayarlar�
    [SerializeField] Text enemyCount;
    [SerializeField] Text bossCount;
    [SerializeField] Text success;
    [SerializeField] Text gameOver;
    [SerializeField] Button tryAgain;

    //VEKT�RLER
    Vector3 pointA;
    Vector3 pointB;
    Vector3 moveDirection;
    Vector3 enemyDirection;    

    
    void Start()
    {
        Time.timeScale = 1;
    }

    
    void Update()
    {
        Movement();

        GetTarget();
        PlayerBehaviour();

        WeaponStatus();
        AreaBlast();

        Texts();        

        if (FindObjectsOfType<EnemyAI>().Length == 0)
        {
            SuccessScreen();
        }
    }

    //OYUNCU HAREKET�   
    private void Movement() 
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                pointA = new Vector3(touch.position.x, transform.position.y, touch.position.y);  //Ekrana dokunulan nokta              
            }

            if (touch.phase == TouchPhase.Moved)
            {
                pointB = new Vector3(touch.position.x, transform.position.y, touch.position.y);  //Kayd�r�lan nokta
            }
            else if (touch.phase == TouchPhase.Stationary)
            {
                pointB = new Vector3(touch.position.x, transform.position.y, touch.position.y);
            }

            if(touch.phase == TouchPhase.Ended)
            {
                particle.gameObject.SetActive(true);                
            }

            moveDirection = pointB - pointA;                                                     //�lerleme do�rultusu
            Vector3 direction = Vector3.ClampMagnitude(moveDirection, maxSpeed);                 //Maksimum h�z sabiti
            playerRb.velocity = direction * Time.deltaTime * speed;                              //Karakterin ilerlemesi
            
            
        }
        else
        {
            playerRb.velocity = Vector3.zero;            //Karakterin durmas�
        }
    }
    void Rotate(Vector3 direction)
    {
        float timeCount = 0.0f;
        timeCount += Time.deltaTime * 10f;           //D�n�� h�z�n� ayarlayan de�er

        direction.y = 0;                             //Karakterin do�rultusunun bozulmamas� i�in Y ekseninin sabitlenmesi 

        Quaternion lookRotation = Quaternion.LookRotation(direction);        
        body.localRotation = Quaternion.Slerp(body.localRotation, lookRotation, timeCount);   //Karakter v�cudunun d�nmesi
    }


    //OYUNCU DAVRANI�I
    private void GetTarget()
    {
        var sceneEnemies = FindObjectsOfType<EnemyAI>();  //Aktif sahnedeki b�t�n d��manlar

        foreach (EnemyAI enemy in sceneEnemies)           
        {
            float distanceToEnemy = Vector3.Distance(enemy.transform.position, gameObject.transform.position);

            if (distanceToEnemy <= attackRange)          //Aktif d��manlar aras�nda menzilde olanlar�n bir listeye eklenmesi
            {
                if (!enemyList.Contains(enemy))
                {
                    enemyList.Add(enemy);
                }
            }
            if(distanceToEnemy > attackRange)            //Menzil d���na ��kanlar�n listeden ��kart�lmas�
            {
                if (enemyList.Contains(enemy))
                {
                    enemyList.Remove(enemy);
                }
            }
        }

        for (int i = 0; i < enemyList.Count; i++)         
        {
            if (enemyList[i] == null)                          //Yokedilmi� d��manlar�n listeden temizlenmesi
            {
                enemyList.Remove(enemyList[i]);
            }
            else if (enemyList[i].GetComponent<BossHealth>())  //Boss'un �ncelikli hedef olarak se�ilmesi
            {
                enemyList[0] = enemyList[i];
            }
        }

        if (enemyList.Count == 0)    //Hedef belirlenmesi
        {
            target = null;
        }
        else if (enemyList[0] != null)
        {            
            target = enemyList[0].transform;
        }
    }
    private void PlayerBehaviour()  
    {
        if (target)        //Karakterin hedef durumuna g�re davran�� �ekli
        {
            enemyDirection = target.position - transform.position;
            Rotate(enemyDirection);
            Shoot(true);
        }
        else
        {
            Rotate(moveDirection);
            Shoot(false);
        }
    }


    //SALDIRILAR
    private void Shoot(bool shoot)  
    {
        foreach (GameObject weapon in weapons)         
        {
            ParticleSystem ps = weapon.GetComponentInChildren<ParticleSystem>(); 
            var em = ps.emission;
            em.enabled = shoot;                        
        }
    }  
    private void WeaponStatus()
    {
        foreach (GameObject weapon in weapons)
        {
            if (powerUpActive == true)
            {
                weapon.SetActive(false);
            }
            else
            {
                weapon.SetActive(true);
            }
        }
    }
    private void AreaBlast()
    {
        //Sadece tek bir de�i�ken ayarlayarak (maxBlastCharge) b�t�n �zellik ayarlanabilir

        //Alan sald�r�s�n�n zaman i�inde birikmesi
        blastCharge += Time.deltaTime;                                
        blastCharge = Mathf.Clamp(blastCharge, 0, maxBlastCharge);

        //Oyuncunun alt�ndaki sald�r� alan�n�n �arja g�re b�y�mesi
        circle.localScale = new Vector3(blastCharge, 0.01f, blastCharge);  

        //Alan sald�r�s�n�n �arja ba�l� h�z�
        var startSpeed = particle.main;               
        startSpeed.startSpeed = maxBlastCharge * (blastCharge / maxBlastCharge);

        //Alan sald�r�sn�n zaman i�inde boyutunun artmas�
        var size = particle.sizeOverLifetime;           
        AnimationCurve curve = new AnimationCurve();
        curve.AddKey(0.0f, 0.1f);
        curve.AddKey(1f, 5f * (blastCharge / maxBlastCharge));
        size.size = new ParticleSystem.MinMaxCurve(1, curve);

        //Alan sald�r�s� i�in kullan�lan collider'�n �arja ba�l� olarak yar��ap�n�n artmas�
        particle.GetComponent<SphereCollider>().radius = blastCharge * 0.5f;  
    }
    public void ActivatePowerUp(GameObject powerUp)
    {
        StartCoroutine(PowerUp(powerUp));
    }
    IEnumerator PowerUp(GameObject powerUp)
    {
        powerUpActive = true;
        powerUp.SetActive(true);

        yield return new WaitForSeconds(powerUpDuration);

        powerUpActive = false;
        powerUp.SetActive(false);
    }


    //OYUNCU �L�M�
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            GameOverScreen();
        }
    }


    //ARAY�Z
    private void Texts()
    {
        enemyCount.text = "Enemy Count = " + FindObjectsOfType<EnemyHealth>().Length.ToString();
        bossCount.text = "Boss Count = " + FindObjectsOfType<BossHealth>().Length.ToString();
        success.text = "Mission Successful!";
        gameOver.text = "Game Over";
    }
    private void SuccessScreen()
    {        
        success.gameObject.SetActive(true);
        Invoke("ReloadScene", 2.0f); 
    }
    private void GameOverScreen()
    {        
        gameOver.gameObject.SetActive(true);
        Invoke("ReloadScene", 2.0f);        
    }
    void ReloadScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
}
