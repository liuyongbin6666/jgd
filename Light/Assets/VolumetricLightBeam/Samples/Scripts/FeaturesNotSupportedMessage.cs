using UnityEngine;
using VolumetricLightBeam.Scripts;

namespace VolumetricLightBeam.Samples.Scripts
{
    public class FeaturesNotSupportedMessage : MonoBehaviour
    {
        void Start()
        {
            if(!Noise3D.isSupported)
                Debug.LogWarning(Noise3D.isNotSupportedString);
        }
    }
}
