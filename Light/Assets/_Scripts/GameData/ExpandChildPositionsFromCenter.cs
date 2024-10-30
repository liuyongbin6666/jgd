using Sirenix.OdinInspector;
using UnityEngine;

namespace Utls
{
    public class ExpandChildPositionsFromCenter : MonoBehaviour
    {
        public Transform parentTransform; // 父物体
        public float scaleFactor = 1.5f;  // 缩放因子

        [Button]void Adjust()
        {
            // 如果未指定父物体，默认使用当前物体
            if (parentTransform == null)
            {
                parentTransform = this.transform;
            }

            // 调整子物体的位置
            AdjustChildPositionsFromCenter();
        }

        void AdjustChildPositionsFromCenter()
        {
            Vector3 center = Vector3.zero;
            int count = 0;

            foreach (Transform child in parentTransform)
            {
                center += child.localPosition;
                count++;
            }

            center /= count; // 计算中心点

            foreach (Transform child in parentTransform)
            {
                Vector3 offset = child.localPosition - center;
                offset.x *= scaleFactor;
                offset.z *= scaleFactor;
                child.localPosition = center + offset;
            }
        }
    }
}