using UnityEngine;

public class TicketPurchase : MonoBehaviour
{
    public int ticketPrice = 1000;  // Cijena tiketa
    public GameObject dialoguePanel;  // UI panel za dijalog
    public CurrencyManager currencyManager;  // Referenca na tvoj CurrencyManager

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (currencyManager.CurrentMoney >= ticketPrice)
            {
                ShowDialogue();
            }
            else
            {
                // Ako nema dovoljno novca, prikaži obavijest
//                 Debug.Log("Nemate dovoljno novca za tiket!");
            }
        }
    }

    void ShowDialogue()
    {
        // Prikazivanje dijaloga
        dialoguePanel.SetActive(true);
        // Aktiviraj opciju za kupovinu tiketa i završavanje dijaloga
    }
}
