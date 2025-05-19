using UnityEngine;
using System.Collections;

public class DoorLockManager : MonoBehaviour
{
    public static DoorLockManager Instance;
    [SerializeField]private DoorLockData[] doorLockData;

    [SerializeField]private float LockProbability = 0.5f; // 鎖住的機率

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

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
            doorLock.OnSmallMap.SetActive(true);
        }
        else
        {
            Destroy(doorLock.doorObject.gameObject);
            Destroy(doorLock.OnSmallMap.gameObject);
        }
        yield return new WaitForSeconds(0.5f);
    }

    public void UnlockDoor(string doorName)
    {
        foreach (var doorLock in doorLockData)
        {
            if (doorLock.doorName == doorName)
            {
                Destroy(doorLock.OnSmallMap.gameObject);
                break;
            }
        }
    }
}

[System.Serializable]
public class DoorLockData
{
    public string doorName;
    public GameObject doorObject;
    public GameObject OnSmallMap;
}