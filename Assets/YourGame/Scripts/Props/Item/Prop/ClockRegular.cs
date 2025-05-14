using UnityEngine;

public class ClockRegular : PropItem
{
    public override void UseEffect()
    {
        TimerManager.Instance.AddTime(30);
    }
}
