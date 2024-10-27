using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Action
{
    public string actionName;
    public int cost; // e.g., MP or stamina cost
    public int damage;
    public bool heal;
    public bool block;
    public bool aoe;

    // Constructor for initializing action values
    public Action(string name, int cost, int damage = 0, bool heal = false, bool block = false, bool aoe = false)
    {
        actionName = name;
        this.cost = cost;
        this.damage = damage;
        this.heal = heal;
        this.block = block;
        this.aoe = aoe;
    }

    // Method to apply the action on target characters
    public void Execute(Character user, List<Character> targets)
    {
        user.energy -= cost;
        if (block) {
            user.Block();
        }

        foreach (Character target in targets)
        {
            if (damage > 0 && heal == false)
            {
                target.TakeDamage(damage);
            }
            else if (heal)
            {
                target.Heal(damage);
            }
        }
    }
}
