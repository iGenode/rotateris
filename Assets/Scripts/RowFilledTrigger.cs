using System.Collections.Generic;
using UnityEngine;

public class RowFilledTrigger : MonoBehaviour
{
    [SerializeField]
    private PlayingFieldState _playingFieldState;
    [SerializeField]
    private LayerMask _obstacleLayerMask;
    private Vector3 _triggerHalfExtents;
    private HashSet<float> _checkedYs = new HashSet<float>();
    private HashSet<float> _ysToMove = new HashSet<float>();
    private SpawnManager _spawnManager;

    private void Start()
    {
        _spawnManager = GameObject.Find($"/{transform.parent.name}/Spawn Manager").GetComponent<SpawnManager>();
        _spawnManager.OnSettledWithData += CheckTriggerForObjects;

        _triggerHalfExtents = new(_playingFieldState.Size / 2.0f - .1f, 0.45f, 0.45f);
    }

    private void DestroyRow(Collider[] colliders)
    {
        foreach (Collider collider in colliders)
        {
            Destroy(collider.gameObject);
        }
    }

    private void CheckTriggerForObjects(List<Transform> settledObjects)
    {
        var clearedRowsCount = 0;
        foreach (Transform child in settledObjects)
        {
            if (_checkedYs.Contains(Utils.RoundFloatToTwoDecimals(child.position.y))) // If this row was checked already, skip it
            {
                continue;
            }

            var row = Physics.OverlapBox(
                new Vector3(transform.position.x, child.position.y, transform.position.z),
                _triggerHalfExtents,
                transform.rotation
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
            _playingFieldState.IncreaseLinesCleared(clearedRowsCount);
        }

        _checkedYs.Clear();
        _ysToMove.Clear();
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
