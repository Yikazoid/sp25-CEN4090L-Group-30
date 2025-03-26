using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float acceleration = 10f;
    public float jumpForce = 10f;
    public float airDeceleration = 5f;
    [Tooltip("The gravity multiplier added when jump is released early")]
    public float jumpEndEarlyGravityModifier = 3f;
    [Tooltip("The time before coyote jump becomes unusable")]
    public float coyoteTime = 0.15f;
    [Tooltip("The amount of time we buffer a jump")]
    public float jumpBuffer = 0.2f;
    [Range(0f, 1f)]
    public float frictionFactor = 0.9f;

    [Header("Ground Check")]
    public LayerMask groundLayer;
    public Transform groundCheck;

    private Rigidbody2D rb;
    private float time;
    private bool jumpToConsume;
    private float timeJumpWasPressed;
    private bool grounded;
    private float frameLeftGrounded = float.MinValue;
    private bool endedJumpEarly;
    private float inputX;
    private Vector2 frameVelocity;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        time += Time.deltaTime;
        inputX = Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonDown("Jump"))
        {
            jumpToConsume = true;
            timeJumpWasPressed = time;
        }
    }

    private void FixedUpdate()
    {
        frameVelocity = rb.linearVelocity;
        CheckGround();
        HandleJump();
        HandleHorizontal();
        HandleGravity();
        ApplyMovement();
        HandleSpriteFlip();
    }

    private void CheckGround()
    {
        Vector2 pointA = (Vector2)groundCheck.position - new Vector2(0.2f, 0.2f);
        Vector2 pointB = (Vector2)groundCheck.position + new Vector2(0.2f, 0.2f);
        Collider2D[] colliders = Physics2D.OverlapAreaAll(pointA, pointB, groundLayer);
        bool wasGrounded = grounded;
        grounded = colliders.Length > 0;
        if (grounded && !wasGrounded)
            endedJumpEarly = false;
        else if (!grounded && wasGrounded)
            frameLeftGrounded = time;
    }

    private void HandleJump()
    {
        bool hasBufferedJump = jumpToConsume && (time < timeJumpWasPressed + jumpBuffer);
        bool canUseCoyote = !grounded && (time < frameLeftGrounded + coyoteTime);
        if (hasBufferedJump && (grounded || canUseCoyote))
        {
            frameVelocity.y = jumpForce;
            jumpToConsume = false;
        }
        if (!endedJumpEarly && !grounded && !Input.GetButton("Jump") && frameVelocity.y > 0)
            endedJumpEarly = true;
    }

    private void HandleHorizontal()
    {
        float targetVelocityX = inputX * moveSpeed;
        if (Mathf.Abs(inputX) > 0.01f)
            frameVelocity.x = Mathf.MoveTowards(frameVelocity.x, targetVelocityX, acceleration * Time.fixedDeltaTime);
        else if (grounded)
            frameVelocity.x *= frictionFactor;
        else
            frameVelocity.x = Mathf.MoveTowards(frameVelocity.x, targetVelocityX, airDeceleration * Time.fixedDeltaTime);
    }

    private void HandleGravity()
    {
        float gravityMultiplier = (!grounded && endedJumpEarly && frameVelocity.y > 0) ? jumpEndEarlyGravityModifier : 1f;
        frameVelocity.y += Physics2D.gravity.y * gravityMultiplier * Time.fixedDeltaTime;
    }

    private void ApplyMovement()
    {
        rb.linearVelocity = frameVelocity;
    }

    private void HandleSpriteFlip()
    {
        if (inputX > 0f)
            transform.localScale = new Vector3(1f, 1f, 1f);
        else if (inputX < 0f)
            transform.localScale = new Vector3(-1f, 1f, 1f);
    }
}
