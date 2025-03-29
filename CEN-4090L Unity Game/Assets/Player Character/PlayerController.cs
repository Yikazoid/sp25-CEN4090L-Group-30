using System.Collections;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 3f;
    public float sprintSpeed = 5f;
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
    private float moveSpeed = 3f;
    private bool jumpToConsume;
    private float timeJumpWasPressed;
    private bool grounded;
    private bool isSprint;
    private float frameLeftGrounded = float.MinValue;
    private bool endedJumpEarly;
    private float inputX;
    private int direction;
    private Vector2 frameVelocity;
    private Animator animator;
    private string currentAnimation = "";

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        //Check Movement
        time += Time.deltaTime;
        inputX = Input.GetAxisRaw("Horizontal");
        if (inputX == 1)
            direction = 1;
        else if (inputX == -1)
            direction = -1;

        //Check for Sprint
        if (grounded && Input.GetButtonDown("Sprint"))
        {
            moveSpeed = sprintSpeed;
            isSprint = true;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            moveSpeed = walkSpeed;
            isSprint = false;
        }       

        //Check for Jump
        if (grounded && Input.GetButtonDown("Jump"))
        {
            if (direction >= 1)
                ChangeAnimation("JumpR1_Grapple");
            else if (direction <= -1)
                ChangeAnimation("JumpL1_Grapple");

            jumpToConsume = true;
            timeJumpWasPressed = time;
        }
        else
            CheckAnimation();
    }
    public void ChangeAnimation(string animation, float crossfade = 0.15f, float time = 0)
    {
        if (time > 0) StartCoroutine(Wait());
        else Validate();

        IEnumerator Wait()
        {
            yield return new WaitForSeconds(time - crossfade);
            Validate();
        }

        void Validate()
        {
            if(currentAnimation != animation)
            {
                currentAnimation = animation;

                if (currentAnimation == "")
                    CheckAnimation();
                else
                    animator.CrossFade(animation, crossfade);
            }
        }
    }

    private void CheckAnimation()
    {
        if (currentAnimation == "JumpR1_Grapple" ||
            currentAnimation == "JumpL1_Grapple")
            return;

        if(direction == 1)
        {
            if (inputX == 0)
                ChangeAnimation("IdleR_Grapple");
            else if(inputX > 0)
            {
                if (isSprint == true)
                    ChangeAnimation("SprintR_Grapple");
                else
                    ChangeAnimation("WalkR_Grapple");
            }
        }else if(direction == -1)
        {
            if (inputX == 0)
                ChangeAnimation("IdleL_Grapple");
            else if (inputX < 0)
            {
                if (isSprint == true)
                    ChangeAnimation("SprintL_Grapple");
                else
                    ChangeAnimation("WalkL_Grapple");
            }
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
        //HandleSpriteFlip();
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
        print(targetVelocityX);
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

    /*private void HandleSpriteFlip()
    {
        if (inputX > 0f)
            transform.localScale = new Vector3(1f, 1f, 1f);
        else if (inputX < 0f)
            transform.localScale = new Vector3(-1f, 1f, 1f);
    }*/
}
