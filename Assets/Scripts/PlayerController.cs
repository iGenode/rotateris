using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // TODO: when instantiating a player give it an index, that index corresponds to players position and angle
    // TODO: 360 / index is an angle at which the players repeat, angle * index is the coordinate of the player

    private GameState _gameState;
    private float _xBound = 5;
    private Vector2 _move;
    // TODO: have a game script that determines speed, moveUnit, playing field amount

    // Start is called before the first frame update
    void Start()
    {
        _gameState = GameObject.Find("Game Controller").GetComponent<GameState>();

    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x < -_xBound)
        {
            transform.position = new(-_xBound, transform.position.y, transform.position.z);
        }
        else if (transform.position.x > _xBound)
        {
            transform.position = new(_xBound, transform.position.y, transform.position.z);
        }

        transform.Translate(_move.x * Vector3.right);
        transform.Translate(_gameState.Speed * GameState.MoveUnit * Time.deltaTime * Vector3.down);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // TODO: start timer for rotation/movement of the brick, settle this player and spawn another?
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _move = context.ReadValue<Vector2>();
    }
    
    public void OnRotateLeft(InputAction.CallbackContext context)
    {

    }

    public void OnRotateRight(InputAction.CallbackContext context)
    {

    }
}
