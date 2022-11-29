using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewSpawnManager : MonoBehaviour
{
    private float _offsetFromWorldCenter;
    private Vector3 _cameraOffset = new(0, 35, 0);
    private Quaternion _cameraRotation = Quaternion.Euler(30, 0, 0);

    [SerializeField]
    private GameObject _previewFieldPrefab;
    [SerializeField]
    private PlayingFieldSettings _settings;
    [SerializeField]
    private Camera _mainCamera;

    private readonly List<GameObject> _previewFields = new();

    public void SpawnPreviewForCurrentSettings()
    {
        DestroyOldFields();
        
        var fieldCount = _settings.PlayingFieldCount;
        _offsetFromWorldCenter = 15 + 13 * (fieldCount / 5);
        _cameraOffset.z = -20 - _offsetFromWorldCenter;
        _mainCamera.transform.position = _cameraOffset;
        _mainCamera.transform.rotation = _cameraRotation;

        var angleStep = fieldCount > 0 ? (float) 360 / fieldCount : 0;
        var defaultSpawnPos = new Vector3(0, 0, -_offsetFromWorldCenter);

        for (int i = 0; i < fieldCount; i++)
        {
            var angle = new Vector3(0, angleStep * i, 0);
            var spawnPoint = Utils.RotatePointAroundPivot(defaultSpawnPos, Vector3.zero, angle);
            var previewField = Instantiate(_previewFieldPrefab, spawnPoint, _previewFieldPrefab.transform.rotation);
            previewField.name = $"Preview field {i}";
            previewField.transform.Rotate(angle);

            _previewFields.Add(previewField);
        }
    }

    private void DestroyOldFields()
    {
        foreach (GameObject field in _previewFields)
        {
            Destroy(field);
        }
        _previewFields.Clear();
    }
}
