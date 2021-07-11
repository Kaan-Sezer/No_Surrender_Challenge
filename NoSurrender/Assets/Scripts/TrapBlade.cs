using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapBlade : MonoBehaviour
{
    [SerializeField] float rotationSpeed;
    [SerializeField] float moveSpeed;
    [SerializeField] float moveRange;
    
    void Update()
    {
        //Yerinde dönme
        transform.Rotate(0, 0, -rotationSpeed * Time.deltaTime);

        //Ýleri geri hareket
        Vector3 pos = transform.localPosition;
        pos.x = Mathf.PingPong(Time.time * moveSpeed, moveRange) - moveRange/2;
        transform.localPosition = pos;
    }
}
