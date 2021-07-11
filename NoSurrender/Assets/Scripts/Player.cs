using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

    [Header("PLAYER SETTINGS")]   //Oynanabilir karakter ayarlarý
    [SerializeField] float speed = 0.005f;             
    [SerializeField] float maxSpeed = 50f;    
    [SerializeField] float attackRange = 15f;

    [SerializeField] Rigidbody playerRb;
    [SerializeField] Transform body;
    [SerializeField] GameObject[] weapons;

    [Header("AREA BLAST SETTINGS")]    //Alan saldýrýsý ayarlarý
    [SerializeField] Transform circle;
    [SerializeField] ParticleSystem particle;
    public float blastCharge = 0f;
    public float maxBlastCharge = 15f;   

    [Header("POWER UP SETTINGS")]     //Yerden alýnan silahlarýn ayarlarý     
    public bool powerUpActive = false;
    public GameObject laser;
    public GameObject shotgun;
    [SerializeField] float powerUpDuration = 2f;    

    [Header("ENEMY SETTINGS")]       //Düþman bilgileri
    [SerializeField] List<EnemyAI> enemyList = new List<EnemyAI>();    
    [SerializeField] Transform target;

    [Header("UI SETTINGS")]          //Arayüz ayarlarý
    [SerializeField] Text enemyCount;
    [SerializeField] Text bossCount;
    [SerializeField] Text success;
    [SerializeField] Text gameOver;
    [SerializeField] Button tryAgain;

    //VEKTÖRLER
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

    //OYUNCU HAREKETÝ   
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
                pointB = new Vector3(touch.position.x, transform.position.y, touch.position.y);  //Kaydýrýlan nokta
            }
            else if (touch.phase == TouchPhase.Stationary)
            {
                pointB = new Vector3(touch.position.x, transform.position.y, touch.position.y);
            }

            if(touch.phase == TouchPhase.Ended)
            {
                particle.gameObject.SetActive(true);                
            }

            moveDirection = pointB - pointA;                                                     //Ýlerleme doðrultusu
            Vector3 direction = Vector3.ClampMagnitude(moveDirection, maxSpeed);                 //Maksimum hýz sabiti
            playerRb.velocity = direction * Time.deltaTime * speed;                              //Karakterin ilerlemesi
            
            
        }
        else
        {
            playerRb.velocity = Vector3.zero;            //Karakterin durmasý
        }
    }
    void Rotate(Vector3 direction)
    {
        float timeCount = 0.0f;
        timeCount += Time.deltaTime * 10f;           //Dönüþ hýzýný ayarlayan deðer

        direction.y = 0;                             //Karakterin doðrultusunun bozulmamasý için Y ekseninin sabitlenmesi 

        Quaternion lookRotation = Quaternion.LookRotation(direction);        
        body.localRotation = Quaternion.Slerp(body.localRotation, lookRotation, timeCount);   //Karakter vücudunun dönmesi
    }


    //OYUNCU DAVRANIÞI
    private void GetTarget()
    {
        var sceneEnemies = FindObjectsOfType<EnemyAI>();  //Aktif sahnedeki bütün düþmanlar

        foreach (EnemyAI enemy in sceneEnemies)           
        {
            float distanceToEnemy = Vector3.Distance(enemy.transform.position, gameObject.transform.position);

            if (distanceToEnemy <= attackRange)          //Aktif düþmanlar arasýnda menzilde olanlarýn bir listeye eklenmesi
            {
                if (!enemyList.Contains(enemy))
                {
                    enemyList.Add(enemy);
                }
            }
            if(distanceToEnemy > attackRange)            //Menzil dýþýna çýkanlarýn listeden çýkartýlmasý
            {
                if (enemyList.Contains(enemy))
                {
                    enemyList.Remove(enemy);
                }
            }
        }

        for (int i = 0; i < enemyList.Count; i++)         
        {
            if (enemyList[i] == null)                          //Yokedilmiþ düþmanlarýn listeden temizlenmesi
            {
                enemyList.Remove(enemyList[i]);
            }
            else if (enemyList[i].GetComponent<BossHealth>())  //Boss'un öncelikli hedef olarak seçilmesi
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
        if (target)        //Karakterin hedef durumuna göre davranýþ þekli
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
        //Sadece tek bir deðiþken ayarlayarak (maxBlastCharge) bütün özellik ayarlanabilir

        //Alan saldýrýsýnýn zaman içinde birikmesi
        blastCharge += Time.deltaTime;                                
        blastCharge = Mathf.Clamp(blastCharge, 0, maxBlastCharge);

        //Oyuncunun altýndaki saldýrý alanýnýn þarja göre büyümesi
        circle.localScale = new Vector3(blastCharge, 0.01f, blastCharge);  

        //Alan saldýrýsýnýn þarja baðlý hýzý
        var startSpeed = particle.main;               
        startSpeed.startSpeed = maxBlastCharge * (blastCharge / maxBlastCharge);

        //Alan saldýrýsnýn zaman içinde boyutunun artmasý
        var size = particle.sizeOverLifetime;           
        AnimationCurve curve = new AnimationCurve();
        curve.AddKey(0.0f, 0.1f);
        curve.AddKey(1f, 5f * (blastCharge / maxBlastCharge));
        size.size = new ParticleSystem.MinMaxCurve(1, curve);

        //Alan saldýrýsý için kullanýlan collider'ýn þarja baðlý olarak yarýçapýnýn artmasý
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


    //OYUNCU ÖLÜMÜ
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            GameOverScreen();
        }
    }


    //ARAYÜZ
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
