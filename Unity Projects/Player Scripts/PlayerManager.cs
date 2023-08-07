using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public enum PlayerClass
    {
        Knight,
        Wizard,
        Ranger,
        Rogue
    }

    public enum PlayerState
    {
        Alive,
        Dead
    }

    [Header("Player Stats")]
    public PlayerClass playerClass;
    public float maxhealth;
    public float attack;
    public float defense;

    [Header("Player Components")]
    [SerializeField] private HealthBar hpBar;

    private float currentHealth;
    private PlayerState state;

    public void SetPlayerClass(PlayerClass pClass)
    {
        playerClass = pClass;
    }

    public void SetMaxHealth(float hp)
    {
        maxhealth = hp;
        currentHealth = maxhealth;
        hpBar.SetMaxValue(maxhealth);
        hpBar.UpdateHealth(currentHealth);
    }

    public float GetMaxHealth()
    {
        return maxhealth;
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public void SetAttack(float atk)
    {
        attack = atk;
    }

    public void SetDefense(float def)
    {
        defense = def;
    }

    public PlayerState GetState()
    {
        return state;
    }

    private void Start()
    {
        currentHealth = maxhealth;
        state = PlayerState.Alive;
        hpBar.SetMaxValue(maxhealth);
    }

    private void Update()
    {
        if (currentHealth < 0)
        {
            state = PlayerState.Dead;
        }
        else
        {
            state = PlayerState.Alive;
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            TakeDamage(10);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            currentHealth = maxhealth;
            hpBar.UpdateHealth(currentHealth);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= (int)(damage - (damage * (defense / 100)));
        currentHealth = Mathf.Clamp(currentHealth, 0, maxhealth);
        hpBar.UpdateHealth(currentHealth);
    }
}
