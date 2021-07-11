using NUnit.Framework;
using UnityEngine;


public class EditTests
{
    [Test]
    public void ColorRed()
    {
        Assert.AreEqual(new Color(1,0,0,1), Color.red);
    }
}
