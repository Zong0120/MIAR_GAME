using UnityEngine;
using System.Collections;

public class Elevator : MonoBehaviour
{
    [SerializeField] private Transform playposition;
    public Vector3 startPosition;
    public Vector3 endPosition;
    //[SerializeField] private GameObject FixedColl;
    [SerializeField] private GameObject TargetRoom1;
    [SerializeField] private GameObject TargetRoom2;

    [SerializeField] private Animator _animator;

    private Coroutine moveCoroutine;
    private Coroutine cancelCoroutine;
    private bool isGoToTarget = true;
    private GameObject targetRoom;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (cancelCoroutine != null) return;
            targetRoom = StoryManager.Instance.ChaptersCollectionComplete() ? TargetRoom2 : TargetRoom1;
            if (transform.position.y == startPosition.y)
                isGoToTarget = true;
            else isGoToTarget = false;
            moveCoroutine = StartCoroutine(MoveElevator());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
                moveCoroutine = null;
                // 播放 Cancel 動畫
                cancelCoroutine = StartCoroutine(CancelDoorClose(_animator.GetCurrentAnimatorStateInfo(0)));

            }
        }
    }

    private IEnumerator MoveElevator()
    {
        yield return new WaitForSeconds(3f);
        // 播放 Close 動畫
        _animator.CrossFade("DoorClose", 0.1f);
        yield return new WaitForSeconds(0.5f);
        // 等待關門動畫結束
        AnimatorStateInfo stateInfo;
        do
        {
            stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            yield return null; // 等待下一幀
        } while (stateInfo.IsName("DoorClose") && stateInfo.normalizedTime < 1f);
        StartCoroutine(WaitDoorOpen(_animator.GetCurrentAnimatorStateInfo(0)));

        if (isGoToTarget)
        {
            TimerManager.Instance.isTimePause = true;
            targetRoom.SetActive(true);
            MapZoneManager.Instance.TemporarilyHideCurrentZone();
        }
        else
        {
            TimerManager.Instance.isTimePause = false;
            MapZoneManager.Instance.ReactivateCurrentZone();
            targetRoom.SetActive(false);
        }

        // 開始移動電梯
        Vector3 start = transform.position;
        Vector3 end = isGoToTarget ? endPosition : startPosition;
        Vector3 playpositionStart = playposition.position;
        Vector3 playpositionEnd = new Vector3(playposition.position.x, end.y, playposition.position.z);
        float elapsedTime = 0;
        while (elapsedTime < 3f)
        {
            transform.position = Vector3.Lerp(start, end, elapsedTime);
            playposition.position = Vector3.Lerp(playpositionStart, playpositionEnd, elapsedTime);
            elapsedTime += Time.deltaTime;
            yield return null; // 等待下一幀
        }
        // 確保電梯到達起始位置
        transform.position = end;
        // 播放 Open 動畫
        _animator.CrossFade("DoorOpen", 0.1f);
        yield return new WaitForSeconds(0.5f);
        // 等待關門動畫結束
        do
        {
            stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            yield return null; // 等待下一幀
        } while (stateInfo.IsName("DoorOpen") && stateInfo.normalizedTime < 1f);
        moveCoroutine = null;
    }

    private IEnumerator WaitDoorOpen(AnimatorStateInfo stateInfo)
    {
        while (stateInfo.IsName("DoorClose"))
        {
            stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            yield return null; // 等待下一幀
        }
    }

    private IEnumerator CancelDoorClose(AnimatorStateInfo stateInfo)
    {
        _animator.CrossFade("DoorOpen", 0.1f);
        yield return new WaitForSeconds(0.5f);
        // 等待關門動畫結束
        do
        {
            stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            yield return null; // 等待下一幀
        } while (stateInfo.IsName("DoorOpen") && stateInfo.normalizedTime < 1f);
        cancelCoroutine = null;
    }
}
