using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public string characterName;
    public int health, maxHealth;
    public int energy, maxEnergy;
    public int defense, shield;
    public int recoveryTurns, poisonTurns;
    public bool isPoisoned, isRecovering;

    // List of actions that this character can perform
    // You can add more in the editor since actions are serialized
    public List<Action> actions;

    void Awake()
    {
        health = maxHealth;
        energy = maxEnergy;
        shield = 0;
    }

    // Methods to handle damage and healing
    public void TakeDamage(int amount = 0)
    {
        if (amount == 0) {
            health -= maxHealth/20;
        }
        else 
        {
            int damage = amount - shield;
            if (damage <=0 )
            {
                damage = 0;
            }
            health -=  damage;
            shield = 0;
        }
        if (health < 0) 
        {
            health = 0;
            recoveryTurns = 0;
            poisonTurns = 0;
            isRecovering = false;
            isPoisoned = false;
        }
    }

    public void Heal(int amount = 0)
    {
        if (amount <= 0) {
            health += maxHealth/10;
        }
        else {
            health += amount;
        }
        if (health > maxHealth) health = maxHealth;
    }

    public void Block(int amount = 0) 
    {
        if (amount <= 0) {
            shield += defense;
        }
        else 
        {
            shield += amount;
        }
    }

    public void RestoreEnergy(int amount = 0) 
    {
        if (amount <= 0) {
            energy += maxEnergy/10;
        }
        else {
            energy += amount;
        }
        if (energy > maxEnergy) {
            energy = maxEnergy;
        }
    }

    public void StartRecovering() 
    {
        isRecovering = true;
        recoveryTurns = 3;
    }

    public void Recover()
    {
        Heal();
        RestoreEnergy();
        recoveryTurns -= 1;
        if (recoveryTurns <= 0) 
        {
            recoveryTurns = 0;
            isRecovering = false;
        }
    }

    public void BecomePoisoned()
    {
        isPoisoned = true;
        poisonTurns = 3;
    }

    public void PoisonEffect() 
    {
        TakeDamage();
        poisonTurns -= 1;
        if (poisonTurns <= 0) 
        {
            poisonTurns = 0;
            isPoisoned = false;
        }
    }
}


