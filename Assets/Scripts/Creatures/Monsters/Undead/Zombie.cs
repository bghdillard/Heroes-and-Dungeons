using System.Collections;
using System.Collections.Generic;
using BehaviorTree;
using UnityEngine;

public class Zombie : Monster
{
    protected override Node TreeSetup()
    {
        Node root = base.TreeSetup();
        root.AddCombatTree(new List<Node> {
            new Selector()
        });
        return root;
    }
}
