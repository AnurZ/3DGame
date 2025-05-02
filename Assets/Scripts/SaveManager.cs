using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
    public GameObject player;

    private void Start()
    {
        Invoke(nameof(LoadGame), 0.1f); // Mali delay da se sve učita
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            SaveGame();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadGame();
        }
    }

    public void SaveGame()
    {
        if (player == null)
        {
            Debug.LogWarning("Cannot save: Player reference missing!");
            return;
        }

        PlayerSaveData data = new PlayerSaveData
        {
            posX = player.transform.position.x,
            posY = player.transform.position.y,
            posZ = player.transform.position.z,
            rotY = player.transform.eulerAngles.y
        };

        string json = JsonUtility.ToJson(data, true);
        string path = Application.persistentDataPath + "/savefile.json";
        Debug.Log("Saving to path: " + path);
        File.WriteAllText(path, json);
    }

    public void LoadGame()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PlayerSaveData data = JsonUtility.FromJson<PlayerSaveData>(json);

            if (player == null)
            {
                Debug.LogWarning("Player reference not set. Trying to find...");
                player = GameObject.FindWithTag("Player");
                if (player == null)
                {
                    Debug.LogError("No GameObject with tag 'Player' found!");
                    return;
                }
            }

            player.transform.position = new Vector3(data.posX, data.posY, data.posZ);
            player.transform.eulerAngles = new Vector3(0, data.rotY, 0);

            Debug.Log("Loaded player position: " + player.transform.position);
        }
        else
        {
            Debug.LogWarning("Save file not found at " + path);
        }
    }
}

[System.Serializable]
public class PlayerSaveData
{
    public float posX, posY, posZ;
    public float rotY;
}
