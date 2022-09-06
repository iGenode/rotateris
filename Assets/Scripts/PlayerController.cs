using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // TODO: when instantiating a player give it an index, that index corresponds to players position and angle
    // TODO: 360 / index is an angle at which the players repeat, angle * index is the coordinate of the player
    //[SerializeField]
    //float horizontalMoveDelay = .2f;
    public Transform MovePoint;
    public LayerMask ObstacleLayerMask;

    private PlayingFieldState _playingFieldState;
    private Vector2 _move;
    private float _timeToMove = 0f;
    private bool _shouldMoveDown = false;
    private bool _isGrounded = false;
    private Vector3 _extents;
    // Since all children are the same - cache one extent for later calculations
    private Vector3 _childExtents;
    private Transform _pivot;
    private bool _shouldSettle = false;
    private bool _isDroppingDown = false;

    void Start()
    {
        _playingFieldState = transform.parent.Find("Playing Field Controller").GetComponent<PlayingFieldState>();
        MovePoint.parent = null;
        transform.position = MovePoint.position;

        Collider[] colliders = gameObject.GetComponentsInChildren<Collider>();
        _childExtents = colliders[0].bounds.extents;
        var bounds = new Bounds(transform.position, Vector3.one);
        foreach (Collider c in colliders)
        {
            bounds.Encapsulate(c.bounds);
        }
        // Shrink extents to prevent false collision detection
        _extents = bounds.extents - Vector3.one * 0.1f;

        // TODO: probably some logic to game over and delete an object instead of moving up
        var collisions = Physics.OverlapBox(MovePoint.position, _extents, transform.rotation, ObstacleLayerMask).Length;
        if (collisions != 0) // If figure collides with something as it spawns - move up
        {
            MoveFigure(new(0, collisions, 0));
        }

        _pivot = GetRotationPoint();

        StartCoroutine(WaitToMove());
    }

    void Update()
    {
        if (_timeToMove >= GameState.HorizontalMoveDelay)
        {
            // Since actual movement happens on different frames (only after HorizontalMoveDelay)
            // - check if still safe to move, and if not - return MovePoint to player's transform.position
            if (IsSafeToMove((MovePoint.position - transform.position).normalized))
            {
                transform.position = MovePoint.position;
            }
            else
            {
                MovePoint.position = transform.position;
            }
            _timeToMove = 0;
        }
        if (Vector3.Distance(transform.position, MovePoint.position) <= .05f)
        {
            _timeToMove = 0f;
            //if (Mathf.Abs(_move.x) == 1)
            if (_move.x != 0)
            {
                var moveDirection = new Vector3(_move.x, 0, 0);
                if (IsSafeToMove(moveDirection))
                {
                    // Move MovePoint for the player to follow after HorizontalMoveDelay
                    MovePoint.position += moveDirection;
                }
                //else
                //{
                //    Debug.Log("CANT MOVE");
                //}
            }
        }

        _timeToMove += Time.deltaTime;

        if (_shouldMoveDown)
        {
            if (IsSafeToMove(Vector3.down))
            {
                MoveFigure(Vector3.down);
                if (!_isDroppingDown)
                {
                    StartCoroutine(WaitToMove());
                }

                // If object was able to move down again after being grounded - stop destruction
                if (_isGrounded)
                {
                    Debug.Log("Stop destruction, can move again");
                    _shouldSettle = false;
                    _isGrounded = false;
                }
            }
            else if (!_shouldSettle) // If not already waiting to settle
            {
                if (_isDroppingDown) // If dropping down settle instantly
                {
                    Settle();
                }
                else // Else prepare to settle
                {
                    PrepareToSettle();
                }
            }
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
        _shouldMoveDown = false;
        yield return new WaitForSeconds(_playingFieldState.Speed);
        _shouldMoveDown = true;
    }

    private IEnumerator WaitUntilSettle()
    {
        yield return new WaitForSeconds(5);
        if (_shouldSettle)
        {
            Settle();
        }
    }

    // THIS MIGHT BE NEEDED BUT NOT FINDING ANY USE AT THE MOMENT
    //private void OnCollisionEnter(Collision collision)
    //{
    //    PrepareToSettle();
    //}

    private void OnMove(Vector2 move)
    {
        _move = move;
    }

    private void OnRotate(float direction)
    {
        var directionModifier = -Mathf.RoundToInt(direction);
        if (_pivot && IsSafeToRotate(90 * directionModifier))
        {
            transform.RotateAround(_pivot.position, Vector3.forward, 90 * directionModifier);
            MovePoint.position = transform.position;
        }
    }

    private void OnDropBlock()
    {
        if (_isGrounded || _shouldSettle)
        {
            Settle();
        }

        _isDroppingDown = true;
        _shouldMoveDown = true;
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
            if (Physics.OverlapBox(point, _childExtents, child.transform.rotation, ObstacleLayerMask).Length != 0)
            {
                //Debug.Log("Can not just rotate");
                // If simply rotating isn't enough move to the right
                point.x++;
                if (Physics.OverlapBox(point, _childExtents, child.transform.rotation, ObstacleLayerMask).Length != 0)
                {
                    //Debug.Log("Can not just rotate and move right");
                    // If moving to the right wasn't enough move to the left of origin (two local)
                    point.x -= 2;
                    if (Physics.OverlapBox(point, _childExtents, child.transform.rotation, ObstacleLayerMask).Length != 0)
                    {
                        //Debug.Log("Can not rotate at all");
                        return false;
                    }
                    else
                    {
                        //Debug.Log("Can rotate with move left");
                        shouldMoveLeft = true;
                    }
                }
                else
                {
                    //Debug.Log("Can rotate with move right");
                    shouldMoveRight = true;
                }
            }
        }
        if (shouldMoveRight)
        {
            if (IsSafeToMove(Vector3.right))
            {
                MoveFigure(Vector3.right);
                return true;
            }
            else
            {
                return false;
            }
        }
        if (shouldMoveLeft)
        {
            if (IsSafeToMove(Vector3.left))
            {
                MoveFigure(Vector3.left);
                return true;
            }
            else
            {
                return false;
            }
        }

        return true;
    }

    private bool IsSafeToMove(Vector3 direction)
    {
        foreach (Transform child in transform)
        {
            if (Physics.OverlapBox(
                child.transform.position + direction,
                _childExtents - Vector3.one * 0.1f,
                child.transform.rotation,
                ObstacleLayerMask).Length != 0)
            {
                return false;
            }
        }
        return true;
    }

    private void MoveFigure(Vector3 direction)
    {
        MovePoint.Translate(direction);
        transform.Translate(direction, Space.World);
    }

    private void PrepareToSettle()
    {
        Debug.Log("Scheduled to settle!");
        _isGrounded = true;
        StartCoroutine(WaitUntilSettle());
        _shouldSettle = true;
    }

    private void Settle()
    {
        transform.SetLayerRecursively((int)Mathf.Log(ObstacleLayerMask.value, 2));
        Destroy(gameObject.GetComponent<Rigidbody>());
        Destroy(this);
        Destroy(MovePoint.gameObject);

        transform.DetachChildren();
        Destroy(gameObject);
    }

    private void OnEnable()
    {
        InputController.OnMoveEvent += OnMove;
        InputController.OnRotateEvent += OnRotate;
        InputController.OnDropDownEvent += OnDropBlock;
    }

    private void OnDisable()
    {
        InputController.OnMoveEvent -= OnMove;
        InputController.OnRotateEvent -= OnRotate;
        InputController.OnDropDownEvent -= OnDropBlock;
    }

    // Debug
    private void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector3.zero, _extents * 2);
    }
}
