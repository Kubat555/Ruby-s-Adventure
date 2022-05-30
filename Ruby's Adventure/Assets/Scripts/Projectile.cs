using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float deathTime = 6f;
    public ParticleSystem hitEffect;
    float timer;
    bool death;
    Rigidbody2D rigidbody2d;
    
    void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        timer = deathTime;
        death = false;
    }

    private void Update() {
        if(death){
            timer -= Time.deltaTime;
        }
        if(timer < 0){
            Destroy(gameObject);
        }
    }

    public void Launch(Vector2 direction, float force)
    {
        rigidbody2d.AddForce(direction * force);
        death = true;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        Instantiate(hitEffect, rigidbody2d.position, Quaternion.identity);
        EnemyController e = other.collider.GetComponent<EnemyController>();
        if (e != null)
        {
            e.Fix();
        }
        Destroy(gameObject);
    }
}
