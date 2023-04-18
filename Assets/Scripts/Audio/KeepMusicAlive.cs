using UnityEngine;

public class KeepMusicAlive : MonoBehaviour
{
    private static KeepMusicAlive _instance;

    private void Awake()
    {
        if (_instance)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
    }
}
