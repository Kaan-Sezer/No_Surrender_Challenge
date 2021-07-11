using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    Player player;

    private void Start()
    {
        player = FindObjectOfType<Player>();
    }

    private void OnTriggerEnter(Collider other)   //Collider bazl� sald�r�lar i�in
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
            player.blastCharge += 0.1f;     //�arj�n d��man �l�m� ile artmas�
        }        
    }    

    private void OnParticleCollision(GameObject other) //Par�ac�k bazl� sald�r�lar i�in
    {
        Destroy(gameObject);
        player.blastCharge += 0.1f;     //�arj�n d��man �l�m� ile artmas�
    }

    //private void OnDestroy()
    //{
    //    player.blastCharge += 0.1f;     //�arj�n d��man �l�m� ile artmas�
    //}

    //private void OnDisable()
    //{
    //    player.blastCharge += 0.1f;     //�arj�n d��man �l�m� ile artmas�
    //}
}
