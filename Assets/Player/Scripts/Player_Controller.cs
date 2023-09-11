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
    public GameObject arrow;
    public Transform arrowPosition;

    // Player Variables
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float jumpPower = 10f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] private float baseGravity = 5f;
    [SerializeField] private float horizontal;
    [SerializeField] private float vertical;
    [SerializeField] float coyoteTime = 0.2f;
    [SerializeField] private float coyoteTimer;

    // Checks
    [SerializeField] bool isTouchingLadder;
    [SerializeField] bool isAlive = true;
    //[SerializeField] bool isShooting = false;

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
        isAlive = true;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        capsCollider = GetComponent<CapsuleCollider2D>();
    }

    void Update()
    {
        anim.SetFloat("yVelocity", rb.velocity.normalized.y);
        GroundCheck();
        FlipSprite();
        LadderCheck();
        if (!isAlive) { rb.velocity = new Vector2(0f, rb.velocity.y); return; }
        if (!isTouchingLadder)
        {
            rb.velocity = new Vector2(horizontal * moveSpeed, rb.velocity.y);
            rb.gravityScale = baseGravity;
        }else
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

        // Coyote Jump
        if (isGrounded)
        {
            coyoteTimer = coyoteTime;
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
        }
    }

    //Player Controlls
    public void OnJump(InputAction.CallbackContext context)
    {
        if (!isAlive) { return; }
        if (context.performed && (isGrounded || coyoteTimer > 0f))
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
        if (!isAlive) { return; }
        horizontal = context.ReadValue<Vector2>().x;
        anim.SetBool("isMoving", Mathf.Abs(horizontal) > Mathf.Epsilon);
    }
    public void OnLadder(InputAction.CallbackContext context)
    {
        if (!isAlive) { return; }
        vertical = context.ReadValue<Vector2>().y;
    }
    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed && isAlive)
        {
            Instantiate(arrow, arrowPosition.position, transform.rotation);
        }
        else if (!isAlive) { return; }
    }

    //Phycics Methods
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Hazards"))
        {
            isAlive = false;
            StartCoroutine(deathRoutine());
            Debug.Log("Enemy detected!");
        }
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

    // Player Coroutines
    IEnumerator deathRoutine()
    {
        anim.SetTrigger("isDead");
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
    }
    //IEnumerator shooting()
    //{
    //    anim.SetBool("isShooting", true);
    //    yield return new WaitForSeconds(0.4f);
    //    anim.SetBool("isShooting", false);
    //}
}
