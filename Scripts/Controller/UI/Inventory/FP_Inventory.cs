using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum ItemID { Ball = 0, DartTranquilizer = 1, Dart = 2, Firehose = 3, Handle = 4, Hypnotic = 5, KeyACast = 6, KeyA = 7, KeyCard = 8,
                     CigarettePack = 9, Picklock = 10, Pilers = 11, RockA = 12, RockB = 13, RockC = 14, RockD = 15,
                     Sledgehammer = 16, Soap = 17, SoapCast = 18, StepLadder = 19, Tablets = 20, Valve = 21, 
                     Watch = 22, WD = 23, Wrench = 24, Wrowolf = 25}

public class Item
{
    public int ID => _id;
    public int Slot => _slot;
    public Sprite ItemSprite => _sprite;

    private int _id;
    private int _slot;
    private Sprite _sprite;

    public Item()
    {
        _id = -1;
        _slot = -1;
        _sprite = null;
    }

    public Item(int id, int slot, Sprite sprite)
    {
        _id = id;
        _slot = slot;
        _sprite = sprite;
    }

    public void ChangeSlotID(int slotID)
    {
        _slot = slotID;
    }
}

public class FP_Inventory : MonoBehaviour
{
    private static FP_Inventory _instance;
    public static FP_Inventory Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<FP_Inventory>();
            return _instance;
        }
    }

    private FP_InventoryCell[] _inventoryCells;
    
    [SerializeField]
    private LayerMask _dropLayerMask;

    public List<Item> InventoryItems => _inventoryItems;
    private List<Item> _inventoryItems = new List<Item>();
    private GameObject[] _itemPrefabs;
    private Sprite[] _sprites;

    private void Awake()
    {
        _inventoryCells = GetComponentsInChildren<FP_InventoryCell>();

        _itemPrefabs = Resources.LoadAll<GameObject>("Prefabs");
        _sprites = Resources.LoadAll<Sprite>("Sprites");

        Array.Sort(_sprites, delegate (Sprite x, Sprite y) { return int.Parse(x.name.Split('_')[0]).CompareTo(int.Parse(y.name.Split('_')[0])); });
        Array.Sort(_itemPrefabs, delegate (GameObject x, GameObject y) { return int.Parse(x.name.Split('_')[0]).CompareTo(int.Parse(y.name.Split('_')[0])); });
    }

    private void UpdateVisualInfo()
    {
        foreach (FP_InventoryCell cell in _inventoryCells)
        {
            cell.Emptify();
        }

        foreach (Item item in _inventoryItems)
        {
            FP_InventoryCell cell = _inventoryCells[item.Slot].GetComponent<FP_InventoryCell>();
            cell.Fill(item);
        }
    }

    private int GetFirstFreeSlotID()
    {
        for (int i = 0; i < _inventoryCells.Length; i++)
            if (_inventoryCells[i].Free)
                return i;
        return -1;
    }

    /// <summary>
    /// Returns true if adding item was successful
    /// </summary>
    /// <param name="itemID">id of an item to be added</param>
    /// <returns></returns>
    public bool AddItem(int itemID)
    {
        int firstFreeSlot = GetFirstFreeSlotID();

        if (firstFreeSlot != -1)
        {
            Item item = new Item(itemID, firstFreeSlot, _sprites[itemID]);
            _inventoryItems.Add(item);

            UpdateVisualInfo();

            return true;
        }

        return false;
    }

    public bool AddItem(int itemID, int slot)
    {
        for (int i = 0; i < _inventoryItems.Count; i++)
        {
            if (_inventoryItems[i].Slot == slot)
            {
                return false;
            }
        }

        Item item = new Item(itemID, slot, _sprites[itemID]);
        _inventoryItems.Add(item);

        UpdateVisualInfo();

        return true;
    }

    /// <summary>
    /// Removes a given item
    /// </summary>
    /// <param name="item">item to remove</param>
    public void RemoveItem(Item item)
    {
        if (_inventoryItems.Contains(item))
        {
            int itemIDInList = -1;
            for (int i = 0; i < _inventoryItems.Count; i++)
            {
                if (_inventoryItems[i].Equals(item))
                {
                    itemIDInList = i;
                }
            }

            if (itemIDInList != -1)
            {
                _inventoryItems.Remove(item);

                UpdateVisualInfo();
            }
        }
    }


    public void OnCellClick(int cellID)
    {
        foreach (FP_InventoryCell inventoryCell in _inventoryCells)
        {
            inventoryCell.DeactivateFrame();
        }

        if (_inventoryCells.Length > 0)
        {
            _inventoryCells[cellID].ActivateFrame();
        }
    }

    /// <summary>
    /// Swaps two existent items in the inventory
    /// </summary>
    /// <param name="firstItem"></param>
    /// <param name="secondItem"></param>
    public void CombineItems(Item firstItem, Item secondItem)
    {
        if (_inventoryItems.Contains(firstItem) && _inventoryItems.Contains(secondItem) && !firstItem.Equals(secondItem))
        {
            int firstItemSlot = firstItem.Slot;

            RemoveItem(firstItem);
            RemoveItem(secondItem);

            AddItem(5, firstItemSlot);

            UpdateVisualInfo();
        }
    }

    public void MoveItemToSlot(int beforeSlotID, int afterSlotID)
    {
        FP_InventoryCell beforeCell = _inventoryCells[beforeSlotID].GetComponent<FP_InventoryCell>();
        FP_InventoryCell afterCell = _inventoryCells[afterSlotID].GetComponent<FP_InventoryCell>();

        Item passingItem = beforeCell.CurrentItem;

        beforeCell.Emptify();
        afterCell.Fill(passingItem);
        passingItem.ChangeSlotID(afterSlotID);

        UpdateVisualInfo();
    }

    public void DropItem(Item item)
    {
        Vector3 itemPos;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.GetTouch(0).position), out hit, 3.0f, _dropLayerMask))
        {
            itemPos = hit.point + Vector3.up * 0.5f;
        }
        else
        {
            itemPos = Camera.main.transform.position + Camera.main.transform.forward * 3.0f;
        }
        Instantiate(_itemPrefabs[item.ID], itemPos, Quaternion.identity);
        RemoveItem(item);
    }
}
