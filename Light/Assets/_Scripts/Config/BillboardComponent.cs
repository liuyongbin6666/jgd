using Sirenix.OdinInspector;
using UnityEngine;

public class BillboardComponent : MonoBehaviour
{
    [SerializeField] Camera _camera;

    [Button("设定2D子物件适配摄像头")]
    public void FaceToCamera()
    {
        // 获取主摄像头的 Transform
        if (_camera == null)
        {
            Debug.LogError("主摄像头未找到！");
            return;
        }

        Transform cameraTransform = _camera.transform;

        // 遍历所有子物体（递归）
        RotateAllChildren(transform, cameraTransform.localEulerAngles.x);
        Debug.Log("子物体角度已适配主摄像头。");
    }

    // 递归方法，遍历所有子物体并旋转带有SpriteRenderer的物体
    void RotateAllChildren(Transform parent, float cameraRotationX)
    {
        foreach (Transform child in parent)
        {
            // 检查是否有SpriteRenderer组件
            if (child.GetComponent<SpriteRenderer>() != null)
            {
                // 旋转子物体的X轴
                child.localEulerAngles = child.localEulerAngles.ChangeX(cameraRotationX);
            }

            // 递归调用，遍历所有下层子物体
            RotateAllChildren(child, cameraRotationX);
        }
    }

    [Button("重置旋转")]
    public void ResetRotation()
    {
        // 递归重置所有子物体的旋转
        ResetAllChildrenRotation(transform);
        Debug.Log("子物体角度已重置。");
    }

    // 递归重置所有子物体的旋转
    void ResetAllChildrenRotation(Transform parent)
    {
        foreach (Transform child in parent)
        {
            child.localEulerAngles = Vector3.zero;

            // 递归调用，重置所有下层子物体的旋转
            ResetAllChildrenRotation(child);
        }
    }
}