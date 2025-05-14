using UnityEngine;
using System.Collections;

public class DoorLockManager : MonoBehaviour
{
    [SerializeField]private DoorLockData[] doorLockData;

    [SerializeField]private float LockProbability = 0.5f; // 鎖住的機率

    private void Start()
    {
        foreach (var doorLock in doorLockData)
        {
            StartCoroutine(InitLockDoor(doorLock));
        }
    }

    private IEnumerator InitLockDoor(DoorLockData doorLock)
    {
        if(Random.value < LockProbability)
        {
            //doorLock.OnSmallMap.SetActive(true);
        }
        else
        {
            Destroy(doorLock.doorObject);
        }
        yield return new WaitForSeconds(0.5f);
    }
}

[System.Serializable]
public class DoorLockData
{
    public string doorName;
    public GameObject doorObject;
    public GameObject OnSmallMap;
}