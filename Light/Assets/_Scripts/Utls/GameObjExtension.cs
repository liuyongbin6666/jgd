using UnityEngine;

public static class GameObjExtension
{
    public static bool IsInLayerMask(this GameObject obj,LayerMask mask) => (mask.value & (1 << obj.layer)) != 0;
}