using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaBlast : MonoBehaviour
{
    private void OnDisable()
    {
        GetComponentInParent<Player>().blastCharge = 0f;  //�arj�n par�ac�k efekti bittikten sonra s�f�rlanmas�
    }
}
