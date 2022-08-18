using UnityEngine;

public class GridController : MonoBehaviour
{
    public int GridWidth = 13;
    public int GridHeight = 20;
    [SerializeField]
    private GameObject gridPrefab;

    void Start()
    {
        var verticalOffset = GridWidth % 2 == 0 ? 0 : -0.5f;
        var verticalPosition = new Vector3(transform.position.x + verticalOffset - GridWidth / 2, transform.position.y + GridHeight / 2);
        var verticalScale = new Vector3(0.1f, GridHeight, 0.1f);
        for (int i = 0; i <= GridWidth; i++)
        {
            var line = Instantiate(gridPrefab);
            line.transform.parent = transform;
            line.name = $"Vertical Grid Line {i + 1}";
            line.transform.localScale = verticalScale;
            line.transform.position = verticalPosition;
            verticalPosition.Set(verticalPosition.x + 1, verticalPosition.y, verticalPosition.z);
        }

        var horizontalPosition = transform.position;
        var horizontalScale = new Vector3(GridWidth, 0.1f, 0.1f);
        for (int i = 0; i <= GridHeight; i++)
        {
            var line = Instantiate(gridPrefab);
            line.transform.parent = transform;
            line.name = $"Horizontal Grid Line {i + 1}";
            line.transform.localScale = horizontalScale;
            line.transform.position = horizontalPosition;
            horizontalPosition.Set(horizontalPosition.x, horizontalPosition.y + 1, horizontalPosition.z);
        }
    }
}
