using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverTrigger : MonoBehaviour
{
    [SerializeField]
    private LayerMask _obstacleLayerMask;
    private Vector3 _halfExtents;

    private void Start()
    {
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
            GameOver();
        }
    }

    public void GameOver()
    {
        Debug.Log("GAME OVER");
    }

    private void OnEnable()
    {
        SpawnManager.OnSettled += CheckTriggerForObjects;
    }

    private void OnDisable()
    {
        SpawnManager.OnSettled -= CheckTriggerForObjects;
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.matrix = transform.localToWorldMatrix;
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireCube(Vector3.zero, _halfExtents * 2);
    //}
}
