using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxEventDispatcher : MonoBehaviour
{
    public event Action playerHitEvent;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Bullet")
            return;

        Debug.Log("OnCollision");
        playerHitEvent?.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Bullet")
            return;

        Debug.Log("OnTrigger");
        playerHitEvent?.Invoke();
    }
}
