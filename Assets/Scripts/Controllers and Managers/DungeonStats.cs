using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DungeonStats
{
    private static Dictionary<string, int> dungeonStats = new Dictionary<string, int> //these will all eventually all need to be sent to here from elsewhere so as not to mess with saves, as well as to make editing the patrol options easier
    {
        {"Prestige", 0}, // Prestige will affect progression, higher prestige comes from higher quality cell types and minions, and, in turn, will attract higher quality heroes
        {"Tier 1 Cap", 0}, // Recruitment based rooms will increase the amount of minions of certain tiers that can be hired. higher tiered rooms will be more expensive to make
        {"Gold", 0}, // Treasury Items will increase the amount of gold that can be stored. Gold will be used to build rooms of higher quality, and to hire and pay minions of higher qualities. Perhaps some item crafting as well
        {"Ore", 0}, // Some Items will increase the amount of special ores that can be held for the use of crafting
        {"Weapons", 0 }, // Special weapons will need to be stored in specific armory items
        {"PatrolPointLimit", 5}, // Patrols will be limited to set number of points that can be increased via research options
        {"PatrolPointDistance", 20} // Patrol points will need to be within a certain distance of the last patrol point of their patrol, that distance can be increased through research
    };

    private static List<Monster> monsters = new List<Monster>();

    public static void UpdateStat(string toUpdate, int updateAmount)
    {
        dungeonStats[toUpdate] += updateAmount;
    }

    public static int GetStat(string toGet)
    {
        return dungeonStats[toGet];
    }

    public static List<Monster> GetMonsters()
    {
        return monsters;
    }

    public static void AddMonster(Monster toAdd)
    {
        monsters.Add(toAdd);
    }

    public static void RemoveMonster(Monster toRemove)
    {
        monsters.Remove(toRemove);
    }
}
