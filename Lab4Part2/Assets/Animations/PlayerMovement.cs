using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float speed = 5f;

    // 2. Declare the movement vector
    private Vector3 movement;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
    }
    private void HandleMovement()
    {
        // input will store a value between -1 and +1
        // GetAxisRaw() takes exactly -1 or +1
        // GetAxis() takes a value between and up to -1 to +1 (useful for acceleration)
        // Getting the axis is mapped to A/D, left/right arrow and joystick left/right
        float input = Input.GetAxis("Horizontal");
        movement.x = input * speed * Time.deltaTime;
        transform.Translate(movement);
    
        if (input != 0)
        {
            animator.SetBool("IsRunning", true);
        }
        else
        {
            animator.SetBool("IsRunning", false);
        }
    }
}
