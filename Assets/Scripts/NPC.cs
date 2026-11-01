using UnityEngine;
using UnityEngine.AI;

public class NPC : NPCComponent
{
    public float maxHealth;
    public float health;

    public Squad squad;
    public NavMeshAgent agent;

    // Combat stats
    public CombatClass primary;
    public CombatClass secondary;
    public Team targets;
    public float attackDamage;
    public float cooldownTime; // The time it takes until next attack (constant)
    public float untilNextAction; // Time remaining (the actual cooldown)
    public int numTargets; // For every "attack"/"action", it will be sequentially performed numTargets times, once on each target (duplicate targets OK), -1 means AOE



    //public Animator anim;
    public Vector3 direction { get; private set; } = Vector3.forward;
    public Vector3 Position => transform.position;

    public bool IsAlive()
    {
        return health > 0;
    }

    public void AddToSquad(Squad s)
    {
        s.GetMembers.Add(this);
        squad = s;
    }

    public void RemoveFromSquad(Squad s)
    {
        s.GetMembers.Remove(this);
        squad = null;
    }

    // For navigation
    public bool HasArrived(Vector3 loc)
    {
        //Debug.Log($"{Vector3.Distance(loc, this.transform.position)}");
        float distFrom = Vector3.Distance(this.transform.position, loc);
        return distFrom < 15 || (distFrom < 36 && this.agent.velocity.sqrMagnitude < 0.001f);
    }

    public void ClampHealth()
    {
        health = Mathf.Min(health, maxHealth);
    }

    // TODO: Secondary class implementation (I am cutting corners for the sake of the deadline)
    // Initializing base stats depending on class and team
    private void InitializeStats()
    {
        if (squad.manager.team == Team.Defense)
            targets = Team.Attack;
        else
            targets = Team.Defense;

        switch (primary)
        {
            // Added more hp compared to coordination because i feel like they die too fast
            case CombatClass.Medic:
                maxHealth = 500f;
                // 20 AOE healing every 5 seconds, targets own squad
                attackDamage = 20f;
                cooldownTime = 5f;
                targets = this.squad.manager.team;
                numTargets = -1;
                
                break;
            case CombatClass.Melee:
                maxHealth = 600f;
                // 100 single target damage every 5 seconds
                // Changed to 60 every 3, still same DPS but having differences would make combat look less rigid
                attackDamage = 60f;
                cooldownTime = 3f;
                numTargets = 1;
                break;
            case CombatClass.Ranged:
                maxHealth = 450;
                // 75 x 2 damage every 5 seconds
                // Changed to 90 x 2 every 6 seconds for same reasons as above
                attackDamage = 90f;
                cooldownTime = 6f;
                numTargets = 2;
                break;
            case CombatClass.Repair:
                maxHealth = 300f;
                // 20 Repair per second (originally per 5 but idk)
                attackDamage = 20f;
                cooldownTime = 1f;
                numTargets = 1;
                break;
            case CombatClass.Brute:
                maxHealth = 600f;
                // 150 single target damage every 5 second
                // Changed to 240 every 8 seconds
                attackDamage = 240f;
                cooldownTime = 8f;
                numTargets = 1;
                break;
        }
        health = maxHealth;
        untilNextAction = 0f;

        agent.acceleration = 10f;
        agent.speed = 10f;
        // Defenders are a bit faster
        if (squad.manager.team == Team.Defense)
        {
            agent.acceleration = 12f;
            agent.speed = 12f;
        }
    }

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        squad = GetComponentInParent<Squad>();
        if (this.squad.manager.team == Team.Defense)
        {
            gameObject.layer = 9;
        } else if (this.squad.manager.team == Team.Attack)
        {
            gameObject.layer = 10;
        }
        InitializeStats();
        
    }

    // Update is called once per frame
    void Update()
    {

    }


    public enum CombatClass
    {
        Medic,
        Melee,
        Ranged,
        Repair,
        Brute,
        Enchanter
    }
}