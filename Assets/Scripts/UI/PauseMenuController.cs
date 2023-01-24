using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField]
    private InputAction _pauseAction;
    [SerializeField]
    private GameObject _pauseMenu;

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    private void PauseCallback(InputAction.CallbackContext context)
    {
        _pauseMenu.SetActive(!GameState.IsGamePaused);
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
