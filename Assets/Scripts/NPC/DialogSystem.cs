using TMPro;
using UnityEngine;

public class DialogSystem : MonoBehaviour
{
    public TextMeshProUGUI dialogText; // Text component
    public GameObject dialogPanel;

    private void Start()
    {
        dialogPanel.SetActive(false);
    }

    public void ShowDialog(string dialog)
    {
        dialogPanel.SetActive(true);
        dialogText.text = dialog;
    }

    public void CloseDialog()
    {
        dialogPanel.SetActive(false);
    }
}