using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public float Speed = 1;
    // fixme: set this at the start/pass from menu scene
    public int PlayingFields = 1;
    public const int MoveUnit = 1;
    public int Score = 0;
    public float angle;

    // Start is called before the first frame update
    void Start()
    {
        angle = 360 / PlayingFields;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
