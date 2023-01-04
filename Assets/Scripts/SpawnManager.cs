using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public delegate void SettledAction();
    public event SettledAction OnSettled;
    public delegate void SettledWithDataAction(List<Transform> settledChildren);
    public event SettledWithDataAction OnSettledWithData;
    public delegate void SettledWithPositionAction(Vector3 position);
    public event SettledWithPositionAction OnSettledWithPosition;

    [SerializeField]
    private ConstantListOfGameObjects PlayerPrefabs;
    [SerializeField]
    private Transform _spawnPoint;
    private List<Transform> _currentChildren = new List<Transform>();
    private PlayerController _currentPlayerController;
    private int _previousIndex = -1;
    private bool _canHold = true;

    void Start()
    {
        InstantiateNewPlayer();
    }

    private void SettlePlayer()
    {
        OnSettled?.Invoke();
        OnSettledWithData?.Invoke(_currentChildren);
        OnSettledWithPosition?.Invoke(_currentChildren[0].position);
        InstantiateNewPlayer();
    }

    public PlayerController GetCurrentPlayer() => _currentPlayerController;

    public void InstantiateNewPlayer()
    {
        _canHold = true;
        _currentChildren.Clear();
        var index = Random.Range(0, PlayerPrefabs.List.Count);
        if (index == _previousIndex) // Reroll index if it's equal to previous to lessen the chance of the same pieces
        {
            index = Random.Range(0, PlayerPrefabs.List.Count);
        }
        var currentPlayer = Instantiate(PlayerPrefabs.List[index], _spawnPoint.position, transform.parent.rotation);
        currentPlayer.transform.parent = transform.parent;
        _currentPlayerController = currentPlayer.GetComponent<PlayerController>();
        _currentPlayerController.OnSettlePlayer += SettlePlayer;
        foreach (Transform child in currentPlayer.transform)
        {
            if (!child.gameObject.CompareTag("MovePoint"))
            {
                _currentChildren.Add(child);
            }
        }
        _previousIndex = index;
    }

    public void InstantiateNewPlayerWithName(string name)
    {
        _canHold = true;
        _currentChildren.Clear();
        var prefab = PlayerPrefabs.List.Find(shape => name.Contains(shape.name));
        if (prefab == null)
        {
            Debug.Log("Name not found!");
            return;
        }
        var currentPlayer = Instantiate(prefab, _spawnPoint.position, transform.parent.rotation);
        currentPlayer.transform.parent = transform.parent;
        _currentPlayerController = currentPlayer.GetComponent<PlayerController>();
        _currentPlayerController.OnSettlePlayer += SettlePlayer;
        foreach (Transform child in currentPlayer.transform)
        {
            if (!child.gameObject.CompareTag("MovePoint"))
            {
                _currentChildren.Add(child);
            }
        }
    }

    public bool CanHold() => _canHold;

    public bool SetHoldFlag(bool canHold) => _canHold = canHold;

    public void DiscardCurrentPlayerController()
    {
        _currentPlayerController.OnSettlePlayer -= SettlePlayer;
        _currentPlayerController = null;
    }
}
