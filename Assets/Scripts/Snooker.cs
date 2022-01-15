using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class Snooker : MonoBehaviour
{
    public float velocityThreshold = 0.1f;
    public SnookerSpawner snookerSpawnPoint;
    protected Rigidbody2D rb;
    protected float thrust = 2f;
    protected bool velocityLimitState = true;


    protected void FireSnooker(Quaternion rotation, float angle = 0f)
    {
        transform.rotation = rotation;

        rb.AddForce(transform.up * thrust, ForceMode2D.Impulse);
        velocityLimitState = false;

        GlobalProc.AddScore(GlobalProc.GetGlobalVar().scoreText, -1);
    }

    
    void CheckFrictionThreshold()
    {
        if (rb.velocity.magnitude <= velocityThreshold)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = 0f;
            rb.Sleep();
            velocityLimitState = true;
        }
    }
    // FixedUpdate is called once per physics step
    private void FixedUpdate()
    {
        if (!velocityLimitState) CheckFrictionThreshold();
    }
}
