using UnityEngine;

public class InheritanceBoxAnimation : MonoBehaviour
{
    
    public void OnBoxAnimationStart()
    {
        InheritanceManager.Instance.ShowCanvas();
    }
    public void OnBoxAnimationEnd()
    {
        InheritanceManager.Instance.CloseCanvas();
    }
}
