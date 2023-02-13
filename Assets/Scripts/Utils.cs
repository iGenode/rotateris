using UnityEngine;
using UnityEngine.SceneManagement;

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

    public static float RoundFloatToTwoDecimals(float toRound)
    {
        return Mathf.Round(toRound * 100) / 100;
    }

    public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        return Quaternion.Euler(angles) * (point - pivot) + pivot;
    }


    // Sound maths
    // AudioMixer decibels calculations
    // https://forum.unity.com/threads/changing-audio-mixer-group-volume-with-ui-slider.297884/
    public static float NormalizeForMixer(float value) => Mathf.Log(value) * 20.0f;
    public static float NormalizeForAudioLabel(float value) => Mathf.Round(value * 100.0f);
    public static float NormalizeForUISlider(float value) => Mathf.Exp(value / 20);


    // Scene management
    public static void MenuToGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public static void GameToMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public static void RestartCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
