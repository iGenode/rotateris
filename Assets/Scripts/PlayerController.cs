using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using System.Linq;

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
    // Since all children are the same - cache one extent for later calculations
    private Vector3 _childExtents;
    private Transform _pivot;

    void Start()
    {
        _gameState = GameObject.Find("Game Controller").GetComponent<GameState>();

        MovePoint.parent = null;

        Collider[] colliders = gameObject.GetComponentsInChildren<Collider>();
        _childExtents = colliders[0].bounds.extents;
        var bounds = new Bounds(transform.position, Vector3.one);
        foreach (Collider c in colliders)
        {
            bounds.Encapsulate(c.bounds);
        }
        _extents = bounds.extents;

        _pivot = GetRotationPoint();

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
                if (Physics.OverlapBox(MovePoint.position + new Vector3(_move.x, 0, 0), _extents, transform.rotation, StopMovement).Length == 0)
                {
                    MovePoint.position += new Vector3(_move.x, 0, 0);
                }
            }
        }

        _timeToMove += Time.deltaTime;

        if (_isMove && !_isGrounded)
        {
            MoveFigure(Vector3.down);
            StartCoroutine(WaitToMove());
        }
    }

    private Transform GetRotationPoint()
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

    private Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        return Quaternion.Euler(angles) * (point - pivot) + pivot;
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

    public void OnRotate(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            var direction = -(int)context.ReadValue<Vector2>().x;
            if (_pivot && IsSafeToRotate(90 * direction))
            {
                transform.RotateAround(_pivot.position, Vector3.forward, 90 * direction);
                MovePoint.position = transform.position;
            }
        }
    }

    /// <summary>
    /// Method tests if it is safe to rotate a figure by <paramref name="angle"/> degrees
    /// </summary>
    /// <param name="angle"> Euler's angle of rotation </param>
    /// <returns> true if rotation is safe and false otherwise </returns>
    private bool IsSafeToRotate(int angle)
    {
        var shouldMoveRight = false;
        var shouldMoveLeft = false;
        foreach (Transform child in transform)
        {
            var point = RotatePointAroundPivot(child.transform.position, _pivot.position, new(0, 0, angle));
            // Test if a part of the object collides with something after roatation
            if (Physics.OverlapBox(point, _childExtents, child.transform.rotation, StopMovement).Length != 0)
            {
                // If simply rotating isn't enough move to the right
                point.x++;
                if (Physics.OverlapBox(point, _childExtents, child.transform.rotation, StopMovement).Length != 0)
                {
                    // If moving to the right wasn't enough move to the left of origin (two local)
                    point.x -= 2;
                    if (Physics.OverlapBox(point, _childExtents, child.transform.rotation, StopMovement).Length != 0)
                    {
                        return false;
                    }
                    else
                    {
                        shouldMoveLeft = true;
                    }
                } 
                else
                {
                    shouldMoveRight = true;
                }
            }
        }
        if (shouldMoveRight)
        {
            MoveFigure(Vector3.right);
            return true;
        }
        if (shouldMoveLeft)
        {
            MoveFigure(Vector3.left);
            return true;
        }

        return true;
    }

    private void MoveFigure(Vector3 direction)
    {
        MovePoint.Translate(direction);
        transform.Translate(direction, Space.World);
    }

    // Debug
    private void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector3.zero, _extents * 2);

        //var oldMatrix = Gizmos.matrix;

        //// create a matrix which translates an object by "position", rotates it by "rotation" and scales it by "halfExtends * 2"
        //Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, _extents * 2);
        //// Then use it one a default cube which is not translated nor scaled
        //Gizmos.DrawWireCube(Vector3.zero, Vector3.one);

        //Gizmos.matrix = oldMatrix;
    }
}
