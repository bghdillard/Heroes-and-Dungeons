using System.Collections;
using System.Collections.Generic;
using BehaviorTree;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public abstract class Monster : Creature
{
    #region statTimers
    [Header("statTimers")]
    [SerializeField]
    private float statDecayMax;
    [SerializeField]
    private float statDecayMin;
    private float currStatDecay;
    [SerializeField]
    private float healthRegainMax;
    [SerializeField]
    private float healthRegainMin;
    private float currHealthRegain;
    [SerializeField]
    private float staminaRegainMax;
    [SerializeField]
    private float staminaRegainMin;
    private float currStaminaRegain;
    [SerializeField]
    private float magicRegainMax;
    [SerializeField]
    private float magicRegainMin;
    private float currMagicRegain;
    #endregion

    #region UIElements
    UIManager UI;
    TMPro.TextMeshProUGUI nameText;
    GameObject monsterPanel;
    Slider currHealthBar;
    Slider maxHealthBar;
    Slider currStaminaBar;
    Slider maxStaminaBar;
    Slider currMagicBar;
    Slider maxMagicBar;
    Slider currLoyaltyBar;
    Slider maxLoyaltyBar;

    #endregion

    [Header("Misc")]
    [SerializeField]
    private string monsterName;
    [SerializeField]
    private float idleTimer;
    [SerializeField]
    private GameObject selectionCircle;

    private bool isSelected;

    private string order;
    private bool targetChanged;

    //private NavMeshAgent agent;
    private Vector3 target;

    private Restorative item; //again, I'm thinking I might want to make an item base class

    private Room guardRoom; //guardRoom and patrol are mutually exclusive: a monster with a guardRoom cannot have a patrol and vice versa
    private Patrol patrol;
    private PatrolGroup group;
    private bool groupAddition;

    protected void Awake()
    {
        Debug.Log("Monster Start");
        //set the initial stats
        currMaxHealth = maxHealth;
        currMaxMagic = maxMagic;
        currMaxStamina = maxStamina;
        currMaxLoyalty = maxLoyalty;
        currHealth = maxHealth;
        currMagic = maxMagic;
        currStamina = maxStamina;
        currLoyalty = maxLoyalty / 2;
        armorValue = inherentArmor;
        resistance = inherentResistance;

        //set the initial stat timers
        currStatDecay = Random.Range(statDecayMin, statDecayMax);
        currHealthRegain = Random.Range(healthRegainMin, healthRegainMax);
        currStaminaRegain = Random.Range(staminaRegainMin, staminaRegainMax);
        currMagicRegain = Random.Range(magicRegainMin, magicRegainMax);

        //set the resistances, immunities, and weaknesses
        resistances = new List<string>(inherentResistances);
        immunities = new List<string>(InherentImmunities);
        weaknesses = new List<string>(inherentWeaknesses);

        //find the UI elements
        UI = GameObject.Find("GameController").GetComponent<UIManager>();
        monsterPanel = UI.monsterPanel;
        nameText = UI.nameText;
        currHealthBar = UI.currHealthBar;
        maxHealthBar = UI.maxHealthBar;
        currStaminaBar = UI.currStaminaBar;
        maxStaminaBar = UI.maxStaminaBar;
        currMagicBar = UI.currMagicBar;
        maxMagicBar = UI.maxMagicBar;
        currLoyaltyBar = UI.currLoyaltyBar;
        maxLoyaltyBar = UI.maxLoyaltyBar;
        groupAddition = false;
        targetChanged = false;
        agent = GetComponent<NavMeshAgent>();
    }
    
    // Update is called once per frame
    protected override void Update()
    {
        Debug.Log("Monster Update");
        if ((currStatDecay -= Time.deltaTime) <= 0)
        {
            currStatDecay = Random.Range(statDecayMin, statDecayMax);
            switch (Random.Range(0, 2)){
                case 0:
                    Debug.Log("Magic Decay");
                    currMaxMagic -= Random.Range(1, (int)(maxMagic * 0.1));
                    if (currMaxMagic < 0)
                    {
                        currMaxMagic = 0;
                        currLoyalty -= Random.Range(1, (int)(maxLoyalty * 0.1));
                    }
                    if (currMagic > currMaxMagic) currMagic = currMaxMagic;
                    if (isSelected)
                    {
                        maxMagicBar.maxValue = maxMagic;
                        maxMagicBar.value = currMaxMagic;
                        currMagicBar.value = currMagic;
                    }
                    Debug.Log("Magic cap is now " + currMaxMagic);
                    break;
                case 1:
                    Debug.Log("Stamina Decay");
                    currMaxStamina -= Random.Range(1, (int)(maxStamina * 0.1));
                    if (currMaxStamina <= 0)
                    {
                        currMaxStamina = 0;
                        currLoyalty -= Random.Range(1, (int)(maxLoyalty * 0.1));
                    }
                    if (currStamina > currMaxStamina) currStamina = currMaxStamina;
                    if (isSelected)
                    {
                        maxStaminaBar.maxValue = maxStamina;
                        maxStaminaBar.value = currMaxStamina;
                        currStaminaBar.value = currStamina;
                    }
                    Debug.Log("Stamina cap is now " + currMaxStamina);
                    break;
                default:
                    Debug.LogError("stat decay on monster landed on a nonexistent stat");
                    break;
            }
        }
        if ((currHealthRegain -= Time.deltaTime) <= 0)
        {
            currHealthRegain = Random.Range(healthRegainMin, healthRegainMax);
            if (currHealth < maxHealth)
            {
                currHealth += (int)Random.Range(1, currMaxHealth * 0.10f);
                if (currHealth > currMaxHealth) currHealth = currMaxHealth;
                if (isSelected) currHealthBar.value = currHealth; ;
            }
        }
        if ((currStaminaRegain -= Time.deltaTime) <= 0)
        {
            currStaminaRegain = Random.Range(staminaRegainMin, staminaRegainMax);
            if(currStamina < maxStamina)
            {
                currStamina += (int)Random.Range(1, currMaxStamina * 0.10f);
                if (currStamina > currMaxStamina) currStamina = currMaxStamina;
                if (isSelected) currStaminaBar.value = currStamina;
            }
        }
        if ((currMagicRegain -= Time.deltaTime) <= 0)
        {
            currMagicRegain = Random.Range(magicRegainMin, magicRegainMax);
            if (currMagic < maxMagic)
            {
                currMagic += (int)Random.Range(1, currMaxMagic * 0.10f);
                if (currMagic > currMaxMagic) currMagic = currMaxMagic;
                if (isSelected) currMagicBar.value = currMagic;
            }
        }
        base.Update();
        /*
        //check for inCombat
        if (!inCombat) {
            if (!(order == "RestoreHealth" || order == "RestoreStamina" || order == "RestoreMagic"))
            {
                //if we are not already restoring our stats, see if we need to start restoring them;
                List<Restorative> restoratives = new List<Restorative>();
                if (currMaxHealth <= maxHealth * 0.4) //for each stat, if it is below a certain benchmark check to see if there is an available item to restore that stat
                {
                    restoratives = GridManager.GetRestoratives("Health");
                }
                if(restoratives.Count == 0) if (currMaxStamina <= maxStamina * 0.4)
                {
                    restoratives = GridManager.GetRestoratives("Stamina");
                }
                if(restoratives.Count == 0) if (currMaxMagic <= maxMagic * 0.4)
                {
                    restoratives = GridManager.GetRestoratives("Magic");
                }
                if (restoratives.Count != 0) //if we found a stat in need of restoring and found at least one item of that type
                {
                    Restorative toUse = null;
                    float closestDistance = float.MaxValue;
                    NavMeshPath path = new NavMeshPath();
                    foreach (Restorative restorative in restoratives) //find the closest item of the correct stat type that is not already in use
                    {
                        if (!restorative.GetInUse())
                        {
                            if (NavMesh.CalculatePath(transform.position, restorative.GetLocation(), -1, path))
                            {
                                float distance = Vector3.Distance(transform.position, path.corners[0]);
                                for (int y = 1; y < path.corners.Length; y++)
                                {
                                    distance += Vector3.Distance(path.corners[y - 1], path.corners[y]);
                                }
                                if (distance < closestDistance)
                                {
                                    toUse = restorative;
                                    closestDistance = distance;
                                }
                            }
                            else Debug.LogError("Attempted to calculate path from monster at " + transform.position +
                                " to restorative at " + restorative.GetLocation() + " but path calculation failed");
                        }
                    }
                    if (toUse != null) //if an item of the correct type was found, mark it as the target and set the order type to restore;
                    {
                        item = toUse;
                        target = toUse.GetLocation();
                        order = "Restore" + item.GetType();
                        targetChanged = true;
                        item.SetUser(this);
                    }
                }
            }
            if(patrol != null && !(order == "RestoreHealth" || order == "RestoreStamina" || order == "RestoreMagic" || order == "Patrol"))
            {
                Debug.Log("Monster at " + transform.position + " is now patroling");
                //if we are part of a patrol, and we are not currently occupied by restoring our stats join the patrol;
                if (patrol.GetGroup() != null) //if our patrol already has a group, join it;
                {
                    group = patrol.GetGroup();
                }
                else //if not, create a new group;
                {
                    group = patrol.CreateGroup(this);
                }
                target = group.GetPoint().GetPoint(); //Now that we have our group, set the target for the current location of the group
                target.x += Random.Range(-0.5f, 0.5f);
                target.z += Random.Range(-0.5f, 0.5f);
                order = "Patrol";
                targetChanged = true;
            }
            if(guardRoom != null && !(order == "RestoreHealth" || order == "RestoreStamina" || order == "RestoreMagic" || order == "Guard"))
            {
                //if we have a guardroom assigned, and are not currently occupied by restoring our stats, take position in our guardroom
                Debug.Log("Monster at " + transform.position + " is now guarding");
                target = guardRoom.GetRandomPoint();
                order = "Guard";
                targetChanged = true;
            }
            if(order == null)
            {
                if((idleTimer -= Time.deltaTime) <= 0){ //only subtract from the idle timer if there's no current order
                    Debug.Log("Monster at " + transform.position + " is now idling");
                    idleTimer = Random.Range(idleTimerMin, idleTimerMax);
                    order = "Idle";
                    targetChanged = true;
                    List<Room> rooms = GridManager.GetRooms();
                    if (rooms.Count == 1) target = rooms[0].GetRandomPoint();
                    else target = rooms[Random.Range(1, rooms.Count)].GetRandomPoint();
                }
            }
            if (targetChanged) //I might also want to have some sort of universal boolean for if the navMesh was changed
            {
                //if our order was changed, pathfind to the new location
                agent.SetDestination(target);
                targetChanged = false;
            }
            switch (order)
            {
                case "RestoreHealth":
                    if(currMaxHealth == maxHealth)
                    {
                        item.RemoveUser();
                        order = null;
                    }
                    break;
                case "RestoreStamina":
                    if(currMaxStamina == maxStamina)
                    {
                        item.RemoveUser();
                        order = null;
                    }
                    break;
                case "RestoreMagic":
                    if(currMaxMagic == maxMagic)
                    {
                        item.RemoveUser();
                        order = null;
                    }
                    break;
                case "Guard":
                    if(Vector3.Distance(transform.position, target) <= 0.5f)
                    {
                        if((idleTimer -= Time.deltaTime) <= 0)
                        {
                            idleTimer = Random.Range(idleTimerMin, idleTimerMax);
                            target = guardRoom.GetRandomPoint();
                            targetChanged = true;
                        }
                    }
                    break;
                case "Patrol":
                    if(Vector3.Distance(transform.position, target) <= 0.5f && !groupAddition)
                    {
                        if((idleTimer -= Time.deltaTime) <= 0)
                        {
                            idleTimer = Random.Range(idleTimerMin, idleTimerMax);
                            group.AddArrival();
                        }
                    }
                    break;
                case "Idle":
                    if (Vector3.Distance(transform.position, target) <= 0.5f)
                    {
                        if ((idleTimer -= Time.deltaTime) <= 0)
                        {
                            idleTimer = Random.Range(idleTimerMin, idleTimerMax);
                            targetChanged = true;
                            List<Room> rooms = GridManager.GetRooms();
                            if (rooms.Count == 1) target = rooms[0].GetRandomPoint();
                            else target = rooms[Random.Range(1, rooms.Count)].GetRandomPoint();
                        }
                    }
                    break;
                default:
                    Debug.LogError("Monster order switch with no viable order. Order is currently " + order);
                    break;
            }
        }
        */
    }

    protected override Node TreeSetup()
    {
        return new Selector(new List<Node> //root selector for the tree
        {
            new Selector(new List<Node> //selector for the stat restoring sub-tree
            {
                new Sequence(new List<Node> //sequence for the health restoring sub-tree
                {
                    new HealthFindNode(this),
                    new RestorePathfindNode(agent),
                    new HealthRestoreNode(this)
                }),
                new Sequence(new List<Node> //sequence for the stamina restoring sub-tree
                {
                    new StaminaFindNode(this),
                    new RestorePathfindNode(agent),
                    new StaminaRestoreNode(this)
                }),
                new Sequence(new List<Node>{ //sequence for the magic restoring sub-tree
                    new MagicFindNode(this),
                    new RestorePathfindNode(agent),
                    new MagicRestoreNode(this)
                })
            }), //end of stat restoring sub-tree
            new Sequence(new List<Node> //sequence for the patrolling sub-tree
            {
                new GroupNode(this),
                new PatrolNode(agent, idleTimer, this)
            }), //end of patrolling sub-tree
            new GuardNode(agent, idleTimer), //GuardNode leaf
            new IdleNode(agent, idleTimer) //IdleNode leaf
        });
    }

    public bool GetGuardRoom(out Room room)
    {
        room = guardRoom;
        return guardRoom != null;
    }

    public bool GetPatrol(out Patrol patrol)
    {
        patrol = this.patrol;
        return patrol != null;
    }

    public void AssignDefense(Room toAssign)
    {
        guardRoom = toAssign;
        patrol = null;
        root.RemoveData("guardRoom");
        root.RemoveData("patrol");
        root.SetData("guardRoom", toAssign);
    }

    public void AssignDefense(Patrol toAssign)
    {
        Debug.Log("Assign patrol called");
        patrol = toAssign;
        guardRoom = null;
        root.RemoveData("guardRoom");
        root.RemoveData("patrol");
        root.SetData("patrol", toAssign);
    }

    public void UnassignDefense()
    {
        guardRoom = null;
        patrol = null;
        if (group != null) group.RemoveMonster(this);
        group = null;
        root.RemoveData("guardRoom");
        root.RemoveData("patrol");
    }

    public void Select()
    {
        nameText.text = monsterName;
        maxHealthBar.maxValue = maxHealth;
        maxHealthBar.value = currMaxHealth;
        currHealthBar.maxValue = maxHealth;
        currHealthBar.value = currHealth;

        maxStaminaBar.maxValue = maxStamina;
        maxStaminaBar.value = currMaxStamina;
        currStaminaBar.maxValue = maxStamina;
        currStaminaBar.value = currStamina;

        maxMagicBar.maxValue = maxMagic;
        maxMagicBar.value = currMaxMagic;
        currMagicBar.maxValue = maxMagic;
        currMagicBar.value = currMagic;

        maxLoyaltyBar.maxValue = maxLoyalty;
        maxLoyaltyBar.value = currMaxLoyalty;
        currLoyaltyBar.maxValue = maxLoyalty;
        currLoyaltyBar.value = currLoyalty;

        monsterPanel.SetActive(true);
        isSelected = true;
    }

    public void PseudoSelect()
    {
        selectionCircle.SetActive(true);
    }

    public void Deselect()
    {
        selectionCircle.SetActive(false);
        monsterPanel.SetActive(false);
        isSelected = false;
    }

    public override void HealMaxHealth(int toHeal)
    {
        base.HealMaxHealth(toHeal);
        if (isSelected)
        {
            maxHealthBar.value = currMaxHealth;
            currHealthBar.maxValue = currMaxHealth;
            currHealthBar.value = currHealth;
        }
    }

    public override void HealHealth(int toHeal)
    {
        base.HealHealth(toHeal);
        if (isSelected)
        {
            currHealthBar.value = currHealth;
        }
    }

    public override void DamageHealth(int toDamage)
    {
        base.DamageHealth(toDamage);
        if (isSelected)
        {
            currHealthBar.value = currHealth;
        }
    }

    public override int DamageHealth(int toDamage, int AP, List<string> types)
    {
        int toReturn = base.DamageHealth(toDamage, AP, types);
        if (isSelected){
            currHealthBar.value = currHealth;
        }
        return toReturn;
    }

    public override void HealMaxStamina(int toHeal)
    {
        base.HealMaxStamina(toHeal);
        if (isSelected)
        {
            maxStaminaBar.value = currMaxStamina;
            currStaminaBar.maxValue = currMaxStamina;
            currStaminaBar.value = currStamina;
        }
    }

    public override void HealStamina(int toHeal)
    {
        base.HealStamina(toHeal);
        if (isSelected)
        {
            currStaminaBar.value = currStamina;
        }
    }

    public override void DamageStamina(int toDamage)
    {
        base.DamageStamina(toDamage);
        if (isSelected)
        {
            currStaminaBar.value = currStamina;
        }
    }

    public override void HealMaxMagic(int toHeal)
    {
        base.HealMaxMagic(toHeal);
        if (isSelected)
        {
            maxMagicBar.value = currMaxMagic;
            currMagicBar.maxValue = currMaxMagic;
            currMagicBar.value = currMagic;
        }
    }

    public override void HealMagic(int toHeal)
    {
        base.HealMagic(toHeal);
        if (isSelected)
        {
            currMagicBar.value = currMagic;
        }
    }

    public override void DamageMagic(int toDamage)
    {
        base.DamageMagic(toDamage);
        if (isSelected)
        {
            currMagicBar.value = currMagic;
        }
    }

    public override void DamageLoyalty(int toDamage)
    {
        base.DamageLoyalty(toDamage);
        if (isSelected)
        {
            currLoyaltyBar.value = currLoyalty;
        }
    }

    public void GroupIterate()
    {
        groupAddition = false;
        target = group.GetPoint().GetPoint();
        target.x += Random.Range(-0.5f, 0.5f);
        target.z += Random.Range(-0.5f, 0.5f);
        targetChanged = true;
    }

    public PatrolGroup GetGroup()
    {
        return group;
    }

    public void SetGroup(PatrolGroup toSet)
    {
        group = toSet;
    }

    public void RemoveGroup()
    {
        if (group == null) return;
        group.RemoveMonster(this);
        group = null;
    }

    public bool GetHealthStatus()
    {
        return currMaxHealth < (maxHealth * 0.4);
    }

    public bool GetHealthMax()
    {
        return currMaxHealth == maxHealth;
    }

    public bool GetStaminaStatus()
    {
        return currMaxStamina < (maxStamina * 0.4);
    }

    public bool GetStaminaMax()
    {
        return currMaxStamina == maxStamina;
    }

    public bool GetMagicStatus()
    {
        return currMaxMagic < (maxMagic * 0.4);
    }

    public bool GetMagicMax()
    {
        return currMaxMagic == maxMagic;
    }

    public string GetName()
    {
        return monsterName;
    }
}
