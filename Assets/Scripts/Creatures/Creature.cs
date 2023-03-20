using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Creature : BehaviorTree.Tree
{
    #region basicStats
    [Header("basicStats")]
    [SerializeField]
    protected int maxHealth;
    protected int currMaxHealth;
    protected int currHealth;
    [SerializeField]
    protected int maxMagic;
    protected int currMaxMagic;
    protected int currMagic;
    [SerializeField]
    protected int maxStamina;
    protected int currMaxStamina;
    protected int currStamina;
    [SerializeField]
    protected int maxLoyalty;
    protected int currMaxLoyalty;
    protected int currLoyalty;
    #endregion

    #region advancedStats
    [Header("advancedSats")]
    [SerializeField]
    protected int inherentArmor;
    protected int armorValue; //armor reduces the damage of physical attacks;
    [SerializeField]
    protected int inherentResistance;
    protected int resistance; //resistance reduces the damage of magical attacks;
    [SerializeField]
    protected List<string> inherentWeaknesses;
    protected List<string> weaknesses; //hitting with a weakness does double damage;
    [SerializeField]
    protected List<string> inherentResistances;
    protected List<string> resistances; //hitting with a resistance does half damage;
    [SerializeField]
    protected List<string> InherentImmunities;
    protected List<string> immunities; //hitting with an immunity does no damage;
    #endregion

    #region inProgressStats
    //protected List<Effect> activeEffects; //any passive active effects belong here, positive and negative, as well as both permanent (gained through unlocked skills) and temporary (gained through gameplay) 
    //protected List<Skill> activeSkills; //These will be all the various combat skills a hero or monster is capable of using. I'm thinking now that these will be more nodes in the tree
    //protected List<Equipment> equipment; //Maybe actually this'll be a dictionary to keep track of where each piece of equipment belongs: head, chest, left hand, right hand, feet, and so on
    //protected List<Item> inventory; //I will almost definitely eventually want a better system for managing inventory, actually, maybe just an inventory class;
    #endregion

    #region combatTracker
    protected bool inCombat;
    protected List<Creature> friends;
    protected List<Creature> enemies;
    #endregion

    public virtual void HealMaxHealth(int toHeal)
    {
        currMaxHealth += toHeal;
        if (currMaxHealth > maxHealth) currMaxHealth = maxHealth;
        currHealth += toHeal;
        if (currHealth > currMaxHealth) currHealth = maxHealth;
    }

    public virtual void HealHealth(int toHeal)
    {
        currHealth += toHeal;
        if (currHealth > currMaxHealth) currHealth = maxHealth;
    }

    public virtual void HealMaxStamina(int toHeal)
    {
        currMaxStamina += toHeal;
        if (currMaxStamina > maxStamina) currMaxStamina = maxStamina;
        currStamina += toHeal;
        if (currStamina > currMaxStamina) currStamina = maxStamina;
    }

    public virtual void HealStamina(int toHeal)
    {
        currStamina += toHeal;
        if (currStamina > currMaxStamina) currStamina = maxStamina;
    }

    public virtual void HealMaxMagic(int toHeal)
    {
        currMaxMagic += toHeal;
        if (currMaxMagic > maxMagic) currMaxMagic = maxMagic;
        currMagic += toHeal;
        if (currMagic > currMaxMagic) currMagic = maxMagic;
    }

    public virtual void HealMagic(int toHeal)
    {
        currMagic += toHeal;
        if (currMagic > currMaxMagic) currMagic = maxMagic;
    }

    public virtual void DamageHealth(int toDamage)
    {
        currHealth -= toDamage;
    }

    public virtual void DamageStamina(int toDamage)
    {
        currStamina -= toDamage;
    }

    public virtual void DamageMagic(int toDamage)
    {
        currMagic -= toDamage;
    }

    public virtual void DamageLoyalty(int toDamage)
    {
        currLoyalty -= toDamage;
    }

    public virtual int DamageHealth(int toDamage, int AP, List<string> types)
    {
        int finalDamage = toDamage;
        foreach (string type in types) {
            if (weaknesses.Contains(type)) finalDamage *= 2;
            if (resistances.Contains(type)) finalDamage /= 2;
            if  (immunities.Contains(type)){
                finalDamage *= 0;
                break;
            }
        }
        if (types.Contains("Physical") || types.Contains("Enchanted"))
        {
            int finalArmor = armorValue - AP;
            if (finalArmor < 0) finalArmor = 0; 
            finalDamage -= finalArmor;
            if (finalDamage < 0) finalDamage = 0;
        }
        if (types.Contains("Magical"))
        {
            int finalResistance = resistance - AP;
            if (finalResistance < 0) finalResistance = 0;
            finalDamage -= finalResistance;
            if (finalDamage < 0) finalDamage = 0;
        }
        currHealth -= finalDamage;
        return finalDamage;
    }

    /*
    public void AddEffect(Effect toAdd)
    {
        activeEffects.Add(toAdd);
    }

    public void RemoveEffect(Effect toRemove)
    {
        activeEffects.Remove(toRemove);
    }
    */
}