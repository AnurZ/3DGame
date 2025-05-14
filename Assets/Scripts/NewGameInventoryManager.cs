using UnityEngine;
using UnityEngine.SceneManagement;

public class NewGameInventoryManager : MonoBehaviour
{
    public CurrencyManager currencyManager;

    private void Start()
    {
        if (SaveManager.IsNewGame)
        {
            Debug.Log("✅ Default inventar kreiran nakon učitavanja nove scene.");
            CreateDefaultInventory();
            SaveManager.IsNewGame = false;
        }
        else
        {
            Debug.Log("📦 Loadani podaci iz save-a.");
            SceneManager.sceneLoaded += OnSceneLoaded; // Registriraj za logiranje nakon učitavanja scene
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Ovdje se logira samo kada je scena učitana
        Debug.Log("📦 Loadani podaci iz save-a.");
        SceneManager.sceneLoaded -= OnSceneLoaded; // Odjava od eventa

        
    }

    public void CreateDefaultInventory()
    {
        if (InventoryManager.Instance == null)
        {
            Debug.LogWarning("⚠️ InventoryManager nije inicijaliziran!");
            return;
        }

        var slots = InventoryManager.Instance.inventorySlots;

        // Očisti inventar prije nego što dodamo nove default iteme
        foreach (var slot in slots)
        {
            var inv = slot.GetComponentInChildren<InventoryItem>();
            if (inv != null) Destroy(inv.gameObject);
        }

        // Definiraj default iteme za početak
        string[] defaultItemNames = { "AxeLevel1" };
        int[] defaultAmounts = { 1 };

        // Dodaj default iteme u inventar
        for (int i = 0; i < defaultItemNames.Length && i < slots.Length; i++)
        {
            Item item = Resources.Load<Item>("Items/" + defaultItemNames[i]);
            if (item != null)
            {
                var go = Instantiate(InventoryManager.Instance.inventoryItemPrefab, slots[i].transform);
                var invItem = go.GetComponent<InventoryItem>();
                invItem.InitialiseItem(item, defaultAmounts[i]);
                invItem.item.name = defaultItemNames[i]; // Osiguraj da item ima naziv
            }
            else
            {
                Debug.LogWarning($"Default item '{defaultItemNames[i]}' nije pronađen u Resources/Items/");
            }
        }

        // Dodaj početni novac
        currencyManager.CurrentMoney = 10;
        currencyManager.SetMoneyText();

        FindObjectOfType<SimpleDayNightCycle>().SetFirstDay();

    }


}
