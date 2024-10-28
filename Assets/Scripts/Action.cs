using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Action
{
    public string actionName;
    public int cost, damage;
    public bool heal, block, aoe, poison;

    // Constructor for initializing action values
    public Action(string name, int cost, int damage = 0, bool heal = false, bool block = false, bool aoe = false, bool poison = false)
    {
        actionName = name;
        this.cost = cost;
        this.damage = damage;
        this.heal = heal;
        this.block = block;
        this.aoe = aoe;
        this.poison = poison;
    }

    // Method to apply the action on target characters
    public void Execute(Character user, List<Character> targets)
    {
        user.energy -= cost;
        if (!aoe && block) {
            user.Block();
        }

        foreach (Character target in targets)
        {
            if (damage > 0 && !heal)
            {
                target.TakeDamage(damage);
                if (poison) {
                    target.BecomePoisoned();
                }
            }
            else if (heal)
            {
                target.Heal(damage);
                target.StartRecovering();
            }
            if (aoe && block) {
                target.Block(user.defense);
            }
        }
    }
}
