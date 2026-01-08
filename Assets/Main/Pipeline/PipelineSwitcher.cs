using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public enum DevicePerformance
{
    Low = 1,
    Medium = 2,
    High = 3,
}

public class PipelineSwitcher : MonoBehaviour
{
    public UniversalRenderPipelineAsset highPipelineAsset;
    public UniversalRenderPipelineAsset mediumPipelineAsset;
    public UniversalRenderPipelineAsset lowPipelineAsset;

    public UniversalRendererData highPipelineRendererData;
    public UniversalRendererData mediumPipelineRendererData;
    public UniversalRendererData lowPipelineRendererData;

    private RenderTextureDescriptor descriptor;

    void Start()
    {
        SetPipelineBasedOnPerformance();
        GraphicsSettings.renderPipelineAsset = mediumPipelineAsset;
        GraphicsSettings.defaultRenderPipeline = mediumPipelineAsset;

        SetRenderFeatureActive(mediumPipelineRendererData, "CatchOpaqueRT", false); // 开启指定的RenderFeature
    }

    void SetPipelineBasedOnPerformance()
    {
        var devicePerformance = GetDevicePerformance();

        switch (devicePerformance)
        {
            case DevicePerformance.High:
                Debug.Log("Setting High Pipeline");
                GraphicsSettings.renderPipelineAsset = highPipelineAsset;
                break;
            case DevicePerformance.Medium:
                Debug.Log("Setting Medium Pipeline");
                GraphicsSettings.renderPipelineAsset = mediumPipelineAsset;
                break;
            case DevicePerformance.Low:
                Debug.Log("Setting Low Pipeline");
                GraphicsSettings.renderPipelineAsset = lowPipelineAsset;
                break;
            default:
                Debug.LogWarning("Unknown device performance, defaulting to Medium Pipeline");
                GraphicsSettings.renderPipelineAsset = mediumPipelineAsset;
                break;
        }
    }

    void SetRenderFeatureActive(UniversalRendererData rendererData, string featureName, bool active)
    {
        foreach (var feature in rendererData.rendererFeatures)
        {
            if (feature.name == featureName)
            {
                feature.SetActive(active);
                return;
            }
        }
    }

    public DevicePerformance GetDevicePerformance()
    {
        int retDP = (int)DevicePerformance.High;
        int antiAliasingLevel = QualitySettings.antiAliasing;

        string deviceModel = SystemInfo.deviceModel;
        string operatingSystem = SystemInfo.operatingSystem;
        string graphicsDeviceName = SystemInfo.graphicsDeviceName;
        int graphicsDeviceID = SystemInfo.graphicsDeviceID;
        GraphicsDeviceType graphicsAPI = SystemInfo.graphicsDeviceType;
        string graphicsDeviceVendor = SystemInfo.graphicsDeviceVendor;
        string graphicsDeviceVersion = SystemInfo.graphicsDeviceVersion;
        int supportedRenderTargetCount = SystemInfo.supportedRenderTargetCount;
        int maxTextureSize = SystemInfo.maxTextureSize;
        int graphicsMemorySize = SystemInfo.graphicsMemorySize;
        int systemMemorySize = SystemInfo.systemMemorySize;
        bool supportAstc = SystemInfo.SupportsTextureFormat(TextureFormat.ASTC_4x4);

        Debug.Log("设备信息 DeviceModel: " + deviceModel);
        Debug.Log("设备信息 OperatingSystem: " + operatingSystem);
              
        Debug.Log("设备信息 graphicsDeviceName: " + graphicsDeviceName);
        Debug.Log("设备信息 graphicsDeviceID: " + graphicsDeviceID);
        Debug.Log("设备信息 graphicsDeviceType: " + SystemInfo.graphicsDeviceType);
        Debug.Log("设备信息 graphicsDeviceVendor: " + graphicsDeviceVendor);
        Debug.Log("设备信息 graphicsDeviceVersion: " + graphicsDeviceVersion);
              
        Debug.Log("设备信息 supportedRenderTargetCount: " + supportedRenderTargetCount);
        Debug.Log("设备信息 maxTextureSize: " + maxTextureSize);
        Debug.Log("设备信息 systemMemorySize: " + systemMemorySize);
        Debug.Log("设备信息 graphicsMemorySize: " + graphicsMemorySize);
        Debug.Log("设备信息 supportAstc: " + supportAstc);
        Debug.Log("设备信息 antiAliasingLevel: " + antiAliasingLevel);
        Debug.Log("设备信息 graphicsShaderLevel: " + SystemInfo.graphicsShaderLevel);
        Debug.Log("设备信息 supportsMultisampledTextures: " + SystemInfo.supportsMultisampledTextures);
        Debug.Log("设备信息 GetRenderTextureSupportedMSAASampleCount: " + SystemInfo.GetRenderTextureSupportedMSAASampleCount(descriptor));
        Debug.Log("设备信息 processorFrequency" + SystemInfo.processorFrequency);


        if (Application.platform == RuntimePlatform.Android)
        {
            if (!supportAstc)
            {
                return DevicePerformance.Low;
            }

            if (maxTextureSize < 4096)
            {
                return DevicePerformance.Low;
            }
            else if (maxTextureSize < 16384)
            {
                retDP = Mathf.Min(retDP, (int)DevicePerformance.Medium);
            }
            else
            {
                retDP = Mathf.Min(retDP, (int)DevicePerformance.High);
            }

            //CPU核心数
            if (SystemInfo.processorCount <= 4)
            {
                return DevicePerformance.Low;
            }


            if (SystemInfo.graphicsDeviceVendorID == 32902)
            {
                //集显
                retDP = Mathf.Min(retDP, (int)DevicePerformance.Low);
            }


            //显存和内存
            if (graphicsMemorySize < 2000 && systemMemorySize < 4000)
            {
                return DevicePerformance.Low;
            }
            else if (graphicsMemorySize < 6000 && systemMemorySize < 8000)
            {
                retDP = Mathf.Min(retDP, (int)DevicePerformance.Medium);
            }
            else
            {
                retDP = Mathf.Min(retDP, (int)DevicePerformance.High);
            }


            // 渲染目标数量小于4	
            if (supportedRenderTargetCount < 4)
            {
                return DevicePerformance.Low;
            }
            // 渲染目标数量4到7
            else if (supportedRenderTargetCount < 8)
            {
                retDP = Mathf.Min(retDP, (int)DevicePerformance.Medium);
            }
            // 渲染目标数量大于8
            else
            {
                retDP = Mathf.Min(retDP, (int)DevicePerformance.High);
            }


            if (SystemInfo.processorFrequency < 1800)
            {
                return DevicePerformance.Low;
            }
            else if (SystemInfo.processorFrequency < 3200)
            {
                retDP = Mathf.Min(retDP, (int)DevicePerformance.Medium);
            }
            else
            {
                retDP = Mathf.Min(retDP, (int)DevicePerformance.High);
            }


        }
        else
        {
            return DevicePerformance.High;
        }
        return (DevicePerformance)retDP;
    }
}