#if UNITY_EDITOR && UNITY_2018_1_OR_NEWER
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using VolumetricLightBeam.Scripts;
using VolumetricLightBeam.Scripts.SD;

namespace VLB
{
    public class ActiveBuildTargetListener : IActiveBuildTargetChanged
    {
        public int callbackOrder { get { return 0; } }
        public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
        {
            VolumetricLightBeam.Scripts.Config.Instance.RefreshShaders(VolumetricLightBeam.Scripts.Config.RefreshShaderFlags.All);
            VolumetricLightBeam.Scripts.Config.Instance.SetScriptingDefineSymbolsForCurrentRenderPipeline();
            GlobalMeshSD.Destroy();
            Utils._EditorSetAllMeshesDirty();
        }
    }
}
#endif
