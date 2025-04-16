using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FixCameraRenderer : MonoBehaviour
{
    void Start()
    {
        var camera = GetComponent<Camera>();
        if (camera != null)
        {
            var cameraData = camera.GetUniversalAdditionalCameraData();
            if (cameraData != null)
            {
                // 確保渲染器索引有效
                cameraData.SetRenderer(0); // 將索引設置為 0（或其他有效索引）
                Debug.Log($"Renderer index set to {cameraData.scriptableRenderer}");
            }
        }
    }
}