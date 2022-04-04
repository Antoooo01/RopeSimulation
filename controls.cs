using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controls : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float hori = Input.GetAxis("Horizontal");
        float verti = Input.GetAxis("Vertical");

        if (hori == 0 && verti == 0)
        {
            rb.velocity.Set(0, 0);
        }
        else
        {
            rb.velocity = new Vector2(rb.velocity.x + hori, rb.velocity.y + verti);

            if (rb.velocity.magnitude > 5)
            {
                rb.velocity = rb.velocity.normalized * 5;
            }
        }
    }
}
