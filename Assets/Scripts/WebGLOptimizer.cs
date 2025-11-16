using UnityEngine;

/// <summary>
/// WebGL-specific optimizations and configurations.
/// Attach to a GameObject in your first scene or main menu.
/// </summary>
public class WebGLOptimizer : MonoBehaviour
{
    [Header("WebGL Settings")]
    [Tooltip("Show loading progress in console")]
    public bool showLoadingProgress = true;
    
    [Tooltip("Optimize for web performance")]
    public bool optimizeForWeb = true;

    private void Awake()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        ApplyWebGLOptimizations();
#endif
    }

    private void ApplyWebGLOptimizations()
    {
        // Set target frame rate for web (60 FPS is good for most browsers)
        Application.targetFrameRate = 60;
        
        // Disable VSync on WebGL (browser handles this)
        QualitySettings.vSyncCount = 0;
        
        // Reduce quality for better performance on web
        if (optimizeForWeb)
        {
            QualitySettings.shadows = ShadowQuality.Disable;
            QualitySettings.SetQualityLevel(2, true); // Medium quality
        }
        
        Debug.Log("🌐 WebGL optimizations applied");
        Debug.Log($"   - Target FPS: {Application.targetFrameRate}");
        Debug.Log($"   - Quality Level: {QualitySettings.GetQualityLevel()}");
    }

    /// <summary>
    /// Check if running on WebGL platform
    /// </summary>
    public static bool IsWebGL()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        return true;
#else
        return false;
#endif
    }

    /// <summary>
    /// Open a URL in a new browser tab (WebGL only)
    /// </summary>
    public static void OpenURL(string url)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        Application.ExternalEval($"window.open('{url}', '_blank');");
#else
        Application.OpenURL(url);
#endif
    }

    /// <summary>
    /// Check if the browser supports required features
    /// </summary>
    private void Start()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        CheckBrowserCompatibility();
#endif
    }

    private void CheckBrowserCompatibility()
    {
        // Check WebGL support
        if (!SystemInfo.supportsComputeShaders)
        {
            Debug.LogWarning("⚠️ Browser may not fully support WebGL 2.0");
        }
        
        Debug.Log("🌐 Browser Info:");
        Debug.Log($"   - OS: {SystemInfo.operatingSystem}");
        Debug.Log($"   - GPU: {SystemInfo.graphicsDeviceName}");
        Debug.Log($"   - Memory: {SystemInfo.systemMemorySize} MB");
    }
}
