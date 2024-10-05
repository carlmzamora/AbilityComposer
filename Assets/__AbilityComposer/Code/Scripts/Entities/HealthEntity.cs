using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HealthEntity : ClickableEntity
{
    [SerializeField] public float currentHealth { get; private set; }
    public float maxHealth { get; private set; }

    public void SetCurrentHealth(float setValue)
    {
        currentHealth = setValue;
    }

    public void AddCurrentHealth(float value)
    {
        currentHealth += value;
    }

    public void SetMaxHealth(float setValue)
    {
        maxHealth = setValue;
    }   

    public void AddMaxHealth(float value, bool proportionCurrentHealth = false)
    {
        maxHealth += value;

        if (proportionCurrentHealth)
            currentHealth += value;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
    }
}
