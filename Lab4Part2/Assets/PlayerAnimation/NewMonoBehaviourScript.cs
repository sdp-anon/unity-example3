using UnityEngine;
using UnityEngine.SceneManagement;
public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float speed = 5f;
    [SerializeField] private AudioSource runSound;

    private Vector3 movement;
    private SpriteRenderer sprite;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        float input = Input.GetAxis("Horizontal");

        movement.x = input * speed * Time.deltaTime;
        transform.Translate(movement);

        animator.SetBool("isRunning", input != 0);

        // Flip character
        if (input > 0)
            sprite.flipX = false;
        else if (input < 0)
            sprite.flipX = true;

        // Play running sound
        if (input != 0)
        {
            if (!runSound.isPlaying)
                runSound.Play();
        }
        else
        {
            runSound.Stop();
        }
    }
}