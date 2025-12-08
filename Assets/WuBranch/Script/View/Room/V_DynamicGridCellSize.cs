using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup)), RequireComponent(typeof(RectTransform))]
public class V_DynamicGridCellSize : MonoBehaviour
{
    private RectTransform rectTransform;
    private GridLayoutGroup gridLayoutGroup;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        gridLayoutGroup = GetComponent<GridLayoutGroup>();
    }

    void OnEnable()
    {
        // Recalculate cell size when the object becomes active
        UpdateCellSize();
    }

    // Call this method whenever the parent's size changes or you want to recalculate
    public void UpdateCellSize()
    {
        if (gridLayoutGroup.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
        {
            float parentWidth = rectTransform.rect.width;
            Debug.Log($"width: {parentWidth}");
            int columnCount = gridLayoutGroup.constraintCount;
            float totalSpacing = gridLayoutGroup.spacing.x * (columnCount - 1);
            float totalPadding = gridLayoutGroup.padding.left + gridLayoutGroup.padding.right;

            float availableWidth = parentWidth - totalSpacing - totalPadding;
            float cellSizeX = availableWidth / columnCount;

            gridLayoutGroup.cellSize = new Vector2(cellSizeX, gridLayoutGroup.cellSize.y);
        }
        else
        {
            Debug.LogWarning("FlexibleGridWidth script requires GridLayoutGroup constraint to be FixedColumnCount.");
        }
    }
}