using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    public float walkSpeed = 5f;
    public float runSpeed = 7f;
    public float jumpForce = 16f;
    public float lowJumpGravity = 30f;
    public float fallGravity = 50f;
    private Rigidbody rb;
    private Vector3 movement;

    [HideInInspector] public bool bloqueado = false;

    [Header("Sprites")]
    public SpriteRenderer spriteRenderer;
    public Sprite[] upSprites;
    public Sprite[] downSprites;
    public Sprite[] rightSprites;

    private float animationTimer = 0f;
    public float walkAnimationInterval = 0.2f;
    public float runAnimationInterval = 0.1f;
    private int animationFrame = 0;
    private int lastDirection = 0;

    private bool isGrounded = false;
    private int jumpCount = 0;
    public int maxJumps = 2;
    private bool jumpRequested = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (PlayerPositionMemory.hasSavedPosition)
            return;

        if (CheckpointManager.instance != null &&
            CheckpointManager.instance.shouldRespawn &&
            CheckpointManager.instance.currentCheckpoint != null)
        {
            transform.position = CheckpointManager.instance.GetRespawnPosition();
            CheckpointManager.instance.shouldRespawn = false;
        }
    }

    void Update()
    {
        if (bloqueado)
        {
            movement = Vector3.zero;
            return;
        }

        float x = 0f;
        float z = 0f;

        if (Keyboard.current.wKey.isPressed) z += 1;
        if (Keyboard.current.sKey.isPressed) z -= 1;
        if (Keyboard.current.aKey.isPressed) x -= 1;
        if (Keyboard.current.dKey.isPressed) x += 1;

        bool isRunning = Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed;
        float speed = isRunning ? runSpeed : walkSpeed;
        float animationInterval = isRunning ? runAnimationInterval : walkAnimationInterval;

        movement = new Vector3(x, 0f, z).normalized * speed;

        if (Keyboard.current.spaceKey.wasPressedThisFrame && jumpCount < maxJumps)
            jumpRequested = true;

        if (movement.magnitude > 0.01f)
        {
            if (Mathf.Abs(x) > Mathf.Abs(z))
                lastDirection = x > 0 ? 2 : 3;
            else if (Mathf.Abs(z) > 0)
                lastDirection = z > 0 ? 1 : 0;

            animationTimer += Time.deltaTime;
            if (animationTimer >= animationInterval)
            {
                animationFrame = (animationFrame + 1) % 4;
                animationTimer = 0f;
            }
        }
        else
        {
            animationFrame = 0;
            animationTimer = 0f;
        }

        switch (lastDirection)
        {
            case 0:
                spriteRenderer.sprite = downSprites[animationFrame];
                spriteRenderer.flipX = false;
                break;
            case 1:
                spriteRenderer.sprite = upSprites[animationFrame];
                spriteRenderer.flipX = false;
                break;
            case 2:
                spriteRenderer.sprite = rightSprites[animationFrame];
                spriteRenderer.flipX = false;
                break;
            case 3:
                spriteRenderer.sprite = rightSprites[animationFrame];
                spriteRenderer.flipX = true;
                break;
        }
    }

    void FixedUpdate()
    {
        if (bloqueado)
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            return;
        }

        Vector3 velocity = rb.linearVelocity;
        velocity.x = movement.x;
        velocity.z = movement.z;
        rb.linearVelocity = velocity;

        if (jumpRequested && jumpCount < maxJumps)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpCount++;
            jumpRequested = false;
        }
        else
        {
            jumpRequested = false;
        }

        if (rb.linearVelocity.y > 0.1f)
            rb.AddForce(Vector3.down * lowJumpGravity, ForceMode.Acceleration);
        else if (rb.linearVelocity.y < -0.1f)
            rb.AddForce(Vector3.down * fallGravity, ForceMode.Acceleration);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            jumpCount = 0;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }
}





