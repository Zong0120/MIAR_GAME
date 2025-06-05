using UnityEngine;

public class CollToStart : MonoBehaviour
{
    public bool isStart{ get; set; }=true;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (isStart)
            {
                GuidanceSystem.Instance.WrongEnding();

            }
            else
                VideoManager.Instance.PlayClearGameVideo();
            gameObject.SetActive(false);
        }
    }
}
