using UnityEngine;

public class Character : MonoBehaviour
{
    public string Name;

    public int Health;
    public int MaxHealth;

    public int BaseAttackPower;
    public int BaseDefense;

    public float damageMultiplier = 1f;
    public float defenseMultiplier = 1f;

    public bool IsDefending;
    public HealthBarFollow healthBar;

    private Animator animator;

    public int AttackPower => Mathf.CeilToInt(BaseAttackPower * damageMultiplier);
    public int Defense => Mathf.CeilToInt(BaseDefense * defenseMultiplier);

    void Start()
    {
        animator = GetComponent<Animator>();

        if (healthBar != null)
            healthBar.UpdateHealthBar();
    }

    public bool IsAlive => Health > 0;

    public void TakeDamage(int amount)
    {
        Health -= amount;
        if (Health < 0) Health = 0;


        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }

        if (healthBar != null)
            healthBar.UpdateHealthBar();
    }

    public void TriggerAttackAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
    }

    public void StartDefending()
    {
        IsDefending = true;
        defenseMultiplier = 1f;
        if (animator != null)
        {
            animator.SetBool("Block", true); 
        }
    }

    public void StopDefending()
    {
        IsDefending = false;
        defenseMultiplier = 1f;
        if (animator != null)
        {
            animator.SetBool("Block", false);
        }
    }
}
