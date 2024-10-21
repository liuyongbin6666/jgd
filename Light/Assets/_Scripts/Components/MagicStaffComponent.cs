using Sirenix.OdinInspector;
using UnityEngine;

public class MagicStaffComponent : MonoBehaviour
{
    [SerializeField,LabelText("内光")] ParticleSystem innerPar;
    [SerializeField,LabelText("散光")] ParticleSystem glow;

    [Button("开启")]public void SetActive(bool active)
    {
        innerPar.gameObject.SetActive(active);
        glow.gameObject.SetActive(active);
    }
}