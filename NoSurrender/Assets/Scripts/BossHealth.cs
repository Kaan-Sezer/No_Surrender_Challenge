using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossHealth : MonoBehaviour
{
    Player player;

    [SerializeField] float health = 100f;  //Dinamik can de�i�keni
    float baseHealth;                      //Statik can de�i�keni

    //Boss'un boyut de�erleri
    float x;
    float y;
    float z;
    
    void Start()
    {
        player = FindObjectOfType<Player>();

        baseHealth = health; //Statik can de�erinin ba�tan belirlenmesi
        
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
        //Boss'un boyutunun cana ba�l� olarak azalmas�
        gameObject.transform.localScale = new Vector3(x / 5 + x * (health / baseHealth) * 0.8f,
                                                      y / 5 + y * (health / baseHealth) * 0.8f,
                                                      z / 5 + z * (health / baseHealth) * 0.8f);
    }
    private void ColorChange()
    {
        //Boss'un renginin cana ba�l� olarak de�i�mesi
        gameObject.GetComponent<Renderer>().material.color = new Color(((baseHealth - health) / baseHealth), (health / baseHealth), 0, 1);
    }    

    private void OnTriggerStay(Collider other)              //Collider bazl� sald�r�lar i�in
    {
        if (other.gameObject.CompareTag("Player"))
        {
            health -= Time.deltaTime * 100f;
            player.blastCharge += Time.deltaTime;           //�arj�n d��mana hasar ile artmas�
        }
    }

    private void OnParticleCollision(GameObject other)      //par�ac�k bazl� sald�r�lar i�in
    {
        health -= 1f;
        player.blastCharge += 0.1f;                         //�arj�n d��mana hasar ile artmas�
    }   
}
