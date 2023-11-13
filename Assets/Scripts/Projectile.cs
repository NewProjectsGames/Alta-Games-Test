using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private Rigidbody rigidbody;

    [SerializeField] private float force = 500;

    // Start is called before the first frame update
    private float _radiusDamage;


    public void Fire(float power)
    {
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        transform.eulerAngles = Vector3.zero;
        rigidbody.AddForce((transform.forward + transform.up) * force * rigidbody.mass);
        _radiusDamage = power * 3;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("Obstacle"))
        {
            Explosion();
        }

        if (other.transform.CompareTag("Door"))
        {
            BallController.OnDestroyObstacle.Invoke();
            gameObject.SetActive(false);
        }
    }

    void Explosion()
    {
        var obstacles = Physics.OverlapSphere(transform.position, _radiusDamage);
        foreach (var item in obstacles)
        {
            if (item.TryGetComponent(out Obstacle obs))
            {
                obs.StartDestruction(Vector3.Distance(item.transform.position, transform.position));
            }
        }

        gameObject.SetActive(false);
    }
}