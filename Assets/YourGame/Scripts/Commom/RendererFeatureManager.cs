using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using System.Collections.Generic;
using System.Collections;

public class RendererFeatureManager : MonoBehaviour
{
    public static RendererFeatureManager Instance;

    private new ScriptableRenderer renderer;

    private Dictionary<string, ScriptableRendererFeature> featureDict = new();

    [SerializeField]private Material _GlitchMaterial,_GaryMaterial;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        /*
        // 🎯 拿到 Main Camera 當前實際運行中的 Renderer 實例
        var cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("Main Camera not found.");
            return;
        }

        var camData = cam.GetUniversalAdditionalCameraData();
        renderer = camData.scriptableRenderer;

        if (renderer == null)
        {
            Debug.LogError("Renderer is null.");
            return;
        }

        var rendererFeaturesField = typeof(ScriptableRenderer).GetField("rendererFeatures", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (rendererFeaturesField != null)
        {
            var features = rendererFeaturesField.GetValue(renderer) as List<ScriptableRendererFeature>;
            foreach (var feature in features)
            {
                if (feature != null && !string.IsNullOrEmpty(feature.name))
                {
                    featureDict[feature.name] = feature;
                    feature.SetActive(false); // ✅ 關閉實例中的 Feature
                }
            }
        }
        else
        {
            Debug.LogError("Unable to access renderer features.");
        }

        Debug.Log($"RendererFeatureManager initialized with {featureDict.Count} features.");
        */
    }



    /// <summary>
    /// 啟用/停用特定 Feature
    /// </summary>
    public void SetFeatureActive(string featureName, bool active)
    {
        if (featureDict.TryGetValue(featureName, out var feature))
        {
            feature.SetActive(active);
        }
        else
        {
            Debug.LogWarning($"Feature {featureName} not found.");
        }
    }

    /// <summary>
    /// feature effect function
    /// </summary>

    public void DeathEffect()
    {
        _GaryMaterial.SetFloat("_GrayscaleIntensity", 0);
        //SetFeatureActive("GaryEffect", true);
        StartCoroutine(GrayEffect(2));
    }
    private IEnumerator GrayEffect(float time)
    {
        float t = 0;
        while (t < time)
        {
            t += Time.deltaTime;
            _GaryMaterial.SetFloat("_GrayscaleIntensity", Mathf.Lerp(0, 1, t / time));
            yield return null;
        }
    }
    public void DeathEnd()
    {
        //SetFeatureActive("GaryEffect", false);
        _GaryMaterial.SetFloat("_GrayscaleIntensity", 0);
    }

    public bool IsFeatureActive(string featureName)
    {
        return featureDict.TryGetValue(featureName, out var feature) && feature.isActive;
    }
}
