using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    private Rigidbody _bulletRigidbody;

    [SerializeField]
    private float _speed = 10.0f;

    void Awake()
    {
        _bulletRigidbody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        _bulletRigidbody.velocity = transform.forward * _speed;
    }

    void Update()
    {
        
    }

    void OnTrigger(Collider other)
    {
        Destroy(gameObject);
    }
}
