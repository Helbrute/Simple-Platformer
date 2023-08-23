using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Controller : MonoBehaviour
{
    #region Player Components
    Rigidbody2D rb;
    Animator anim;
    CapsuleCollider2D capsCollider;
    #endregion

    #region Player Variables
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float jumpPower = 10f;
    [SerializeField] float baseGravity = 5f;
    [SerializeField] float horizontal;
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
            anim.SetBool("isJumping", !value);
        }
    }
    #endregion

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        capsCollider = GetComponent<CapsuleCollider2D>();
    }


    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        //make player move
        rb.velocity = new Vector2(horizontal * moveSpeed, rb.velocity.y);
        

        //Methods
        GroundCheck();
        FlipSrpite();
        //anim.SetBool("isJumping", !isGrounded);
        anim.SetFloat("yVelocity", rb.velocity.y);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        
        if (isGrounded && context.performed)
        {
            //anim.SetBool("isJumping", true);
            rb.velocity = new Vector2(rb.velocity.y, jumpPower);
        }
        else if (context.canceled && rb.velocity.y != 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<Vector2>().normalized.x;

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

    void FlipSrpite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(rb.velocity.x) > Mathf.Epsilon;
        if (playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(rb.velocity.x), 1f);
        }
    }

    void GroundCheck()
    {
        isGrounded = capsCollider.Cast(Vector2.down, groundCheckFilter, groundCheckHit, groundCheckDistance) > 0;
    }
}
