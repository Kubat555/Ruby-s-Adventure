using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubyController : MonoBehaviour
{
    public AudioClip hitAudio;
    public AudioClip throwAudio;
    public GameObject projectilePrefab;
    public float speed = 3f;
    public int maxHealth = 5;
    public float timeInvincible = 2f;
    public int health { get { return currentHealth; }} // Это свойство хеалз, позволяет получить доступ к currentHealth не нарушая инкапсуляцию
    int currentHealth;

    bool isInvincible;
    float invincibleTimer;
    
    Rigidbody2D rb;
    Animator animator;
    Vector2 lookDirection = new Vector2(1,0);
    float horizontal;
    float vertical;
    AudioSource audioSource;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;

        audioSource= GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        Vector2 move = new Vector2(horizontal, vertical);
        
        if(!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }
                
        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        if(isInvincible){
            invincibleTimer -= Time.deltaTime;
            if(invincibleTimer < 0){
                isInvincible = false;
            }
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            Launch();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            //Raycasting — это действие по созданию луча в сцене и проверке пересечения этого луча с коллайдером . Луч имеет начальную точку, направление и длину. Термин «отбрасывать» луч используется потому, что проверка проводится от начальной точки по всему лучу до его конца.


            //В этой переменной хранится результат Raycast , который нам дает Physics2D.Raycast 
            RaycastHit2D hit = Physics2D.Raycast(rb.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                    character.DisplayDialog();
                }  
            }
        }
    }

    private void FixedUpdate() {
        Vector2 position = rb.position;
        position.x = position.x + speed*horizontal * Time.deltaTime;
        position.y = position.y + speed*vertical * Time.deltaTime;
        
        // движение персонажа
        rb.MovePosition(position);
    }

    public void ChangeHealth(int amount)
    {
        if(amount < 0){
            animator.SetTrigger("Hit");
            if(isInvincible) return;

            isInvincible = true;
            invincibleTimer = timeInvincible;
            PlaySound(throwAudio);
        }


        // Функция Mathf.Clamp() позволяет изменять значение здоровья в пределах максимального и минимального значения
        // Ограничение гарантирует, что первый параметр (здесь currentHealth + сумма ) никогда не будет ниже второго параметра (здесь 0 ) и никогда не превысит третий параметр ( maxHealth ). Таким образом , здоровье Руби всегда будет оставаться между 0 и maxHealth .
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
    }

    void Launch(){
        GameObject projectileObject = Instantiate(projectilePrefab, rb.position + Vector2.up * 0.5f, Quaternion.identity);
        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 700);

        animator.SetTrigger("Launch");
        PlaySound(hitAudio);
    }
}
