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

    void FixedUpdate()
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
}
