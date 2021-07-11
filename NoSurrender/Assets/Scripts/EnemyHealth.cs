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

    private void OnTriggerEnter(Collider other)   //Collider bazlý saldýrýlar için
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
            player.blastCharge += 0.1f;     //Þarjýn düþman ölümü ile artmasý
        }        
    }    

    private void OnParticleCollision(GameObject other) //Parçacýk bazlý saldýrýlar için
    {
        Destroy(gameObject);
        player.blastCharge += 0.1f;     //Þarjýn düþman ölümü ile artmasý
    }

    //private void OnDestroy()
    //{
    //    player.blastCharge += 0.1f;     //Þarjýn düþman ölümü ile artmasý
    //}

    //private void OnDisable()
    //{
    //    player.blastCharge += 0.1f;     //Þarjýn düþman ölümü ile artmasý
    //}
}
