using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// fixme: any mouse movement switches control scheme, which resets held controls (stick left/right) on gamepad

/// <summary>
/// A Singleton MonoBehaviour class for managing ControlsView's icons
/// </summary>
public class ControlsViewController : MonoBehaviour
{
    public static string LastControlScheme = "Keyboard&Mouse";

    [Header("Shape Rotation Images")]
    public Image RotateLeftImage;
    public Image RotateRightImage;

    [Header("Field Rotation Images")]
    public Image ChangeFieldLeftImage;
    public Image ChangeFieldRightImage;

    [Header("Drop Shape Image")]
    public Image DropShapeImage;

    [Header("Hold Shape Image")]
    public Image HoldShapeImage;

    [Header("Sprite Sets")]
    [SerializeField]
    private ControlSchemeSpriteSet[] _controlSchemeSpriteSets;

    [Header("Playing Field Settings")]
    [SerializeField]
    private PlayingFieldSettings _playingFieldSettings;

    // Singleton instances
    private static ControlsViewController _instance = null;

    public static ControlsViewController Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        // Retrieving last control scheme to prevent switching icons
        LastControlScheme = _playingFieldSettings.LastControlScheme;
        SearchForSpriteSetAndApply();
    }

    // Callback method for changing control scheme icons
    public void ChangeControlsIcons(PlayerInput input)
    {
        if (input.currentControlScheme == LastControlScheme)
        {
            return;
        }

        LastControlScheme = input.currentControlScheme;
        _playingFieldSettings.LastControlScheme = input.currentControlScheme;

        SearchForSpriteSetAndApply();
    }

    // Method for searching for a sprite set and applying it
    private void SearchForSpriteSetAndApply()
    {
        foreach (ControlSchemeSpriteSet spriteSet in _controlSchemeSpriteSets)
        {
            if (spriteSet.ControlSchemeName == LastControlScheme)
            {
                ApplySpriteSet(spriteSet);
                return;
            }
        }
    }

    // Method for applying sprites from sprite set
    private void ApplySpriteSet(ControlSchemeSpriteSet spriteSet)
    {
        RotateLeftImage.sprite = spriteSet.RotateLeftSprite;
        RotateRightImage.sprite = spriteSet.RotateRightSprite;

        ChangeFieldLeftImage.sprite = spriteSet.ChangePlayingFieldLeftSprite;
        ChangeFieldRightImage.sprite = spriteSet.ChangePlayingFieldRightSprite;

        DropShapeImage.sprite = spriteSet.DropShapeSprite;

        HoldShapeImage.sprite = spriteSet.HoldShapeSprite;
    }

}
