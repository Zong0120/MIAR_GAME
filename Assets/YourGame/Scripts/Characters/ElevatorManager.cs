using System.Collections.Generic;
using UnityEngine;

public class ElevatorManager : MonoBehaviour
{
    public static ElevatorManager Instance { get; private set; }

    public List<GameObject> level1, leve2 = new List<GameObject>(3);
    public List<GameObject> level1coll, leve2coll = new List<GameObject>(3);
    public List<GameObject> level1Ui, leve2Ui = new List<GameObject>(3);

    void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        Instance = this;
    }

    private void Start()
    {
        int randomIndex = Random.Range(0, level1.Count-1);
        for (int i = 0; i < level1.Count; i++)
        {
            if (i != randomIndex)
            {
                Destroy(level1[i]);
                Destroy(level1Ui[i]);
            }
            else Destroy(level1coll[i]);
        }
        int randomIndex2 = Random.Range(0, leve2.Count-1);
        for (int i = 0; i < leve2.Count; i++)
        {
            if (i != randomIndex2)
            {
                Destroy(leve2[i]);
                Destroy(leve2Ui[i]);
            }
            else Destroy(leve2coll[i]);
        }
        MapZoneManager.Instance._1FZoneObjects.Add(level1[randomIndex]);
        MapZoneManager.Instance._2FZoneObjects.Add(leve2[randomIndex2]);
        if (MapZoneManager.Instance.currentZoneLevel == '1')
        {
            leve2[randomIndex2].SetActive(false);
            level1[randomIndex].SetActive(true);
        }
        else
        {
            level1[randomIndex].SetActive(false);
            leve2[randomIndex2].SetActive(true);
        }
    }
}