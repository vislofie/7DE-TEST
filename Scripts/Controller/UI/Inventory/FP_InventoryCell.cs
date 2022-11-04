using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FP_InventoryCell : MonoBehaviour, IDropHandler
{
    [SerializeField]
    private Image _iconImage;
    private FP_InventoryIcon _icon;
    [SerializeField]
    private Image _imageFrame;

    [SerializeField]
    private Color _activeCellColor;
    [SerializeField]
    private Color _inactiveCellColor;

    private FP_Inventory _inventory;

    public int CellID => _cellID;
    public bool Free => _free;
    public Item CurrentItem => _currentItem;

    private int _cellID;

    private bool _free;
    private Item _currentItem;
    private Item _defaulItem;

    private void Awake()
    {
        _free = true;
        _defaulItem = new Item();
        _icon = _iconImage.GetComponent<FP_InventoryIcon>();

        _cellID = int.Parse(transform.name.Replace("Cell_", "")) - 1;

        _inventory = transform.GetComponentInParent<FP_Inventory>();
    }

    public void ActivateFrame()
    {
        if (_imageFrame != null)
            _imageFrame.color = _activeCellColor;
    }

    public void DeactivateFrame()
    {
        if (_imageFrame != null)
            _imageFrame.color = _inactiveCellColor;
    }

    public void Fill(Item item)
    {
        _currentItem = item;

        _iconImage.sprite = item.ItemSprite;
        _iconImage.color = new Color(1, 1, 1, 1);

        _icon.Activate();

        _free = false;
    }

    public void Emptify()
    {
        _currentItem = _defaulItem;

        _iconImage.sprite = null;
        _iconImage.color = new Color(1, 1, 1, 0);

        _icon.Deactivate();

        _free = true;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            FP_InventoryIcon icon;
            if (eventData.pointerDrag.TryGetComponent<FP_InventoryIcon>(out icon))
            {
                if (icon.Interactable)
                {
                    FP_InventoryCell slot = eventData.pointerDrag.GetComponentInParent<FP_InventoryCell>();

                    if (_free)
                    {
                        _inventory.MoveItemToSlot(slot.CellID, _cellID);
                        return;
                    }

                    _inventory.CombineItems(_currentItem, slot.CurrentItem);
                }
            }
        }
    }
}
