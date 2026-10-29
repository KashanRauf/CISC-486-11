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

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        squad = GetComponentInParent <Squad>();
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}