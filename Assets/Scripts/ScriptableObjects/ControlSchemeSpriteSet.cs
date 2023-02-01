using UnityEngine;

[CreateAssetMenu(fileName = "New Control Scheme Sprite Set", menuName = "ScriptableObjects/ControlSchemeSpriteSet", order = 1)]
public class ControlSchemeSpriteSet : ScriptableObject
{
    [Header("Control Scheme Name")]
    [Tooltip("Should match one of the defined control schemes")]
    public string ControlSchemeName;

    [Header("Shape Rotation")]
    public Sprite RotateLeftSprite;
    public Sprite RotateRightSprite;

    [Header("Playing Field Rotation")]
    public Sprite ChangePlayingFieldLeftSprite;
    public Sprite ChangePlayingFieldRightSprite;

    [Header("Shape Drop")]
    public Sprite DropShapeSprite;

    [Header("Shape Hold")]
    public Sprite HoldShapeSprite;
}
