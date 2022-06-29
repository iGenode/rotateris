using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // TODO: when instantiating a player give it an index, that index corresponds to players position and angle
    // TODO: 360 / index is an angle at which the players repeat, angle * index is the coordinate of the player
    [SerializeField]
    float horizontalSpeed = 5f;
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
    private Vector3 _rotationPoint;

    //private bool _isMoving = false;
    //private Vector3 originPos;
    //private Vector3 targetPos;
    // TODO: have a game script that determines speed, moveUnit, playing field amount

    // Start is called before the first frame update
    void Start()
    {
        _gameState = GameObject.Find("Game Controller").GetComponent<GameState>();

        MovePoint.parent = null;
        //_extents = gameObject.GetComponent<BoxCollider>().bounds.extents;

        Collider[] colliders = gameObject.GetComponentsInChildren<Collider>();
        Debug.Log(colliders.Length);
        var bounds = new Bounds(transform.position, Vector3.one);
        foreach (Collider c in colliders)
        {
            bounds.Encapsulate(c.bounds);
        }
        _extents = bounds.extents;
        Debug.Log(bounds);

        _rotationPoint = GetRotationPoint();

        StartCoroutine(WaitToMove());
    }

    // Update is called once per frame
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

        //if (transform.position.x < -_xBound)
        //{
        //    transform.position = new(-_xBound, transform.position.y, transform.position.z);
        //}
        //else if (transform.position.x > _xBound)
        //{
        //    transform.position = new(_xBound, transform.position.y, transform.position.z);
        //}

        if (_isMove && !_isGrounded)
        {
            MovePoint.Translate(Vector3.down);
            transform.Translate(Vector3.down, Space.World);
            StartCoroutine(WaitToMove());
        }
    }

    private Vector3 GetRotationPoint()
    {
        foreach (Transform child in transform)
        {
            if (child.GetComponent<Collider>().ClosestPoint(transform.position) == transform.position) {
                Debug.Log($"Found rotation point: ${child}, ${child.position}");
                return child.position;
            }
            //child is your child transform
        }
        return Vector3.zero;
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

    //private IEnumerator MovePlayer(Vector2 direction)
    //{
    //    _isMoving = true;

    //    originPos = transform.position;
    //    targetPos = originPos + new Vector3(direction.x, 0, 0);

    //    var elapsedTime = 0f;

    //    // Moving while elapsed time is less than time to move
    //    while (elapsedTime < timeToMove)
    //    {
    //        transform.position = Vector3.Lerp(originPos, targetPos, elapsedTime / timeToMove);
    //        elapsedTime += Time.deltaTime;
    //        yield return null;
    //    }

    //    // Making sure we're at the target point
    //    transform.position = targetPos;

    //    _isMoving = false;
    //}

    private void OnCollisionEnter(Collision collision)
    {
        _isGrounded = true;
        StartCoroutine(WaitUntilDestruction());
        // TODO: start timer for rotation/movement of the brick, settle this player and spawn another?
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _move = context.ReadValue<Vector2>();
        //if (!_isMoving)
        //{
        //    StartCoroutine(MovePlayer(context.ReadValue<Vector2>()));
        //}
    }
    
    public void OnRotateLeft(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            transform.Rotate(Vector3.forward, 90);
        }
    }

    public void OnRotateRight(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            transform.Rotate(Vector3.back, 90);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, _extents * 2);
    }
}
