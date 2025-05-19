using UnityEngine;

public class NPCInteractable : MonoBehaviour
{
    public float interactionDistance = 3f;
    public string npcName = "Shopkeeper";

    [TextArea]
    public string[] dialogLines; // Niz rečenica za prikaz

    public DialogSystem dialogSystem;

    private bool dialogStarted = false;

    private void Update()
    {
        if (Vector3.Distance(transform.position, Camera.main.transform.position) < interactionDistance)
        {
            if (Input.GetKeyDown(KeyCode.F) && !dialogStarted)
            {
                StartDialog();
            }
        }
        else
        {
            dialogStarted = false;
        }
    }

    private void StartDialog()
    {
        dialogStarted = true;
//         Debug.Log($"Talking to {npcName}");

        dialogSystem.dialogLines = dialogLines;
        dialogSystem.enabled = true; // osiguraj da je komponenta aktivna
        dialogSystem.SendMessage("OnTriggerEnter", Camera.main.GetComponent<Collider>(), SendMessageOptions.DontRequireReceiver);
    }
}
