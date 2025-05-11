using UnityEngine;
public class TeleportationArray : MonoBehaviour
{
    [SerializeField] private MapZoneData targetZone;
    [SerializeField] private Transform targetPosition;
    private Animator _animator => GetComponent<Animator>();

    private bool isTeleporting = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isTeleporting) return;

        if (other.CompareTag("Player"))
        {
            isTeleporting = true;

            // 播放動畫（假設你有個動畫控制器可用）
            _animator.CrossFade("TeleportRun", 5f);

            // 呼叫 MapZoneManager 預載入（非 Activate）
            MapZoneManager.Instance.PreloadTeleportZone(targetZone);
        }
    }

    // 由動畫事件觸發
    public void OnAnimationEnd()
    {
        // 正式切換地圖區塊並將玩家傳送
        MapZoneManager.Instance.ActivateTeleportZone(targetZone, targetPosition.position);
    }
}
