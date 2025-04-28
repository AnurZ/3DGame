using UnityEngine;

public class NPCInteractable : MonoBehaviour
{
    public float interactionDistance = 3f;
    public string npcName = "Shopkeeper"; // Ime NPC-a
    public string dialog = "Welcome to my shop!";
    public DialogSystem dialogSystem;
    
    private void Update()
    {
        if (Vector3.Distance(transform.position, Camera.main.transform.position) < interactionDistance)
        {
            // Prikazivanje indikatora da igrač može pritisnuti 'F' za razgovor
            if (Input.GetKeyDown(KeyCode.F))
            {
                OpenDialog();
                dialogSystem.ShowDialog(dialog);
            }
        }
    }

    private void OpenDialog()
    {
        // Ovdje možeš otvoriti dijalog sistem
        Debug.Log($"Talking to {npcName}");
    }
}