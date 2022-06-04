using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{

    public bool isFromInvader = false;

    private Vector3 direction = Vector3.up;

    void Start() 
    {
        if (isFromInvader)
            direction = Vector3.down;
    }


    void Update()
    {
        if (Mathf.Abs(this.transform.position.y) > 5.0f) {
            Destroy(gameObject);
        }

        this.transform.position += direction * 8.0f * Time.deltaTime;
    }
    
    void OnTriggerEnter2D(Collider2D col)
    {
        if (isFromInvader && col.tag == "invaders") return;
        Destroy(gameObject);
    }
}
