using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class HingeJointRb : MonoBehaviour
{
    public Rigidbody rb;
    public HingeJoint hinge;

    public float torquePower = 0.25f;        // МАЛО
    public float maxAngularVelocity = 0.6f;  // МЕДЛЕННО

    private void Start()
    {
        rb.isKinematic = true;
    }
    
    private void FixedUpdate()
    {
        float direction = Mathf.Sign(Mathf.Sin(Time.time * 0.4f));

        rb.AddTorque(
            hinge.axis * direction * torquePower,
            ForceMode.Acceleration
        );

        rb.angularVelocity = Vector3.ClampMagnitude(
            rb.angularVelocity,
            maxAngularVelocity
        );
    }
    
    public void ActivateHinge()
    {
        rb.isKinematic = false;
        StartCoroutine(ChangeLimits());
    }

    private IEnumerator ChangeLimits()
    {
        yield return new WaitForSeconds(0.4f);
        JointLimits limits = hinge.limits;
        limits.max = -50f;
        hinge.limits = limits;
        hinge.useLimits = true;
        yield return null;
    }
}
