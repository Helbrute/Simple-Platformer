using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Controller : MonoBehaviour
{
    // Player Components
    private Rigidbody2D rb;
    private Animator anim;
    private CapsuleCollider2D capsCollider;
    private SpriteRenderer spriteRenderer;
    public Sprite sprite;

    // Player Variables
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float jumpPower = 10f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] float baseGravity = 5f;
    [SerializeField] float horizontal;
    [SerializeField] float vertical;
    [SerializeField] bool isTouchingLadder;

    // Ground Check
    [SerializeField] float groundCheckDistance = 0.5f;
    RaycastHit2D[] groundCheckHit = new RaycastHit2D[5];
    ContactFilter2D groundCheckFilter;
    bool groundCheck;
    public bool isGrounded
    {
        get { return groundCheck; }
        private set
        {
            groundCheck = value;
            if (!isTouchingLadder)
            {
                anim.SetBool("isJumping", !value);
            }
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        capsCollider = GetComponent<CapsuleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        GroundCheck();
        FlipSprite();
        LadderCheck();
    }

    private void FixedUpdate()
    {
        if (!isTouchingLadder)
        {
            rb.velocity = new Vector2(horizontal * moveSpeed, rb.velocity.y);
            rb.gravityScale = baseGravity;
        }
        else
        {
            rb.velocity = new Vector2(horizontal * 0.6f, vertical * climbSpeed);
            rb.gravityScale = 0f;

            if (rb.velocity.magnitude < 0.1f)
            {
                anim.SetBool("isStopped", true);
            }
            else
            {
                anim.SetBool("isStopped", false);
            }
        }
        anim.SetFloat("yVelocity", rb.velocity.normalized.y);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (isGrounded && context.performed)
        {
            anim.SetBool("isJumping", true);
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
        }
        else if (context.canceled && rb.velocity.y != 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<Vector2>().x;
        anim.SetBool("isMoving", Mathf.Abs(horizontal) > Mathf.Epsilon);
    }

    public void OnLadder(InputAction.CallbackContext context)
    {
        vertical = context.ReadValue<Vector2>().y;
    }

    void FlipSprite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(horizontal) > Mathf.Epsilon;
        if (playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(horizontal), 1f);
        }
    }

    void GroundCheck()
    {
        isGrounded = capsCollider.Cast(Vector2.down, groundCheckFilter, groundCheckHit, groundCheckDistance) > 0;
    }

    void LadderCheck()
    {
        if (vertical > 0)
        {
            if (capsCollider.IsTouchingLayers(LayerMask.GetMask("Ladder")))
            {
                isTouchingLadder = true;
                anim.SetBool("isClimbing", true);
            }
            else
            {
                isTouchingLadder = false;
                anim.SetBool("isClimbing", false);
            }
        }
        else if (!capsCollider.IsTouchingLayers(LayerMask.GetMask("Ladder")))
        {
            isTouchingLadder = false;
            anim.SetBool("isClimbing", false);
        }
    }
}
