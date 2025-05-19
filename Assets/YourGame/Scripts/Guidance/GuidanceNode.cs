using UnityEngine;

public class GuidanceNode : MonoBehaviour
{
    public string nodeId;
    public bool IsOnce = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (IsOnce)
            {
                GuidanceSystem.Instance.TriggerNode(nodeId);
                Destroy(gameObject);
            }
            else
            {
                GuidanceSystem.Instance.TriggerNode(nodeId);
            }
        }
    }

    public void TriggerNode()
    {
        GuidanceSystem.Instance.TriggerNode(nodeId);
    }
}
