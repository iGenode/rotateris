using UnityEngine;

public class GameOverTrigger : MonoBehaviour
{
    [SerializeField]
    private LayerMask _obstacleLayerMask;
    private Vector3 _halfExtents;
    private SpawnManager _spawnManager;

    private void Start()
    {
        _spawnManager = GameObject.Find($"/{transform.parent.name}/Spawn Manager").GetComponent<SpawnManager>();
        _spawnManager.OnSettled += CheckTriggerForObjects;

        _halfExtents = gameObject.GetComponent<Collider>().bounds.extents;
    }
    private void CheckTriggerForObjects()
    {
        if (Physics.OverlapBox(
            transform.position,
            _halfExtents,
            transform.rotation,
            _obstacleLayerMask).Length != 0)
        {
            //Debug.Log("GAME OVER");
            GameState.GameOver();
        }
    }

    private void OnEnable()
    {
        if (_spawnManager != null)
        {
            _spawnManager.OnSettled += CheckTriggerForObjects;
        }
    }

    private void OnDisable()
    {
        _spawnManager.OnSettled -= CheckTriggerForObjects;
    }
}
