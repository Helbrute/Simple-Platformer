using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Player_Controller : MonoBehaviour
{
    #region Player Components
    private Rigidbody2D rb;
    private Animator anim;
    private CapsuleCollider2D capsCollider;
    public GameObject arrow;
    public Transform arrowPosition;
    
    #endregion

    #region Player Variables
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float jumpPower = 10f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] private float baseGravity = 5f;
    [SerializeField] private float horizontal;
    [SerializeField] private float vertical;
    [SerializeField] float coyoteTime = 0.2f;
    [SerializeField] private float coyoteTimer;
    [SerializeField] float arrowCooldown = 1f;
    private float lastArrowShotTime;
    #endregion

    #region Other Checks
    [SerializeField] bool isTouchingLadder;
    [SerializeField] bool isAlive = true;
    [SerializeField] bool isShooting = false;
    #endregion

    #region Ground Check
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
    #endregion

    void Start()
    {
        isAlive = true;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        capsCollider = GetComponent<CapsuleCollider2D>();
    }
    void Update()
    {
        FlipSprite();

        // Cheking methods
        GroundCheck();
        LadderCheck();

        // Dying behaviour
        if (!isAlive) { rb.velocity = new Vector2(0f, rb.velocity.y); return; }

        #region Basic movement physics and conditions
        if (!isTouchingLadder && !isShooting)
        {
            rb.velocity = new Vector2(horizontal * moveSpeed, rb.velocity.y); // Standard ladder Vector2
            rb.gravityScale = baseGravity; // Adjust Gravity while not climbing ladder
        }
        else
        {
            rb.velocity = new Vector2(horizontal * 0.6f, vertical * climbSpeed); // Climbing ladder Vector2
            rb.gravityScale = 0f; // Adjust gravity while climbing ladder

            // Check if the player is shooting to change Vector2 
            if (isShooting)
            {
                rb.velocity = Vector2.zero;
            }

            // Ladder animation
            if (rb.velocity.magnitude < 0.1f)
            {
                anim.SetBool("isStopped", true);
            }
            else
            {
                anim.SetBool("isStopped", false);
            }
        }
        #endregion

        // Animations
        anim.SetBool("isShooting", isShooting);
        anim.SetFloat("yVelocity", rb.velocity.normalized.y);

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

    #region Player Controlls (Input System)
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
        if (context.performed && isAlive && !isTouchingLadder && isGrounded)
        {
            if (Time.time - lastArrowShotTime >= arrowCooldown)
            {
                StartCoroutine(Shooting());
                lastArrowShotTime = Time.time;
            }
        }
        else if (!isAlive) { return; }
    }
    #endregion

    #region Other Methods
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
    #endregion

    // Collision behaviour
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Hazards"))
        {
            isAlive = false;
            StartCoroutine(DeathRoutine());
            Debug.Log("Enemy detected!");
        }
    }

    #region Player Coroutines
    IEnumerator DeathRoutine()
    {
        anim.SetTrigger("isDead");
        yield return new WaitForSeconds(1.5f);
        FindObjectOfType<GameSession>().ProcessPlayerDeath();
        Destroy(gameObject);
    }
    IEnumerator Shooting()
    {
        isShooting = true;
        yield return new WaitForSeconds(0.4f);
        Instantiate(arrow, arrowPosition.position, transform.rotation);
        isShooting = false;
        yield return new WaitForSeconds(1f);
    }
    #endregion
}
