
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
    public PotionManager potionManager;
    public UpgradesManager upgradesManager;  // Postavi u Inspectoru
    public GameObject player;
    public CurrencyManager currencyManager;
    public TreeSpawner treeSpawner;

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
        treeSpawner = FindObjectOfType<TreeSpawner>();

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
        
        // debug start
        Debug.Log($"[LoadGame DEBUG] player={(player==null?"NULL":"OK")}, currencyManager={(currencyManager==null?"NULL":"OK")}");
        Debug.Log($"[LoadGame DEBUG] InventoryManager.Instance={(InventoryManager.Instance==null?"NULL":"OK")}, slots count={(InventoryManager.Instance?.inventorySlots?.Length.ToString() ?? "N/A")}");
        Debug.Log($"[LoadGame DEBUG] upgradesManager={(upgradesManager==null?"NULL":"OK")}, upMgr.playerController={(upgradesManager?.playerController==null?"NULL":"OK")}, upMgr.staminaController={(upgradesManager?.staminaController==null?"NULL":"OK")}, upMgr.potionManager={(upgradesManager?.potionManager==null?"NULL":"OK")}");


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

        data.staminaRegenUpgrade       = upgradesManager.hasStaminaRegen;
        data.speedUpgrade              = upgradesManager.hasSpeed;
        data.injuryShieldUpgrade       = upgradesManager.hasInjuryShield;
        data.severeInjuryShieldUpgrade = upgradesManager.hasSevereInjuryShield;
        data.potionEffectUpgrade       = upgradesManager.hasPotionEffect;
        data.choppingSpeedUpgrade      = upgradesManager.hasChoppingSpeed;
        data.choppingStaminaUpgrade    = upgradesManager.hasChoppingStamina;

        // Debug prije pisanja:
        Debug.Log($"[SaveManager] sprema upgradeove → " +
                  $"staminaRegen={data.staminaRegenUpgrade}, speed={data.speedUpgrade}, " +
                  $"injury={data.injuryShieldUpgrade}, severe={data.severeInjuryShieldUpgrade}, " +
                  $"potion={data.potionEffectUpgrade}, chopSpeed={data.choppingSpeedUpgrade}, " +
                  $"chopStam={data.choppingStaminaUpgrade}");
        
        // Zapiši JSON
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(_path, json);
        Debug.Log($"SaveManager: sačuvano → {_path}\n{json}");

        // Ažuriraj praćene vrijednosti
        previousMoney          = currencyManager.CurrentMoney;
        previousPlayerPosition = player.transform.position;
        UpdateItemCounts();
        previousUpgrades       = ups;
        
        treeSpawner.SaveSpawnedTrees();
        Debug.Log("Trees saved!");
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
    // --- Početni debug ---
    //Debug.Log($"[LoadGame] player={(player==null?"NULL":"OK")}, currencyManager={(currencyManager==null?"NULL":"OK")}, InventoryManager={(InventoryManager.Instance==null?"NULL":"OK")}, upgradesManager={(upgradesManager==null?"NULL":"OK")}");
    
    if (IsNewGame)
    {
        //Debug.Log("🟢 Nova igra – preskačem učitavanje.");
        var player = FindObjectOfType<PlayerController>()?.gameObject;
        if (player != null)
        {
            player.transform.position = new Vector3(70f, 0f, 185f); // <-- PROMIJENI AKO IMAŠ KONKRETNU LOKACIJU
            player.transform.eulerAngles = Vector3.zero;
            Debug.Log("🟢 Postavljena početna pozicija igrača za novu igru.");
        }
        else
        {
            Debug.LogWarning("⚠️ Igrač nije pronađen prilikom postavljanja pozicije!");
        }

        return;
    }
    if (!File.Exists(_path))
    {
        Debug.LogWarning($"SaveManager: nema save file na {_path}");
        FindObjectOfType<NewGameInventoryManager>()?.CreateDefaultInventory();
        return;
    }

    isLoading = true;
    var json = File.ReadAllText(_path);
    var data = JsonUtility.FromJson<PlayerSaveData>(json);
    Debug.Log($"SaveManager: učitano →\n{json}");

    // 1) Player
    if (player != null)
    {
        //Debug.Log("[LoadGame] Setting player transform");
        player.transform.position    = new Vector3(data.posX, data.posY, data.posZ);
        player.transform.eulerAngles = new Vector3(0, data.rotY, 0);
    }
    //else Debug.LogError("[LoadGame] player = NULL");

    // 2) Money
    if (currencyManager != null)
    {
        //Debug.Log("[LoadGame] Setting currency");
        currencyManager.CurrentMoney = data.currentMoney;
        currencyManager.SetMoneyText();
    }
    //else Debug.LogError("[LoadGame] currencyManager = NULL");

    // 3) Inventory
    if (InventoryManager.Instance != null)
    {
      //  Debug.Log("[LoadGame] Clearing inventory");
        var slots = InventoryManager.Instance.inventorySlots;
        foreach (var slot in slots)
            foreach (Transform t in slot.transform.Cast<Transform>().ToList())
                Destroy(t.gameObject);

       // Debug.Log("[LoadGame] Recreating inventory items");
        for (int i = 0; i < slots.Length && i < data.inventoryItems.Count; i++)
        {
            var it = data.inventoryItems[i];
            if (string.IsNullOrEmpty(it.itemName) || it.amount <= 0) continue;
            var item = Resources.Load<Item>($"Items/{it.itemName}");
            if (item != null)
            {
                var go = Instantiate(
                    InventoryManager.Instance.inventoryItemPrefab,
                    slots[i].transform
                );
                go.GetComponent<InventoryItem>().InitialiseItem(item, it.amount);
            }
           // else Debug.LogWarning($"[LoadGame] Item '{it.itemName}' nije pronađen");
        }
    }
    //else Debug.LogError("[LoadGame] InventoryManager.Instance = NULL");

    // 4) Flags
    if (upgradesManager != null)
    {
       // Debug.Log("[LoadGame] Applying upgrade flags");
        upgradesManager.hasStaminaRegen       = data.staminaRegenUpgrade;
        upgradesManager.hasSpeed              = data.speedUpgrade;
        upgradesManager.hasInjuryShield       = data.injuryShieldUpgrade;
        upgradesManager.hasSevereInjuryShield = data.severeInjuryShieldUpgrade;
        upgradesManager.hasPotionEffect       = data.potionEffectUpgrade;
        upgradesManager.hasChoppingSpeed      = data.choppingSpeedUpgrade;
        upgradesManager.hasChoppingStamina    = data.choppingStaminaUpgrade;
    }
    //else Debug.LogError("[LoadGame] upgradesManager = NULL");

    // 5) Effects
    if (upgradesManager != null)
    {
        //Debug.Log("[LoadGame] Applying upgrade effects");
        // Stamina Regen
        if (upgradesManager.hasStaminaRegen)
        {
            if (upgradesManager.playerController != null)
                upgradesManager.playerController.isStaminaRegenUpgrade = true;
            //else Debug.LogError("[LoadGame] playerController = NULL");
        }
        // Speed
        if (upgradesManager.hasSpeed)
        {
            if (upgradesManager.playerController != null)
                upgradesManager.playerController.speed = 10f;
            //else Debug.LogError("[LoadGame] playerController = NULL");
        }
        // Injury Shield
        if (upgradesManager.hasInjuryShield)
        {
            if (upgradesManager.playerController != null)
                upgradesManager.playerController.injuryShieldUpgrade = true;
           // else Debug.LogError("[LoadGame] playerController = NULL");
        }
        // Severe
        if (upgradesManager.hasSevereInjuryShield)
        {
            if (upgradesManager.playerController != null)
                upgradesManager.playerController.severeInjuryShieldUpgrade = true;
           // else Debug.LogError("[LoadGame] playerController = NULL");
        }
        // Potion Effect
        if (upgradesManager.potionManager == null)
            upgradesManager.potionManager = FindObjectOfType<PotionManager>();

        
        if (upgradesManager.hasPotionEffect)
        {
            if (upgradesManager.potionManager != null)
                upgradesManager.potionManager.PotionEffectUpgradeBought = true;
           // else Debug.LogError("[LoadGame] potionManager = NULL");
        }
        // Chopping Speed
        if (upgradesManager.hasChoppingSpeed)
        {
            if (upgradesManager.playerController != null)
                upgradesManager.playerController.choppingSpeedUpgrade = true;
           // else Debug.LogError("[LoadGame] playerController = NULL");
        }
        // Chopping Stamina
        if (upgradesManager.hasChoppingStamina)
        {
            if (upgradesManager.staminaController != null)
                upgradesManager.staminaController.staminaReductionRate *= 0.8f;
           // else Debug.LogError("[LoadGame] staminaController = NULL");
        }
    }

    // 6) UI
    if (upgradesManager != null)
    {
        //Debug.Log("[LoadGame] Refreshing upgrade UI");
        upgradesManager.RefreshAllUpgradesUI();
    }
    //else Debug.LogError("[LoadGame] upgradesManager = NULL at UI refresh");

    // 7) Trackers
    previousMoney          = currencyManager?.CurrentMoney ?? previousMoney;
    previousPlayerPosition = player?.transform.position ?? previousPlayerPosition;
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
