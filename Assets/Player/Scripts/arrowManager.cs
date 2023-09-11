using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class arrowManager : MonoBehaviour
{
    // Arrow Component
    Rigidbody2D rb;
    Player_Controller player;

    // Arrow Variables
    [SerializeField] float arrowSpeed = 15f;
    [SerializeField] float xSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<Player_Controller>();
        xSpeed = player.transform.localScale.x * arrowSpeed;
    }

    void Update()
    {
        ArrowFlip();
    }

    void ArrowFlip()
    {
        rb.velocity = new Vector2(xSpeed, 0f);
        //transform.localScale = player.transform.localScale;
        if (rb.velocity.x > 0f)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (rb.velocity.x < 0f)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        StartCoroutine(destroyArrow());
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        StartCoroutine(destroyArrow());
    }

    IEnumerator destroyArrow()
    {
        yield return new WaitForSeconds(0.05f);
        Destroy(gameObject);
    }
}
