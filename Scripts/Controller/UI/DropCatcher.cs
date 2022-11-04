using UnityEngine;
using UnityEngine.EventSystems;

public class DropCatcher : MonoBehaviour, IDropHandler
{


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

                    FP_Inventory.Instance.DropItem(slot.CurrentItem);
                }
            }
        }
    }
}
