using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class CueBall : MonoBehaviour
{
    public int awardedScore = 0;
    private Rigidbody2D rb;
    [SerializeField]
    private float velocityThreshold = 0.1f;
    private float lastVelocityMagnitude = 0f;
    private bool velocityLimitState = false;

    void CheckFrictionThreshold()
    {
        if (rb.velocity.magnitude <= velocityThreshold)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = 0f;
            rb.Sleep();
            velocityLimitState = false;
        }
        else lastVelocityMagnitude = rb.velocity.magnitude;
    }

    // Use this for initialization  
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // FixedUpdate is called once per physics step
    private void FixedUpdate()
    {
        if (velocityLimitState) CheckFrictionThreshold();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        velocityLimitState = true;
    }
}
