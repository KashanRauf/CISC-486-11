using UnityEngine;
using UnityEngine.AI;

public class NPC : NPCComponent
{
    public int maxHealth;
    public int health;
    public float moveForce = 15f;
    public float maxSpeed = 15f;
    public float attackDamage = 15f;

    public Squad squad;
    public NavMeshAgent agent;

    //public Animator anim;
    public Vector3 direction { get; private set; } = Vector3.forward;
    public Vector3 Position => transform.position;

    public bool IsAlive()
    {
        return health > 0 || maxHealth == 0;
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
    public bool HasArrived(Transform t)
    {
        return Vector3.Distance(this.transform.position, t.position) < agent.stoppingDistance;
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        squad = GetComponentInParent <Squad>();
        if (this.squad.manager.team == Team.Defense)
        {
            gameObject.layer = 9;
        } else if (this.squad.manager.team == Team.Attack)
        {
            gameObject.layer = 10;
        }
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}