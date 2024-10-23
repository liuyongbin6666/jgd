using UnityEngine;

namespace Utls
{
    public static class ColliderExtension
    {
        // 检查某个Transform是否在Collider范围内
        public static bool IsInRange(this SphereCollider sphereCollider, Transform targetTransform)
        {
            // 获取SphereCollider的半径并考虑Scale
            var scaledRadius = sphereCollider.radius * Mathf.Max(sphereCollider.transform.lossyScale.x,
                sphereCollider.transform.lossyScale.y, sphereCollider.transform.lossyScale.z);
            var sphereCenter = sphereCollider.transform.position + sphereCollider.center;

            // 计算与中心点的距离
            var distance = Vector3.Distance(sphereCenter, targetTransform.position);
            return distance <= scaledRadius;
        }

        public static bool IsInRange(this BoxCollider boxCollider, Transform targetTransform)
        {
            // 获取BoxCollider的大小并考虑Scale
            var halfSize = Vector3.Scale(boxCollider.size / 2, boxCollider.transform.lossyScale);
            var boxCenter = boxCollider.transform.position + boxCollider.center;

            // 计算世界坐标中的范围
            var bounds = new Bounds(boxCenter, halfSize * 2);
            return bounds.Contains(targetTransform.position);
        }
    }
}