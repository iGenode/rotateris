using UnityEngine;

public class SpawnManager : MonoBehaviour
{

    public delegate void SettledAction();
    public static event SettledAction OnSettled;

    public GameObject[] PlayerPrefabs;
    [SerializeField]
    private Transform _spawnPoint;
    private PlayerController _currentPlayerController;

    // Start is called before the first frame update
    void Start()
    {
        InstantiateNewPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        if (_currentPlayerController == null)
        {
            OnSettled?.Invoke();
            InstantiateNewPlayer();
        }
    }

    private void InstantiateNewPlayer()
    {
        var index = Random.Range(0, PlayerPrefabs.Length);
        var currentPlayer = Instantiate(PlayerPrefabs[index], _spawnPoint.position, PlayerPrefabs[index].transform.rotation);
        _currentPlayerController = currentPlayer.GetComponent<PlayerController>();
    }
}
