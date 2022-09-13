using System.Collections;
using UnityEngine;

// TODO: resolve an issue with controlls propagation when holding horizontal movement
// TODO: add object storage to store one of the shapes in
public class PlayerController : MonoBehaviour
{
    public Transform MovePoint;
    public LayerMask ObstacleLayerMask;

    private PlayingFieldState _playingFieldState;
    private Vector2 _move;
    private float _timeToMove = 0f;
    private bool _shouldMoveDown = false;
    private bool _isGrounded = false;
    // Since all children are the same - cache one extent for later calculations
    private Vector3 _childExtents = new(0.4f, 0.4f, 0.4f);
    private Transform _pivot;
    private bool _shouldSettle = false;
    private bool _isDroppingDown = false;

    void Start()
    {
        _playingFieldState = GetComponentInParent<PlayingFieldState>();
        _playingFieldState.OnFocusChangedEvent += ChangeInputControllerSubscription;
        if (!_playingFieldState.IsFocused)
        {
            ChangeInputControllerSubscription(false);
        }

        MovePoint.parent = null;
        transform.position = MovePoint.position;

        var numberOfCollisions = NumberOfCollisions();
        // If figure collides with something as it spawns - move up by a number of collisions + 1 for safety
        if (numberOfCollisions != 0) 
        {
            MoveFigure(new(0, numberOfCollisions + 1, 0));
        }

        // TODO: probably smarter to just pick one of the objects in a prefab
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
            if (_move.x != 0)
            {
                var moveDirection = _move.x > 0 ? transform.parent.right : -transform.parent.right;
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

    private void OnMove(Vector2 move)
    {
        _move = move;
    }

    private void OnRotate(float direction)
    {
        var directionModifier = -Mathf.RoundToInt(direction);
        if (_pivot && IsSafeToRotate(90 * directionModifier))
        {
            //transform.Rotate(Vector3.forward, 90 * directionModifier, Space.Self);
            transform.RotateAround(_pivot.position, transform.forward, 90 * directionModifier);
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
    /// Tests if it is safe to rotate a figure by <paramref name="angle"/> degrees
    /// </summary>
    /// <param name="angle"> Euler's angle of rotation </param>
    /// <returns> true if rotation is safe and false otherwise </returns>
    private bool IsSafeToRotate(int angle)
    {
        var moveRight = false;
        var moveLeft = false;
        var canMoveRight = true;
        var canMoveLeft = true;
        foreach (Transform child in transform)
        {
            var point = Utils.RotatePointAroundPivot(
                child.position,
                _pivot.position,
                Quaternion.AngleAxis(angle, transform.parent.forward).eulerAngles
            );
            // Test if a part of the object collides with something after roatation
            if (Physics.OverlapBox(point, _childExtents, child.rotation, ObstacleLayerMask).Length != 0)
            {
                // If something collides - test right and left translations separately
                var right = Physics.OverlapBox(point + transform.parent.right, _childExtents, child.rotation,
                    ObstacleLayerMask).Length == 0;
                var left = Physics.OverlapBox(point - transform.parent.right, _childExtents, child.rotation,
                    ObstacleLayerMask).Length == 0;
                if (!right && !left) // If both translations fail - can not rotate
                {
                    //Debug.Log("Cant rotate at all early");
                    return false;
                }
                if (right) // If right translation seems fine - set flag and check if actually safe to move that way
                {
                    moveRight = true;
                    if (canMoveRight) // This if prevents further iterations from overriding the flag
                    {
                        canMoveRight = IsRotationWithMoveSafe(transform.parent.right, angle);
                    }
                }
                if (left) // If left translation seems fine - set flag and check if actually safe to move that way
                {
                    moveLeft = true;
                    if (canMoveLeft) // This if prevents further iterations from overriding the flag
                    {
                        canMoveLeft = IsRotationWithMoveSafe(-transform.parent.right, angle);
                    }
                }
            }
        }
        if (!canMoveRight && !canMoveLeft) // If can not move either way, then can not rotate
        {
            //Debug.Log("Cant rotate at all");
            return false;
        }
        if (moveRight && canMoveRight) // If can rotate right and safe to do so - rotate right with move
        {
            //Debug.Log("Moving right");
            MoveFigure(transform.parent.right);
            return true;
        }
        if (moveLeft && canMoveLeft) // If can rotate left and safe to do so - rotate left with move
        {
            //Debug.Log("Moving left");
            MoveFigure(-transform.parent.right);
            return true;
        }

        // If both rotations can not happen for different reasons
        if ((moveRight && !canMoveRight && !moveLeft && canMoveLeft) ||
            (!moveRight && canMoveRight && moveLeft && !canMoveLeft))
        {
            //Debug.Log("Can not rotate for different reasons");
            return false;
        }

        //Debug.Log("Everything seems clean, rotating");
        return true;
    }

    /// <summary>
    /// Tests if an object will be safe after moving with <paramref name="move"/> vector and rotating <paramref name="angle"/> degrees
    /// </summary>
    /// <param name="move"> Vector to move the object </param>
    /// <param name="angle"> Euler's angle to rotate the object </param>
    /// <returns> true if safe, false otherwise </returns>
    private bool IsRotationWithMoveSafe(Vector3 move, int angle)
    {
        foreach (Transform child in transform)
        {
            var point = Utils.RotatePointAroundPivot(
                child.position + move,
                _pivot.position + move,
                Quaternion.AngleAxis(angle, transform.parent.forward).eulerAngles
            );
            // If something still collides after rotation and move - cant move that way
            if (Physics.OverlapBox(point, _childExtents, child.rotation, ObstacleLayerMask).Length != 0)
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
                child.position + direction,
                _childExtents,
                child.rotation,
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
        Debug.Log($"{gameObject} Scheduled to settle! @{transform.parent}");
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

    private int NumberOfCollisions()
    {
        var count = 0;

        foreach (Transform child in transform)
        {
            if (Physics.OverlapBox(
                child.position,
                _childExtents,
                child.rotation,
                ObstacleLayerMask).Length != 0)
            {
                count++;
            }
        }

        return count;
    }

    private void OnEnable()
    {
        ChangeInputControllerSubscription(true);
        if (_playingFieldState != null)
        {
            _playingFieldState.OnFocusChangedEvent += ChangeInputControllerSubscription;
        }
    }

    private void OnDisable()
    {
        ChangeInputControllerSubscription(false);
        _playingFieldState.OnFocusChangedEvent -= ChangeInputControllerSubscription;
    }

    private void ChangeInputControllerSubscription(bool shouldListen)
    {
        if (shouldListen)
        {
            InputController.OnMoveEvent += OnMove;
            InputController.OnRotateEvent += OnRotate;
            InputController.OnDropDownEvent += OnDropBlock;
        }
        else
        {
            InputController.OnMoveEvent -= OnMove;
            InputController.OnRotateEvent -= OnRotate;
            InputController.OnDropDownEvent -= OnDropBlock;
        }
    }

    // Old Debug
    //private void OnDrawGizmos()
    //{
    //    Debug.Log($"Drawing gizmos for {gameObject} @{transform.parent} with {_extents}");
    //    Gizmos.matrix = transform.localToWorldMatrix;
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireCube(Vector3.zero, _extents * 2);
    //}
}
