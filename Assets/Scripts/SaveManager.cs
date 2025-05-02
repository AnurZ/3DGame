using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public class SaveManager : MonoBehaviour
{
    public static bool IsNewGame = false;

    private int previousMoney;
    private int[] previousItemCounts;
    private Vector3 previousPlayerPosition;

    public GameObject player;
    public CurrencyManager currencyManager;

    private string _path => Path.Combine(Application.persistentDataPath, "savefile.json");

    private void Start()
    {
        previousMoney = currencyManager.CurrentMoney;
        previousPlayerPosition = player.transform.position;
        previousItemCounts = new int[InventoryManager.Instance.inventorySlots.Length];
        UpdateItemCounts();

        if (!IsNewGame)
            Invoke(nameof(LoadGame), 0.1f);
        else
            Debug.Log("🟡 Novi game – preskačem LoadGame()");
    }

    private void UpdateItemCounts()
    {
        for (int i = 0; i < InventoryManager.Instance.inventorySlots.Length; i++)
        {
            var inv = InventoryManager.Instance
                        .inventorySlots[i]
                        .GetComponentInChildren<InventoryItem>();
            previousItemCounts[i] = inv != null ? inv.count : 0;
        }
    }

    private void OnApplicationQuit()
    {
        SaveGame();
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

        // Pripremi podatke
        var slots = InventoryManager.Instance.inventorySlots;
        var data = new PlayerSaveData
        {
            posX = player.transform.position.x,
            posY = player.transform.position.y,
            posZ = player.transform.position.z,
            rotY = player.transform.eulerAngles.y,
            currentMoney = currencyManager.CurrentMoney,
            inventoryItems = new List<InventoryItemData>()
        };

        // Napuni listu itema
        foreach (var slot in slots)
        {
            var inv = slot.GetComponentInChildren<InventoryItem>();
            if (inv != null && inv.item != null && !string.IsNullOrEmpty(inv.item.name))
            {
                data.inventoryItems.Add(new InventoryItemData
                {
                    itemName = inv.item.name,
                    amount = inv.count
                });
            }
            else
            {
                data.inventoryItems.Add(new InventoryItemData
                {
                    itemName = "",
                    amount = 0
                });
            }
        }

        // Zapiši JSON
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(_path, json);
        Debug.Log($"SaveManager: sačuvano → {_path}\n{json}");

        // Ažuriraj praćene vrijednosti
        previousMoney = currencyManager.CurrentMoney;
        previousPlayerPosition = player.transform.position;
        UpdateItemCounts();
    }

    private bool HasGameStateChanged()
    {
        if (currencyManager.CurrentMoney != previousMoney)
        {
            Debug.Log("Detektirana promjena novca.");
            return true;
        }

        if (player.transform.position != previousPlayerPosition)
        {
            Debug.Log("Detektirana promjena pozicije igrača.");
            return true;
        }

        var slots = InventoryManager.Instance.inventorySlots;
        for (int i = 0; i < slots.Length; i++)
        {
            var inv = slots[i].GetComponentInChildren<InventoryItem>();
            int count = inv != null ? inv.count : 0;
            if (count != previousItemCounts[i])
            {
                Debug.Log($"Detektirana promjena inventara u slotu {i}: {previousItemCounts[i]} → {count}");
                return true;
            }
        }

        return false;
    }

    public void LoadGame()
    {
        if (IsNewGame)
        {
            Debug.Log("🟢 Nova igra, preskačem učitavanje podataka.");
            return;
        }

        if (!File.Exists(_path))
        {
            Debug.LogWarning($"SaveManager: nema save file na {_path}");
            FindObjectOfType<NewGameInventoryManager>()?.CreateDefaultInventory();
            return;
        }

        // Čitaj JSON
        string json = File.ReadAllText(_path);
        var data = JsonUtility.FromJson<PlayerSaveData>(json);
        Debug.Log($"SaveManager: učitano →\n{json}");

        // Postavi player i novac
        player.transform.position = new Vector3(data.posX, data.posY, data.posZ);
        player.transform.eulerAngles = new Vector3(0, data.rotY, 0);
        currencyManager.CurrentMoney = data.currentMoney;
        currencyManager.SetMoneyText();

        // **Temeljito očisti sve child objekte svakog slota**
        var slots = InventoryManager.Instance.inventorySlots;
        foreach (var slot in slots)
        {
            // Sakupi listu child objektata
            var toDestroy = slot.transform
                                  .Cast<Transform>()
                                  .Select(t => t.gameObject)
                                  .ToList();
            // Uništi ih sve
            foreach (var go in toDestroy)
                Destroy(go);
        }

        // Ponovo instanciraj samo iteme iz save fajla
        for (int i = 0; i < slots.Length && i < data.inventoryItems.Count; i++)
        {
            var itemData = data.inventoryItems[i];
            if (string.IsNullOrEmpty(itemData.itemName) || itemData.amount <= 0)
                continue;

            var item = Resources.Load<Item>($"Items/{itemData.itemName}");
            if (item != null)
            {
                var go = Instantiate(
                    InventoryManager.Instance.inventoryItemPrefab,
                    slots[i].transform
                );
                go.GetComponent<InventoryItem>().InitialiseItem(item, itemData.amount);
            }
            else
            {
                Debug.LogWarning($"SaveManager: '{itemData.itemName}' nije pronađen u Resources/Items/");
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
    public List<InventoryItemData> inventoryItems;
}

[System.Serializable]
public class InventoryItemData
{
    public string itemName;
    public int amount;
}
