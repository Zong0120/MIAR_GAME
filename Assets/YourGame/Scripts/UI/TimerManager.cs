using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimerManager : MonoBehaviour
{
    public static TimerManager Instance { get; private set; }
    private int seconds;
    [SerializeField] private int min;
    [SerializeField] private int sec;
    public TextMeshProUGUI time;
    public bool isTimePause { get; set; } = false;
    public bool BagTimePause { get; set; } = false;
    //[SerializeField] private Material _Material;
    //private float _Offset=0;
    private int origin_min;
    private int time_threshold;

    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(CountDown());
        /*
        if(_Material!=null)
        {
            _Material.SetFloat("_Blur_Offset",0);
            origin_min = min-1;
        }
        */
        origin_min = min;
        time_threshold = origin_min / 2;
    }

    IEnumerator CountDown()
    {
        time.text = string.Format("{0}:{1}", min.ToString("00"), sec.ToString("00"));
        seconds = min * 60 + sec;

        while (seconds > 0)
        {
            if (isTimePause || BagTimePause)
            {
                yield return null;
            }
            else
            {
                yield return new WaitForSeconds(1f);

                seconds--;
                sec--;

                if (sec < 0 && min > 0)
                {
                    min--;
                    sec = 59;
                    if (min == 1 || min == 2 || min == 4 || min == 7)
                        GhostEnemy.Instance.Activate();
                    //NegativeState();
                }
                else if (sec < 0 && min == 0)
                {
                    sec = 0;
                }

                time.text = string.Format("{0}:{1}", min.ToString("00"), sec.ToString("00"));
            }
        }
        yield return new WaitForSeconds(1f);
        HealthManager.Instance.DirectDeath();

        //Health.Instance.PlayerDeath();
    }

    public void MoreTime()
    {
        seconds *= 5;
        seconds /= 2;
        if (seconds > 900) seconds = 900;
        min = seconds / 60;
        sec = seconds % 60;
        //NegativeState();
    }

    public void ReduceHalfTime()
    {
        seconds /= 2;
        min = seconds / 60;
        sec = seconds % 60;
        //NegativeState();
    }
    public void ReduceQuarterTime()
    {
        seconds = seconds * 3 / 4;
        min = seconds / 60;
        sec = seconds % 60;
        //NegativeState();
    }

    public void AddTime(int _time)
    {
        if (seconds > 900) seconds = 900;
        seconds += _time;
        min = seconds / 60;
        sec = seconds % 60;
        //NegativeState();
    }
    /*
    private void NegativeState()
    {
        if(min <= time_threshold)
        {
            _Offset = (origin_min-min-time_threshold)*0.001f;
            _Material.SetFloat("_Blur_Offset",_Offset);
        }
        else
        {
            _Material.SetFloat("_Blur_Offset",0);
        }
    }
    */
}
