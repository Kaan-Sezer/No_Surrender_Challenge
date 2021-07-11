using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField] PowerUpType powerUpType; 
    
    void Update()
    {
        //Power Up animasyonu
        transform.Rotate(0, 120 * Time.deltaTime, 0);  //Yerinde dönme

        Vector3 pos = transform.position;              //Yukarý aþaðý hareket
        pos.y = Mathf.PingPong(Time.time * 0.5f, 0.5f) + 1f;
        transform.position = pos;
    }

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponentInParent<Player>();

        if (!player.powerUpActive)                //Karakterdeki aktif power up'ýn kontrolü
        {
            if (other.gameObject.CompareTag("Body"))          //Diðer colliderlarla çalýþmamasý için karakter vücudunun kontrolü
            {
                if (powerUpType == PowerUpType.Laser)
                {
                    player.ActivatePowerUp(player.laser);     //Karakterdeki Lazer Power Up'ýnýn aktif edilmesi
                }

                if (powerUpType == PowerUpType.Shotgun)
                {
                    player.ActivatePowerUp(player.shotgun);   //Karakterdeki Shotgun Power Up'ýnýn aktif edilmesi
                }

                Destroy(gameObject);                          
            }
        }
    }
}
