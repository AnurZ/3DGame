using UnityEngine;

public class ShopTrigger : MonoBehaviour
{
    private bool playerInside = false;
    [SerializeField] private ZoomTransition zoomTransition;

    void Start()
    {
        zoomTransition = FindObjectOfType<ZoomTransition>();
    }

    void Update()
    {
        if (playerInside && Input.GetKeyDown(KeyCode.Space))
        {
            zoomTransition.ZoomToShop();
            if (PlayerController.Local != null && PlayerController.Local.gameObject.activeInHierarchy)
                PlayerController.Local.isInShop = true;
            
            var pc = PlayerController.Local;
            pc.isInShop = true;

            // **immediately hide** any prompt
            pc.uiPanel.SetActive(false);
            pc.interactionText.text = "";
            
            PlayerController.Local.SetVisible(false);
            PlayerController.Local.EnterShop();
        }

        if (playerInside && Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("ESC pritisnut - pokušavam zoom u shop.");
            zoomTransition.ZoomBackToPlayer();
            if (PlayerController.Local != null)
                PlayerController.Local.isInShop = false;

            PlayerController.Local.SetVisible(true);
            PlayerController.Local.ExitShop();


        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player je ušao u shop trigger.");
            playerInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            FindObjectOfType<PlayerController>().isInShop = false;
            zoomTransition.ZoomBackToPlayer();
        }
    }
}