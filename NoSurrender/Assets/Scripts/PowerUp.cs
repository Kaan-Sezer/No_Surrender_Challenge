using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField] PowerUpType powerUpType; 
    
    void Update()
    {
        //Power Up animasyonu
        transform.Rotate(0, 120 * Time.deltaTime, 0);  //Yerinde d�nme

        Vector3 pos = transform.position;              //Yukar� a�a�� hareket
        pos.y = Mathf.PingPong(Time.time * 0.5f, 0.5f) + 1f;
        transform.position = pos;
    }

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponentInParent<Player>();

        if (!player.powerUpActive)                //Karakterdeki aktif power up'�n kontrol�
        {
            if (other.gameObject.CompareTag("Body"))          //Di�er colliderlarla �al��mamas� i�in karakter v�cudunun kontrol�
            {
                if (powerUpType == PowerUpType.Laser)
                {
                    player.ActivatePowerUp(player.laser);     //Karakterdeki Lazer Power Up'�n�n aktif edilmesi
                }

                if (powerUpType == PowerUpType.Shotgun)
                {
                    player.ActivatePowerUp(player.shotgun);   //Karakterdeki Shotgun Power Up'�n�n aktif edilmesi
                }

                Destroy(gameObject);                          
            }
        }
    }
}
