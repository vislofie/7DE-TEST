using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public class SaveLoad : MonoBehaviour
{
    private string _filePath;

    private void Awake()
    {
        _filePath = Application.persistentDataPath + "/save.bsv";
    }

    private void Start()
    {
        Save loadData = LoadData();
        if (loadData == null)
            return;

        Save.ItemUIData[] itemData = loadData.PlayerDataSaved.itemData;
        FP_Inventory inventory = FP_Inventory.Instance;

        for (int i = 0; i < itemData.Length; i++)
        {
            inventory.AddItem(itemData[i].id, itemData[i].slot);
        }

        Transform playerTransform = FindObjectOfType<FP_Controller>().transform;
        Vector3 pos = new Vector3(loadData.PlayerDataSaved.position.x, loadData.PlayerDataSaved.position.y, loadData.PlayerDataSaved.position.z);
        Vector3 eulerAngles = new Vector3(loadData.PlayerDataSaved.eulerAngles.x, loadData.PlayerDataSaved.eulerAngles.y, loadData.PlayerDataSaved.eulerAngles.z);

        playerTransform.position = pos;
        playerTransform.eulerAngles = eulerAngles;

        List<Save.ItemObjData> itemObjData = loadData.ItemDataSaved;

        GameObject[] prefabs = Resources.LoadAll<GameObject>("Prefabs");
        Array.Sort(prefabs, delegate (GameObject x, GameObject y) { return int.Parse(x.name.Split('_')[0]).CompareTo(int.Parse(y.name.Split('_')[0])); });
        
        if (itemObjData.Count > 0)
        {
            Collectable[] existingCollectables = FindObjectsOfType<Collectable>();
            for (int i = existingCollectables.Length - 1; i >= 0; i--)
            {
                Destroy(existingCollectables[i].gameObject);
            }
        }

        for (int i = 0; i < itemObjData.Count; i++)
        {
            pos = new Vector3(itemObjData[i].position.x, itemObjData[i].position.y, itemObjData[i].position.z);
            Instantiate(prefabs[itemObjData[i].id], pos, Quaternion.identity);
        }

    }

    private void OnApplicationQuit()
    {
        GameObject player = FindObjectOfType<FP_Controller>().gameObject;
        Collectable[] collectables = FindObjectsOfType<Collectable>();

        SaveData(player, collectables);
    }

    public void SaveData(GameObject player, Collectable[] collectables)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fs = new FileStream(_filePath, FileMode.Create);

        Save save = new Save();
        save.SavePlayer(player);
        save.SaveItems(collectables);

        bf.Serialize(fs, save);
        fs.Close();

    }

    public Save LoadData()
    {
        if (File.Exists(_filePath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = new FileStream(_filePath, FileMode.Open);

            Save save = (Save)bf.Deserialize(fs);

            fs.Close();

            return save;
        }
        return null;
    }

}

[System.Serializable]
public class Save
{
    [System.Serializable]
    public struct Vec3
    {
        public float x;
        public float y;
        public float z;

        public Vec3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }

    [System.Serializable]
    public struct ItemUIData
    {
        public int id;
        public int slot;

        public ItemUIData(int id, int slot)
        {
            this.id = id;
            this.slot = slot;
        }
    }

    [System.Serializable]
    public struct ItemObjData
    {
        public Vec3 position;
        public int id;

        public ItemObjData(Vec3 position, int id)
        {
            this.position = position;
            this.id = id;
        }
    }

    [System.Serializable]
    public struct PlayerData
    {
        public Vec3 position;
        public Vec3 eulerAngles;

        public ItemUIData[] itemData;

        public PlayerData(Vec3 position, Vec3 eulerAngles, ItemUIData[] itemData)
        {
            this.position = position;
            this.eulerAngles = eulerAngles;
            this.itemData = itemData;
        }
    }

    public PlayerData PlayerDataSaved => _playerData;
    public List<ItemObjData> ItemDataSaved => _itemData;

    private PlayerData _playerData;
    private List<ItemObjData> _itemData = new List<ItemObjData>();

    public void SavePlayer(GameObject player)
    {
        ItemUIData[] itemData = new ItemUIData[FP_Inventory.Instance.InventoryItems.Count];

        for (int i = 0; i < itemData.Length; i++)
        {
            Item curItem = FP_Inventory.Instance.InventoryItems[i];
            itemData[i] = new ItemUIData(curItem.ID, curItem.Slot);
        }

        _playerData = new PlayerData(new Vec3(player.transform.position.x, player.transform.position.y, player.transform.position.z),
                                    new Vec3(player.transform.eulerAngles.x, player.transform.eulerAngles.y, player.transform.eulerAngles.z),
                                    itemData);
    }

    public void SaveItems(Collectable[] collectables)
    {
        for (int i = 0; i < collectables.Length; i++)
        {
            _itemData.Add(new ItemObjData(new Vec3(collectables[i].transform.position.x, collectables[i].transform.position.y, collectables[i].transform.position.z),
                                         collectables[i].ItemID));
        }
    }
}
