using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayTests
{    
    [UnityTest]
    public IEnumerator PlayTestsWithEnumeratorPasses()
    {
        GameObject testObject = GameObject.Instantiate(new GameObject());
        PowerUp powerUp = testObject.AddComponent<PowerUp>();

        Quaternion rotation = powerUp.transform.rotation;        

        yield return new WaitForSeconds (1f);

        Quaternion newRotation = powerUp.transform.rotation;
        
        Assert.AreNotEqual(newRotation, rotation);
    }
}
