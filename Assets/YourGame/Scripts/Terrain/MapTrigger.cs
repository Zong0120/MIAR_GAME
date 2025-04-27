using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapTrigger : MonoBehaviour
{
    [SerializeField]private MapZoneData triggerZone;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered trigger zone: " + triggerZone.zoneName);
            Object.FindFirstObjectByType<MapZoneManager>().LoadAndActivateZone(triggerZone);
        }
    }

}