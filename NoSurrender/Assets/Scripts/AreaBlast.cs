using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaBlast : MonoBehaviour
{
    private void OnDisable()
    {
        GetComponentInParent<Player>().blastCharge = 0f;  //Þarjýn parçacýk efekti bittikten sonra sýfýrlanmasý
    }
}
