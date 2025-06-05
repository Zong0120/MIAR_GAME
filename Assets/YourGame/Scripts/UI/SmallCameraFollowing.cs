using System.Collections;
using UnityEngine;

public class SmallCameraFollowing : MonoBehaviour
{
    public static SmallCameraFollowing Instance { get; private set; }
    [SerializeField] private Vector2 minBounds;
    [SerializeField] private Vector2 maxBounds;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);
    [SerializeField] private Transform player;

    private bool isFollowing = true;

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
    void LateUpdate()
    {
        if (!isFollowing) return;
        Vector3 targetPos = player.position + offset;

        float clampedX = Mathf.Clamp(targetPos.x, minBounds.x, maxBounds.x);
        float clampedY = Mathf.Clamp(targetPos.y, minBounds.y, maxBounds.y);

        transform.position = new Vector3(clampedX, clampedY, targetPos.z);
    }

    public void MoveSmallCamera(Vector3 targetPos, float duration)
    {
        StartCoroutine(MoveToPosition(targetPos, duration));
    }

    public IEnumerator MoveToPosition(Vector3 targetPos, float duration)
    {
        isFollowing = false;
        Vector3 startPos = transform.position;
        float elapsedTime = 0;
        float clampedX = Mathf.Clamp(targetPos.x, minBounds.x, maxBounds.x);
        float clampedY = Mathf.Clamp(targetPos.y, minBounds.y, maxBounds.y);

        targetPos = new Vector3(clampedX, clampedY, -181);
        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;

        yield return new WaitForSeconds(4f);

        isFollowing = true;
    }
}
