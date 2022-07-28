using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{

    public delegate void SettledAction();
    public static event SettledAction OnSettled;
    public delegate void SettledWithDataAction(List<Transform> settledChildren);
    public static event SettledWithDataAction OnSettledWithData;

    public GameObject[] PlayerPrefabs;
    [SerializeField]
    private Transform _spawnPoint;
    private List<Transform> _currentChildren = new List<Transform>();
    private PlayerController _currentPlayerController;

    void Start()
    {
        InstantiateNewPlayer();
    }

    void Update()
    {
        if (_currentPlayerController == null)
        {
            OnSettled?.Invoke();
            OnSettledWithData?.Invoke(_currentChildren);
            InstantiateNewPlayer();
        }
    }

    private void InstantiateNewPlayer()
    {
        _currentChildren.Clear();
        var index = Random.Range(0, PlayerPrefabs.Length);
        var currentPlayer = Instantiate(PlayerPrefabs[index], _spawnPoint.position, PlayerPrefabs[index].transform.rotation);
        _currentPlayerController = currentPlayer.GetComponent<PlayerController>();
        foreach (Transform child in currentPlayer.transform)
        {
            if (!child.gameObject.CompareTag("MovePoint"))
            {
                _currentChildren.Add(child);
            }
        }
    }
}
