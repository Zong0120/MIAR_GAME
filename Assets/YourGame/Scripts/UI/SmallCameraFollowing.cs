using UnityEngine;

public class SmallCameraFollowing : MonoBehaviour
{
    [SerializeField] private Vector2 minBounds;
    [SerializeField] private Vector2 maxBounds;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);
    [SerializeField] private Transform player;

    void LateUpdate()
    {
        Vector3 targetPos = player.position + offset;

        float clampedX = Mathf.Clamp(targetPos.x, minBounds.x, maxBounds.x);
        float clampedY = Mathf.Clamp(targetPos.y, minBounds.y, maxBounds.y);

        transform.position = new Vector3(clampedX, clampedY, targetPos.z);
    }
}
