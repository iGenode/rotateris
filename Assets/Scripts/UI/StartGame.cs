using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    [SerializeField]
    private GameObject _errorPopup;
    [SerializeField]
    private PlayingFieldSettings _settings;

    public void PlayGame()
    {
        if (_settings.PlayingFieldCount > 0)
        {
            Utils.MenuToGame();
        }
        else
        {
            StartCoroutine(ShowPopupForSeconds(5));
        }
    }

    private IEnumerator ShowPopupForSeconds(int seconds)
    {
        _errorPopup.SetActive(true);
        yield return new WaitForSeconds(seconds);
        _errorPopup.SetActive(false);
    }
}
