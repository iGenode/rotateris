using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // TODO: when instantiating a player give it an index, that index corresponds to players position and angle
    // TODO: 360 / index is an angle at which the players repeat, angle * index is the coordinate of the player
    [SerializeField]
    float horizontalMoveDelay = .2f;
    public Transform MovePoint;
    public LayerMask StopMovement;

    private GameState _gameState;
    private Vector2 _move;
    private float _timeToMove = 0f;
    private bool _isMove = false;
    private bool _isGrounded = false;
    private Vector3 _extents;
    private Transform RotationPoint
    {
        get
        {
            foreach (Transform child in transform)
            {
                if (child.GetComponent<Collider>().ClosestPoint(transform.position) == transform.position)
                {
                    return child;
                }
            }
            return null;
        }
    }

    void Start()
    {
        _gameState = GameObject.Find("Game Controller").GetComponent<GameState>();

        MovePoint.parent = null;

        Collider[] colliders = gameObject.GetComponentsInChildren<Collider>();
        var bounds = new Bounds(transform.position, Vector3.one);
        foreach (Collider c in colliders)
        {
            bounds.Encapsulate(c.bounds);
        }
        _extents = bounds.extents;

        StartCoroutine(WaitToMove());
    }

    void Update()
    {
        if (_timeToMove >= horizontalMoveDelay)
        {
            transform.position = MovePoint.position;
            _timeToMove = 0;
        }
        if (Vector3.Distance(transform.position, MovePoint.position) <= .05f)
        {
            _timeToMove = 0f;
            if (Mathf.Abs(_move.x) == 1)
            {
                if (Physics.OverlapBox(MovePoint.position + new Vector3(_move.x, 0, 0), _extents, Quaternion.identity, StopMovement).Length == 0)
                {
                    MovePoint.position += new Vector3(_move.x, 0, 0);
                }
            }
        }

        _timeToMove += Time.deltaTime;

        if (_isMove && !_isGrounded)
        {
            MovePoint.Translate(Vector3.down);
            transform.Translate(Vector3.down, Space.World);
            StartCoroutine(WaitToMove());
        }
    }

    private IEnumerator WaitToMove()
    {
        _isMove = false;
        yield return new WaitForSeconds(_gameState.Speed);
        _isMove = true;
    }

    private IEnumerator WaitUntilDestruction()
    {
        yield return new WaitForSeconds(5);
        Destroy(this);
    }

    private void OnCollisionEnter(Collision collision)
    {
        _isGrounded = true;
        StartCoroutine(WaitUntilDestruction());
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _move = context.ReadValue<Vector2>();
    }
    
    public void OnRotateLeft(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (RotationPoint)
            {
                transform.RotateAround(RotationPoint.position, Vector3.forward, 90);
                MovePoint.position = transform.position;
            }
            else
            {
                transform.Rotate(Vector3.forward, 90);
            }
        }
    }

    public void OnRotateRight(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (RotationPoint)
            {
                transform.RotateAround(RotationPoint.position, Vector3.back, 90);
                MovePoint.position = transform.position;
            }
            else
            {
                transform.Rotate(Vector3.back, 90);
            }
        }
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.matrix = transform.localToWorldMatrix;
    //    Gizmos.DrawWireCube(Vector3.zero, _extents * 2);
    //}
}
