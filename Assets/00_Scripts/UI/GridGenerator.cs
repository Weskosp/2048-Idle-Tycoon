using UnityEngine;
using UnityEngine.UI;

public class GridGenerator : MonoBehaviour
{
    [SerializeField] private int _rows;
    [SerializeField] private int _columns;
    [SerializeField] private int _gridPad;
    [SerializeField] private float _cellSpace;

    private float _cellSize;

    public int RowValue => _rows;
    public int ColumnValue => _columns;

    [ContextMenu("Grid Oluştur")]
    void GenerateGrid()
    {
        if (transform.childCount == _columns * _rows) return;

        GameObject gridUI = transform.Find("Grid").gameObject;
        GameObject visualGridUI = transform.Find("VisualGrid").gameObject;

        GridManager gridManager = GetComponent<GridManager>();

        GridLayoutGroup grid = gridUI.GetComponent<GridLayoutGroup>();
        RectTransform gridRectTransform = gridUI.GetComponent<RectTransform>();

        GridLayoutGroup visualGrid = visualGridUI.GetComponent<GridLayoutGroup>();

        _cellSize = (gridRectTransform.rect.width - (2 * _gridPad) - ((_columns - 1) * _cellSpace)) / _columns;

        grid.padding.left = _gridPad;
        grid.padding.right = _gridPad;
        grid.padding.top = _gridPad;
        grid.padding.bottom = _gridPad;

        grid.cellSize = new Vector2(_cellSize, _cellSize);
        grid.spacing = new Vector2(_cellSpace, _cellSpace);

        visualGrid.padding.left = _gridPad;
        visualGrid.padding.right = _gridPad;
        visualGrid.padding.top = _gridPad;
        visualGrid.padding.bottom = _gridPad;

        visualGrid.cellSize = new Vector2(_cellSize, _cellSize);
        visualGrid.spacing = new Vector2(_cellSpace, _cellSpace);

        for (int i = 0; i < (_columns * _rows); i++)
        {
            GameObject newCell = new GameObject{name = "Cell_" + i};
            GameObject newVisualCell = new GameObject{name = "VisualCell_" + i};

            newCell.transform.SetParent(gridUI.transform, false);
            newCell.AddComponent<RectTransform>();
            
            newVisualCell.transform.SetParent(visualGridUI.transform, false);
            newVisualCell.AddComponent<RectTransform>();
            newVisualCell.AddComponent<Image>();
            
            gridManager.AddCellPositionToList(newCell);
        }
    }
}