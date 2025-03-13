using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed; // Player movement speed
    private Vector2 move; // Store movement input
    private Animator animator; // Reference to Animator component

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>(); // Get the Animator component from the player object
    }

    // This function is called by the input system when the player moves
    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>(); // Read movement input
    }

    // This function is called by the input system when the spacebar is pressed or released
    public void OnChop(InputAction.CallbackContext context)
    {
        if (context.performed)  
        {
            Debug.Log("Chop action triggered!");
            animator.SetTrigger("Chop");  // This assumes "Chop" is a Trigger in Animator
        }
        if (context.performed) // Spacebar pressed
        {
            animator.SetBool("IsChopping", true); // Start chopping animation
        }
        else if (context.canceled) // Spacebar released
        {
            animator.SetBool("IsChopping", false); // Stop chopping animation
        }
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer(); // Move the player
        UpdateAnimations(); // Update the animation state
    }

    // Function to handle player movement
    public void MovePlayer()
    {
        Vector3 movement = new Vector3(move.x, 0f, move.y);  // Create a movement vector

        // Rotate the player to face the direction of movement
        if (movement != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), 0.15f);

        // Move the player in the specified direction
        transform.Translate(movement * speed * Time.deltaTime, Space.World);
    }

    // Function to update the animations based on player movement and input
    private void UpdateAnimations()
    {
        // Check if the movement vector is non-zero (i.e., player is moving)
        if (move != Vector2.zero)
        {
            animator.SetBool("IsWalking", true); // Set walking animation true
        }
        else
        {
            animator.SetBool("IsWalking", false); // Set walking animation false
        }
    }
}
