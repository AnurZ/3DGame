using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class SaveManager : MonoBehaviour
{
    public GameObject player;
    public CurrencyManager currencyManager;

    private string _path => Path.Combine(Application.persistentDataPath, "savefile.json");

    private void Start()
    {
        Invoke(nameof(LoadGame), 0.1f); // delay da se sve Awake()/Start() odrade
    }

    private void OnApplicationQuit()
    {
        SaveGame(); // u buildu radi uvijek
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) SaveGame();
        if (Input.GetKeyDown(KeyCode.L)) LoadGame();
    }

    public void SaveGame()
    {
        if (player == null || currencyManager == null || InventoryManager.Instance == null)
        {
            Debug.LogWarning("SaveManager: nedostaju reference!");
            return;
        }

        var slots = InventoryManager.Instance.inventorySlots;
        var data = new PlayerSaveData
        {
            posX = player.transform.position.x,
            posY = player.transform.position.y,
            posZ = player.transform.position.z,
            rotY = player.transform.eulerAngles.y,
            currentMoney = currencyManager.CurrentMoney,
            // inicijalno popuni praznim stringovima
            inventoryItems = new List<string>(new string[slots.Length])
        };

        // Za svaki slot upiši ime itema ili ostavi ""
        for (int i = 0; i < slots.Length; i++)
        {
            var inv = slots[i].GetComponentInChildren<InventoryItem>();
            if (inv != null && inv.item != null && !string.IsNullOrEmpty(inv.item.itemName))
                data.inventoryItems[i] = inv.item.itemName;
            // inače ostaje ""
        }

        // Snimi JSON
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(_path, json);
        Debug.Log($"SaveManager: sačuvano → {_path}\n{json}");
    }

    public void LoadGame()
    {
        if (!File.Exists(_path))
        {
            Debug.LogWarning($"SaveManager: nema save file na {_path}");
            return;
        }

        string json = File.ReadAllText(_path);
        var data = JsonUtility.FromJson<PlayerSaveData>(json);
        Debug.Log($"SaveManager: učitano →\n{json}");

        // Pozicija
        player.transform.position  = new Vector3(data.posX, data.posY, data.posZ);
        player.transform.eulerAngles = new Vector3(0, data.rotY, 0);

        // Novac
        currencyManager.CurrentMoney = data.currentMoney;
        currencyManager.SetMoneyText();

        // Očisti sve stare iteme iz slotova
        var slots = InventoryManager.Instance.inventorySlots;
        foreach (var slot in slots)
        {
            var inv = slot.GetComponentInChildren<InventoryItem>();
            if (inv != null) Destroy(inv.gameObject);
        }

        // Učitaj nazive iz data.inventoryItems, jedan po jedan slot
        for (int i = 0; i < slots.Length && i < data.inventoryItems.Count; i++)
        {
            string name = data.inventoryItems[i];
            if (!string.IsNullOrEmpty(name))
            {
                Item item = Resources.Load<Item>("Items/" + name);
                if (item != null)
                {
                    // instantiate pod slot
                    var go = Instantiate(
                        InventoryManager.Instance.inventoryItemPrefab,
                        slots[i].transform
                    );
                    go.GetComponent<InventoryItem>().InitialiseItem(item);
                }
                else
                {
                    Debug.LogWarning($"SaveManager: '{name}' nije u Resources/Items/");
                }
            }
        }
    }
}

[System.Serializable]
public class PlayerSaveData
{
    public float posX, posY, posZ;
    public float rotY;
    public int currentMoney;
    public List<string> inventoryItems; // duljine = broj slotova
}
