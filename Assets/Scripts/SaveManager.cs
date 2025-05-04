using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections;


public class SaveManager : MonoBehaviour
{
    public static bool IsNewGame = false;
    private bool isLoading = false; 

    [Header("References")]
    public UpgradesManager upgradesManager;  // Postavi u Inspectoru
    public GameObject player;
    public CurrencyManager currencyManager;

    // Praćene vrijednosti za detekciju promjena
    private int previousMoney;
    private int[] previousItemCounts;
    private Vector3 previousPlayerPosition;
    private bool[] previousUpgrades;

    // Putanja do fajla
    private string _path => Path.Combine(Application.persistentDataPath, "savefile.json");

    private IEnumerator Start()
    {
        // Ako nije ručno postavljeno, pronađi automatski
        if (upgradesManager == null)
            upgradesManager = FindObjectOfType<UpgradesManager>();

        previousMoney = currencyManager.CurrentMoney;
        previousPlayerPosition = player.transform.position;
        previousItemCounts = new int[InventoryManager.Instance.inventorySlots.Length];
        UpdateItemCounts();

        // Koristimo interne flagove iz UpgradesManagera
        previousUpgrades = GetCurrentUpgradeStates();

        yield return null;  
        if (!IsNewGame)
            LoadGame();
        else
            Debug.Log("🟡 Novi game – preskačem LoadGame()");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) SaveGame();
        if (Input.GetKeyDown(KeyCode.L)) LoadGame();

        // Automatski save ako se išta promijenilo
        
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private void UpdateItemCounts()
    {
        var slots = InventoryManager.Instance.inventorySlots;
        for (int i = 0; i < slots.Length; i++)
        {
            var inv = slots[i].GetComponentInChildren<InventoryItem>();
            previousItemCounts[i] = inv != null ? inv.count : 0;
        }
    }

    private bool[] GetCurrentUpgradeStates()
    {
        if (upgradesManager == null)
            return new bool[7];

        return new bool[]
        {
            upgradesManager.hasStaminaRegen,
            upgradesManager.hasSpeed,
            upgradesManager.hasInjuryShield,
            upgradesManager.hasSevereInjuryShield,
            upgradesManager.hasPotionEffect,
            upgradesManager.hasChoppingSpeed,
            upgradesManager.hasChoppingStamina
        };
    }

    public void SaveGame()
    {
        
        
        if (player == null || currencyManager == null || InventoryManager.Instance == null)
        {
            Debug.LogWarning("SaveManager: nedostaju reference!");
            return;
        }
        
        if (isLoading)
            return;

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

        // Spremi inventar
        foreach (var slot in slots)
        {
            var inv = slot.GetComponentInChildren<InventoryItem>();
            data.inventoryItems.Add(
                inv != null && inv.item != null && !string.IsNullOrEmpty(inv.item.name)
                    ? new InventoryItemData { itemName = inv.item.name, amount = inv.count }
                    : new InventoryItemData { itemName = "", amount = 0 }
            );
        }

        // Spremi upgradeove iz internog stanja
        bool[] ups = GetCurrentUpgradeStates();
        data.staminaRegenUpgrade       = ups[0];
        data.speedUpgrade              = ups[1];
        data.injuryShieldUpgrade       = ups[2];
        data.severeInjuryShieldUpgrade = ups[3];
        data.potionEffectUpgrade       = ups[4];
        data.choppingSpeedUpgrade      = ups[5];
        data.choppingStaminaUpgrade    = ups[6];

        // Zapiši JSON
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(_path, json);
        Debug.Log($"SaveManager: sačuvano → {_path}\n{json}");

        // Ažuriraj praćene vrijednosti
        previousMoney          = currencyManager.CurrentMoney;
        previousPlayerPosition = player.transform.position;
        UpdateItemCounts();
        previousUpgrades       = ups;
    }

    private bool HasGameStateChanged()
    {
        if (currencyManager.CurrentMoney != previousMoney) return true;
        if (player.transform.position != previousPlayerPosition) return true;

        // Inventar
        var slots = InventoryManager.Instance.inventorySlots;
        for (int i = 0; i < slots.Length; i++)
        {
            var inv = slots[i].GetComponentInChildren<InventoryItem>();
            int count = inv != null ? inv.count : 0;
            if (count != previousItemCounts[i]) return true;
        }

        // Upgradeovi
        bool[] ups = GetCurrentUpgradeStates();
        for (int i = 0; i < ups.Length; i++)
            if (ups[i] != previousUpgrades[i]) return true;

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
        
        isLoading = true; 

        string json = File.ReadAllText(_path);
        var data = JsonUtility.FromJson<PlayerSaveData>(json);
        Debug.Log($"SaveManager: učitano →\n{json}");

        // Postavi player i novac
        player.transform.position    = new Vector3(data.posX, data.posY, data.posZ);
        player.transform.eulerAngles = new Vector3(0, data.rotY, 0);
        currencyManager.CurrentMoney = data.currentMoney;
        currencyManager.SetMoneyText();

        // Očisti i reinstanciraj inventar
        var slots = InventoryManager.Instance.inventorySlots;
        foreach (var slot in slots)
            foreach (Transform t in slot.transform.Cast<Transform>().ToList())
                Destroy(t.gameObject);

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

        // Učitaj upgradeove pomoću Buy… metoda
        if (upgradesManager != null)
        {
            if (data.staminaRegenUpgrade)       upgradesManager.BuyStaminaRegenUpgrade();
            if (data.speedUpgrade)              upgradesManager.BuySpeedUpgrade();
            if (data.injuryShieldUpgrade)       upgradesManager.BuyInjuryShieldUpgrade();
            if (data.severeInjuryShieldUpgrade) upgradesManager.BuySevereInjuryShieldUpgrade();
            if (data.potionEffectUpgrade)       upgradesManager.BuyPotionEffectUpgrade();
            if (data.choppingSpeedUpgrade)      upgradesManager.BuyChoppingSpeedUpgrade();
            if (data.choppingStaminaUpgrade)    upgradesManager.BuyChoppingStaminaUpgrade();
        }

        // Osvježi praćene vrijednosti
        previousMoney          = currencyManager.CurrentMoney;
        previousPlayerPosition = player.transform.position;
        UpdateItemCounts();
        previousUpgrades       = GetCurrentUpgradeStates();
        isLoading = false; 
    }
}


// Klase za JSON
[System.Serializable]
public class PlayerSaveData
{
    public float posX, posY, posZ;
    public float rotY;
    public int currentMoney;
    public List<InventoryItemData> inventoryItems;

    // Upgrade booleani
    public bool staminaRegenUpgrade;
    public bool speedUpgrade;
    public bool injuryShieldUpgrade;
    public bool severeInjuryShieldUpgrade;
    public bool potionEffectUpgrade;
    public bool choppingSpeedUpgrade;
    public bool choppingStaminaUpgrade;
}

[System.Serializable]
public class InventoryItemData
{
    public string itemName;
    public int amount;
}
