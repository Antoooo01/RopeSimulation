using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followMouse : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 temp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //transform.position = new Vector3(temp.x, temp.y, 0);
        rb.velocity = temp - transform.position;
    }
}
