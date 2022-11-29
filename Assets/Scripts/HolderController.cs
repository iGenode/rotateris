using UnityEngine;
using UnityEngine.InputSystem;

public class HolderController : MonoBehaviour
{
    [SerializeField]
    private InputAction _holdAction;
    private PlayerController _heldPlayer;
    private Vector3 _holderPreviewPosition = new(0, 1000, 0);

    private void OnHold(InputAction.CallbackContext context)
    {
        var spawnManager = GameState.GetFocusedSpawnManager();
        if (spawnManager.CanHold())
        {
            if (_heldPlayer)
            {
                var oldPrefabName = _heldPlayer.gameObject.name;
                Destroy(_heldPlayer.gameObject);
                HoldCurrentPlayer();
                spawnManager.InstantiateNewPlayerWithName(oldPrefabName);
            }
            else
            {
                HoldCurrentPlayer();
                spawnManager.InstantiateNewPlayer();
            }
            spawnManager.SetHoldFlag(false);
        }
    }

    private void HoldCurrentPlayer()
    {
        var currentPlayer = GameState.GetFocusedSpawnManager().GetCurrentPlayer();
        currentPlayer.CancelSettle();
        currentPlayer.transform.position = _holderPreviewPosition;
        currentPlayer.transform.rotation = Quaternion.identity;
        currentPlayer.enabled = false;
        _heldPlayer = currentPlayer;
        GameState.GetFocusedSpawnManager().DiscardCurrentPlayerController();
    }

    private void OnEnable()
    {
        _holdAction.Enable();
        _holdAction.performed += OnHold;
    }

    private void OnDisable()
    {
        _holdAction.Disable();
        _holdAction.performed -= OnHold;
    }
}
