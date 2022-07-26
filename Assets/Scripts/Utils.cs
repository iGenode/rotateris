using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    // Fastest method to set children layer
    public static void SetLayerRecursively(this Transform parent, int layer)
    {
        parent.gameObject.layer = layer;

        for (int i = 0, count = parent.childCount; i < count; i++)
        {
            parent.GetChild(i).SetLayerRecursively(layer);
        }
    }
}
