using UnityEditor;
using UnityEngine;

[InitializeOnLoad] 
public class ClearSelectionOnPlay
{
    static ClearSelectionOnPlay()
    {
        // Register ClearSelection
        EditorApplication.playModeStateChanged += ClearSelection;
    }

    private static void ClearSelection(PlayModeStateChange state)
    {
        // when play mode start
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            Selection.activeObject = null;
            Debug.Log("Clear hierarchy selection to prevent bugs!");
        }
    }
}