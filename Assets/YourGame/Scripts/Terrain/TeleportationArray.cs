using UnityEngine;
using System.Collections;
using PlayerInputAction;
public class TeleportationArray : MonoBehaviour
{
    [SerializeField] private MapZoneData targetZone;
    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private string startupItemName;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    private Animator _animator => GetComponent<Animator>();

    private Coroutine teleportCoroutine;
    private bool isFirst = true;

    private bool isTeleporting = false;
    private void Start()
    {
        _spriteRenderer.color = new Color(0, 0, 0, 0.5f); 
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isTeleporting) return;

        if (other.CompareTag("Player"))
        {
            if (InventoryItemManager.Instance.haveItem(startupItemName))
            {
                _spriteRenderer.color = new Color(1, 1, 1, 0.8f);
                isTeleporting = true;
                teleportCoroutine = StartCoroutine(TeleportCoroutine());
            }
            else if(isFirst)GuidanceSystem.Instance.TriggerNode("HintTeleportationArray");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isTeleporting = false;
            if (teleportCoroutine != null)
            {
                StopCoroutine(teleportCoroutine);
                teleportCoroutine = null;
            }
        }
    }

    private IEnumerator TeleportCoroutine()
    {
        yield return new WaitForSeconds(3f);
        isTeleporting = true;
        PlayerController.Instance.FreezePlayer();
    
        // 播放 TeleportRun 動畫
        _animator.CrossFade("TeleportRun", 0.1f);
    
        // 呼叫 MapZoneManager 預載入（非 Activate）
        MapZoneManager.Instance.PreloadTeleportZone(targetZone);
        yield return new WaitForSeconds(1f);
        // 等待動畫結束
        AnimatorStateInfo stateInfo;
        do
        {
            stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            yield return null; // 等待下一幀
        } while (stateInfo.IsName("TeleportRun") && stateInfo.normalizedTime < 1f);
    
        // 動畫結束後解除玩家凍結
        PlayerController.Instance.UnFreezePlayer();
    
        // 觸發動畫結束事件
        OnAnimationEnd();
    }

    // 由動畫事件觸發
    public void OnAnimationEnd()
    {
        
        isTeleporting = false;
        teleportCoroutine = null;
        // 正式切換地圖區塊並將玩家傳送
        MapZoneManager.Instance.ActivateTeleportZone(targetZone, targetPosition);
    }
}
