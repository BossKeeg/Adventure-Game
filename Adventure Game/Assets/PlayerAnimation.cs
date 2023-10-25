using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator anim;
    private Rigidbody rb;
    private float maxSpeed = 5f;

    private void Start()
    {
        anim = this.GetComponent<Animator>();
        rb = this.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        anim.SetFloat("Speed", rb.velocity.magnitude / maxSpeed);
    }
}
