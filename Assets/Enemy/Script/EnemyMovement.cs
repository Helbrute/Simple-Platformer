using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    //Enemy Components
    Rigidbody2D rb;
    Animator anim;
    BoxCollider2D boxCol;
    CapsuleCollider2D capsCol;

    // Enemy Variables
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] bool isAlive;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCol = GetComponent<BoxCollider2D>();
        capsCol = GetComponent<CapsuleCollider2D>();
        isAlive = true;
    }
    void Update()
    {
        Movement();
        if (!isAlive)
        {
            capsCol.enabled = false;
            boxCol.enabled = false;
            rb.velocity = Vector2.zero;
            return;
        }
    }

    // Phycics Functions
    void Movement()
    {
        if (!isAlive) { return; }
        rb.velocity = new Vector2(moveSpeed, 0f);

        // Animation Handler
        bool hasHorizontalSpeed = Mathf.Abs(rb.velocity.x) > Mathf.Epsilon;
        anim.SetBool("isMoving", hasHorizontalSpeed);
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Bounds")
        {
            moveSpeed = -moveSpeed;
            FlipSprite();
        }
        if (other.tag == "Arrow")
        {
            isAlive = false;
            rb.velocity = new Vector2(0f, rb.velocity.y);
            StartCoroutine(DeathRoutine());
        }
    }
    void FlipSprite()
    {
        transform.localScale = new Vector2(-(Mathf.Sign(rb.velocity.x)), 1f);
    }
    IEnumerator DeathRoutine()
    {
        anim.SetTrigger("isDead");
        yield return new WaitForSeconds(1.117f);
        Destroy(gameObject);
    }
}
