using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed = 2.0f;
    public bool vertical;
    public float changeTime = 3.0f;
    public ParticleSystem smokeEffect;

    Rigidbody2D enemyRB;
    Animator animator;
    float timer;
    int direction = 1;
    bool broken = true;
    void Start()
    {
        enemyRB = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        timer = changeTime;
    }

    // Update is called once per frame
    void Update()
    {
        if(!broken) return;

        timer -= Time.deltaTime;

        if(timer < 0){
            direction = -direction;
            timer = changeTime;
        }
    }

    private void FixedUpdate() {
        if(!broken) return;

        Vector2 position = enemyRB.position;
        if(vertical){
            position.y = position.y + Time.deltaTime * speed * direction;
            animator.SetFloat("MoveX", 0);
            animator.SetFloat("MoveY", direction);
        }
        else
        {
            position.x = position.x + Time.deltaTime * speed * direction;
            animator.SetFloat("MoveX", direction);
            animator.SetFloat("MoveY", 0);
        }
        
        enemyRB.MovePosition(position);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        RubyController player = other.gameObject.GetComponent<RubyController>();

        if (player != null)
        {
            player.ChangeHealth(-1);
        }
    }

    public void Fix()
    {
        smokeEffect.Stop();
        animator.SetTrigger("Fixed");
        broken = false;
        enemyRB.simulated = false;
    }
}
