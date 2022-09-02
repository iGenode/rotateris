using System.Collections.Generic;
using UnityEngine;

public class RowFilledTrigger : MonoBehaviour
{
    [SerializeField]
    private PlayingFieldState _playingFieldState;
    [SerializeField]
    private LayerMask _obstacleLayerMask;

    private Vector3 _triggerHalfExtents = new(7, 0.45f, 0.45f);
    private HashSet<float> _checkedYs = new HashSet<float>();
    private HashSet<float> _ysToMove = new HashSet<float>();

    private void DestroyRow(Collider[] colliders)
    {
        _playingFieldState.IncreaseLinesCleared();

        foreach (Collider collider in colliders)
        {
            Destroy(collider.gameObject);
        }
    }

    private void CheckTriggerForObjects(List<Transform> settledObjects)
    {
        foreach (Transform child in settledObjects)
        {
            if (_checkedYs.Contains(Utils.RoundFloatToTwoDecimals(child.position.y))) // If this row was checked already, skip it
            {
                continue;
            }

            var row = Physics.OverlapBox(
                new Vector3(transform.position.x, child.position.y, transform.position.z),
                _triggerHalfExtents);
            if (row.Length == _triggerHalfExtents.x * 2 - 1) // If row is filled, destroy it and save current y to move
            {
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

        _checkedYs.Clear();
        _ysToMove.Clear();
    }

    private void OnEnable()
    {
        SpawnManager.OnSettledWithData += CheckTriggerForObjects;
    }

    private void OnDisable()
    {
        SpawnManager.OnSettledWithData += CheckTriggerForObjects;
    }
}
