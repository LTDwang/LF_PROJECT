using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ManufactureCellUI : MonoBehaviour, IPointerClickHandler
{
    public int x;
    public int y;

    [HideInInspector]
    public ManufactureGridView owner;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() =>
            {
                if (owner != null)
                    owner.OnCellClicked(this);
            });
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (owner != null)
            owner.OnCellClicked(this);
    }
}
