using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public string characterName;
    public int health, maxHealth;
    public int energy, maxEnergy;
    public int defense, shield;

    // List of actions that this character can perform
    // You can add more in the editor since actions are serialized
    public List<Action> actions;

    void Start()
    {
        health = maxHealth;
        energy = maxEnergy;
        shield = 0;

        // Initialize the list of actions
        actions = new List<Action>
        {
            
        };
    }

    // Methods to handle damage and healing
    public void TakeDamage(int amount)
    {
        int damage = amount - shield;
        if (damage <=0 )
        {
            damage = 0;
        }
        health -=  damage;
        shield = 0;
        if (health < 0) health = 0;
    }

    public void Heal(int amount)
    {
        health += amount;
        if (health > maxHealth) health = maxHealth;
    }

    public void Block()
    {
        shield += defense;
    }
}


