using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossHealth : MonoBehaviour
{
    Player player;

    [SerializeField] float health = 100f;  //Dinamik can deðiþkeni
    float baseHealth;                      //Statik can deðiþkeni

    //Boss'un boyut deðerleri
    float x;
    float y;
    float z;
    
    void Start()
    {
        player = FindObjectOfType<Player>();

        baseHealth = health; //Statik can deðerinin baþtan belirlenmesi
        
        x = gameObject.transform.localScale.x;
        y = gameObject.transform.localScale.y;
        z = gameObject.transform.localScale.z;
    }
        
    void Update()
    {
        SizeChange();
        ColorChange();

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void SizeChange()
    {
        //Boss'un boyutunun cana baðlý olarak azalmasý
        gameObject.transform.localScale = new Vector3(x / 5 + x * (health / baseHealth) * 0.8f,
                                                      y / 5 + y * (health / baseHealth) * 0.8f,
                                                      z / 5 + z * (health / baseHealth) * 0.8f);
    }
    private void ColorChange()
    {
        //Boss'un renginin cana baðlý olarak deðiþmesi
        gameObject.GetComponent<Renderer>().material.color = new Color(((baseHealth - health) / baseHealth), (health / baseHealth), 0, 1);
    }    

    private void OnTriggerStay(Collider other)              //Collider bazlý saldýrýlar için
    {
        if (other.gameObject.CompareTag("Player"))
        {
            health -= Time.deltaTime * 100f;
            player.blastCharge += Time.deltaTime;           //Þarjýn düþmana hasar ile artmasý
        }
    }

    private void OnParticleCollision(GameObject other)      //parçacýk bazlý saldýrýlar için
    {
        health -= 1f;
        player.blastCharge += 0.1f;                         //Þarjýn düþmana hasar ile artmasý
    }   
}
