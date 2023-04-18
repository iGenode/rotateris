using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField]
    private InputAction _pauseAction;
    [SerializeField]
    private GameObject _pauseMenu;
    [SerializeField]
    private Button _firstSelectedButton;

    public void GoToMainMenu()
    {
        Utils.GameToMenu();
    }

    private void PauseCallback(InputAction.CallbackContext context)
    {
        _pauseMenu.SetActive(!GameState.IsGamePaused);
        if (!GameState.IsGamePaused)
        {
            _firstSelectedButton.Select();
        }
        GameState.PauseOrResume(!GameState.IsGamePaused);
    }

    private void OnEnable()
    {
        _pauseAction.Enable();
        _pauseAction.performed += PauseCallback;
    }

    private void OnDisable()
    {
        _pauseAction.performed -= PauseCallback;
        _pauseAction.Disable();
    }
}
