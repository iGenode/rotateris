using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverTrigger : MonoBehaviour
{
    //[SerializeField]
    //private LayerMask ObstacleLayerMask;
    //private PlayerController _playerControllerComponent;
    //private bool _isInTrigger = false;

    //private void OnTriggerEnter(Collider other)
    //{
    //    _playerControllerComponent = other.gameObject.GetComponent<PlayerController>();
    //    _isInTrigger = true;
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    _isInTrigger = false;
    //    _playerControllerComponent = null;
    //}

    //private void Update()
    //{
    //    if (_isInTrigger && _playerControllerComponent == null)
    //    {
    //        Debug.Log("GAME OVER");
    //    }
    //}

    private Vector3 _halfExtents;

    private void Start()
    {
        _halfExtents = gameObject.GetComponent<Collider>().bounds.extents;
    }

    private void OnEnable()
    {
        SpawnManager.OnSettled += CheckTriggerForObjects;
    }

    private void OnDisable()
    {
        SpawnManager.OnSettled -= CheckTriggerForObjects;
    }

    private void CheckTriggerForObjects()
    {
        if (Physics.OverlapBox(
            transform.position,
            _halfExtents,
            transform.rotation).Length != 0)
        {
            GameOver();
        }
    }
    
    public void GameOver()
    {
        Debug.Log("GAME OVER");
    }
}
