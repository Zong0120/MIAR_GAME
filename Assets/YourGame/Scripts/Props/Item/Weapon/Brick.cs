using System.Collections;
using UnityEngine;

public class Brick : WeaponItem
{
    void OnEnable()
    {
        HealthManager.Instance.expansionMax(1);
    }
    private void OnDisable() {
        HealthManager.Instance.reduceMax(1);
    }

}
