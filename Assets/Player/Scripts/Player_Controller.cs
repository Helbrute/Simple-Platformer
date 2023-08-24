using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Controller : MonoBehaviour
{
    #region Player Components
    Rigidbody2D rb;
    Animator anim;
    CapsuleCollider2D capsCollider;
    Sprite ladderStopSprite;
    #endregion

    #region Player Variables
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float jumpPower = 10f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] float baseGravity = 5f;
    [SerializeField] float horizontal;
    [SerializeField] float vertical;
    [SerializeField] bool isTouchingLadder;
    #endregion

    #region Ground Check
    [SerializeField] float groundCheckDistance = 0.5f;
    RaycastHit2D[] groundCheckHit = new RaycastHit2D[5];
    ContactFilter2D groundCheckFilter;
    [SerializeField] bool groundCheck;
    public bool isGrounded
    {
        get
        {
            return groundCheck;
        }
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
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        capsCollider = GetComponent<CapsuleCollider2D>();
    }

    #region Updates
    void Update()
    {
        GroundCheck();
        FlipSrpite();
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
            rb.velocity = new Vector2(horizontal * 0.5f, vertical * climbSpeed);
            rb.gravityScale = 0f;
        }
        anim.SetFloat("yVelocity", rb.velocity.normalized.y);
    }
    #endregion

    #region Input Actions
    public void OnJump(InputAction.CallbackContext context)
    {
        
        if (isGrounded && context.performed)
        {
            anim.SetBool("isJumping", true);
            //isGrounded = false;
            rb.velocity = new Vector2(rb.velocity.y, jumpPower);
        }
        else if (context.canceled && rb.velocity.y != 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }
    }
    public void OnMove(InputAction.CallbackContext context)
    { 
        horizontal = context.ReadValue<Vector2>().x;
        //Running Animation
        if (horizontal != 0)
        {
            anim.SetBool("isMoving", true);
        }
        else
        {
            anim.SetBool("isMoving", false);
        }
    }
    public void OnLadder(InputAction.CallbackContext context)
    {
        vertical = context.ReadValue<Vector2>().y;
    }
    #endregion

    void FlipSrpite()
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
        if (capsCollider.IsTouchingLayers(LayerMask.GetMask("Ladder")))
        {
            isTouchingLadder = true;
        }
        else
        {
            isTouchingLadder = false;
        }
    }
}
