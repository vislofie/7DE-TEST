using UnityEngine;
using UnityEngine.EventSystems;

public class FP_RaycastManager : MonoBehaviour
{
    [SerializeField]
    private FP_Inventory _inventory;

    [SerializeField]
    private LayerMask _raycastMask;
    [SerializeField]
    private float _pickingUpDistance = 5.0f;

    Ray savedray;

    private void Update()
    {

        if (Input.touchCount > 0 && (!EventSystem.current.IsPointerOverGameObject() || EventSystem.current.currentSelectedGameObject == null))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            savedray = ray;
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, _pickingUpDistance, _raycastMask))
            {
                Collectable col;
                if (hit.collider.TryGetComponent<Collectable>(out col))
                {
                    if (_inventory.AddItem(col.ItemID))
                    {
                        Destroy(hit.collider.gameObject);
                        Debug.Log("succ");
                    }
                    Debug.Log("no succ");
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(savedray);
    }
}
