using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Controller : MonoBehaviour
{
    #region Player Components
    Rigidbody2D rb;
    CapsuleCollider2D capsCollider;
    Vector2 moveInput;
    #endregion

    #region Player Variables
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float jumpPower = 10f;
    [SerializeField] float baseGravity = 5f;
    #endregion

    #region Ground Check
    [SerializeField] float groundCheckDistance = 0.5f;
    //[SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundLayer;
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
        }
    }
    #endregion

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        capsCollider = GetComponent<CapsuleCollider2D>();
    }

    void Update()
    {
        GroundCheck();
    }

    void GroundCheck()
    {
        groundCheck = Physics2D.OverlapCircle(transform.position, groundCheckDistance, groundLayer);
    }
}
