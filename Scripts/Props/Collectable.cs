using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    public int ItemID => _itemID;
    [SerializeField]
    private int _itemID;
}
