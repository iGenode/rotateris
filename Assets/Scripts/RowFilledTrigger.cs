using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RowFilledTrigger : MonoBehaviour
{
    public delegate void OnDestroyManyRows(int count);
    public event OnDestroyManyRows OnManyRowsDestroyed;
    public delegate void OnDestroyAtPosition(Vector3 position);
    public event OnDestroyAtPosition OnDestroyedAtPosition;

    [SerializeField]
    private PlayingFieldState _playingFieldState;
    [SerializeField]
    private LayerMask _obstacleLayerMask;
    [SerializeField]
    private GameObject _explosionPrefab;

    private Vector3 _triggerHalfExtents;
    private readonly HashSet<float> _checkedYs = new();
    private readonly HashSet<float> _ysToMove = new();
    private SpawnManager _spawnManager;

    private void Start()
    {
        _spawnManager = transform.parent.GetComponentInChildren<SpawnManager>();
        _spawnManager.OnSettledWithData += CheckTriggerForObjects;

        _triggerHalfExtents = new(_playingFieldState.Size / 2.0f - .1f, 0.45f, 0.45f);
    }

    private void DestroyRow(Collider[] colliders)
    {
        foreach (Collider collider in colliders)
        {
            // Caching position and material before destruction
            var position = collider.transform.position;
            var material = collider.gameObject.GetComponent<Renderer>().material;
            // Destroying current cube
            Destroy(collider.gameObject);
            // Instantiating an explosive cube
            var prefab = Instantiate(_explosionPrefab, position, transform.rotation);
            // Setting the material of each child to match to-be-destroyed cube
            foreach (Transform child in prefab.transform)
            {
                child.GetComponent<Renderer>().material = material;
            }
            // Starting a timer to destroy the explosion
            // time scales with difficulty level to prevent covering the blocks with particles for too long at high levels
            StartCoroutine(
                DestroyAfter(
                    prefab,
                    3 - 0.1f * _playingFieldState.Level
                )
            );

        }
    }

    private void CheckTriggerForObjects(List<Transform> settledObjects)
    {
        var clearedRowsCount = 0;
        Vector3 centerPos = new();
        foreach (Transform child in settledObjects)
        {
            if (_checkedYs.Contains(Utils.RoundFloatToTwoDecimals(child.position.y))) // If this row was checked already, skip it
            {
                continue;
            }
            centerPos = new Vector3(transform.position.x, child.position.y, transform.position.z);

            var row = Physics.OverlapBox(
                centerPos,
                _triggerHalfExtents,
                transform.rotation,
                _obstacleLayerMask
            );
            //Debug.Log($"Found {row.Length} objects");
            if (row.Length == _playingFieldState.Size) // If row is filled, destroy it and save current y to move
            {
                //Debug.Log($"Row filled, destroying!");
                clearedRowsCount++;
                DestroyRow(row);
                _ysToMove.Add(Utils.RoundFloatToTwoDecimals(child.position.y));
            }
            _checkedYs.Add(Utils.RoundFloatToTwoDecimals(child.position.y));
        }
        if (_ysToMove.Count != 0)
        {
            foreach (float y in _ysToMove)
            {
                var verticalCenter = (24 - y) / 2 + y;
                var collidersToMove = Physics.OverlapBox(
                    new Vector3(transform.position.x, verticalCenter, transform.position.z),
                    new Vector3(_triggerHalfExtents.x, 24 - verticalCenter, _triggerHalfExtents.z),
                    transform.rotation,
                    _obstacleLayerMask);
                foreach (Collider collider in collidersToMove)
                {
                    collider.transform.Translate(Vector3.down, Space.World);
                }
            }
        }
        if (clearedRowsCount > 0)
        {
            OnManyRowsDestroyed?.Invoke(clearedRowsCount);
            OnDestroyedAtPosition?.Invoke(centerPos);
        }

        _checkedYs.Clear();
        _ysToMove.Clear();
    }

    private IEnumerator DestroyAfter(GameObject toDestroy, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(toDestroy);
    }

    private void OnEnable()
    {
        if (_spawnManager != null)
        {
            _spawnManager.OnSettledWithData += CheckTriggerForObjects;
        }
    }

    private void OnDisable()
    {
        _spawnManager.OnSettledWithData += CheckTriggerForObjects;
    }
}
