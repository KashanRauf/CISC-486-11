using UnityEngine;
using UnityEngine.AI;

public class NPC : NPCComponent
{
    public int maxHealth;
    public int health;
    public int squadNum; // (or reference to Squad?)
    public float moveForce = 1f;
    public float maxSpeed = 5f;

    public Squad squad;

    public NavMeshAgent agent;
<<<<<<< Updated upstream
    //public Animator anim;
=======

    // Combat stats
    public CombatClass primary;
    public CombatClass secondary;
    public Team targets;
    public float attackDamage;
    public float cooldownTime; // The time it takes until next attack (constant)
    public float untilNextAction; // Time remaining (the actual cooldown)
    public int numTargets; // For every "attack"/"action", it will be sequentially performed numTargets times, once on each target (duplicate targets OK), -1 means AOE


    // For triggering animations
    public Animator animator;
    private SquadBehavior squadBehavior;

>>>>>>> Stashed changes
    public Vector3 direction { get; private set; } = Vector3.forward;
    public Vector3 Position => transform.position;

    public bool IsAlive()
    {
        return health > 0;
    }

    public void AddToSquad(Squad s)
    {
        s.AddMember(this);
    }

    public void RemoveFromSquad(Squad s)
    {
        s.RemoveMember(this);
    }

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
<<<<<<< Updated upstream
        
=======
        squadBehavior = GetComponentInParent<SquadBehavior>();
        // For triggering animations
        if (health <= 0)
        {
            animator.SetTrigger("death");
        }

        if (squadBehavior.CurrentState() == SquadBehavior.SquadStates.Battle)
        {
            animator.SetTrigger("attack");
        }
        // else if (squadBehavior.CurrentState() == SquadStates.sth)
        // {
        //     animator.SetTrigger("shoot");
        // }
        // else if (squadBehavior.CurrentState() == SquadStates.sth)
        // {
        //     animator.SetTrigger("heal");
        // }
        // else if (squadBehavior.CurrentState() == SquadStates.sth)
        // {
        //     animator.SetTrigger("repair");
        // }
        else
        {
            animator.SetTrigger("idle");
        }

    }


    public enum CombatClass
    {
        Medic,
        Melee,
        Ranged,
        Repair,
        Brute,
        Enchanter
>>>>>>> Stashed changes
    }

    
}