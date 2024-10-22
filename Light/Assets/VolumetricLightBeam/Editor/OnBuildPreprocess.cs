#if UNITY_EDITOR && UNITY_2018_1_OR_NEWER
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using VolumetricLightBeam.Scripts;

class OnBuildPreprocess : IPreprocessBuildWithReport
{
    public int callbackOrder { get { return 0; } }
    public void OnPreprocessBuild(BuildReport report)
    {
        PlatformHelper.SetBuildTargetOverride(report.summary.platform);
        VolumetricLightBeam.Scripts.Config.Instance.SetScriptingDefineSymbolsForCurrentRenderPipeline();
    }
}
#endif
