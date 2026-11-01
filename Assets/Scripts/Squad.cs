using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;
using Unity.VisualScripting;

public class Squad : NPCComponent
{
    [SerializeField] private List<NPC> members = new List<NPC>();

    public int deadMembers { get; private set; }

    public List<NPC> GetMembers => members;
    public TeamManager manager;
    public SquadBehavior behavior;
    public float battleEngageRadius = 24f;
    public int enemyTeamLayer;


    public int MemberCount()
    {
        return this.members.Count;
    }

    // The member with index 0 will be treated as the 'leader'
    // Their NavMeshAgent will have them go to the planned location of the Squad, and other members will follow

    public int GetMemberIndex(NPC member)
    {
        return members.IndexOf(member);
    }


    public bool IsSquadLeader(NPC m)
    {
        return members[0] == m;
    }

    public NPC GetSquadLeader()
    {
        return members[0];
    }

    public void AddMember(NPC mem)
    {
        members.Add(mem);
        mem.squad = this;
    }

    public void RemoveMember(NPC mem)
    {
        members.Remove(mem);
        mem.squad = null;
    }

    public NPC SpawnMember(GameObject prefab)
    {
        //Debug.Log($"Spawning a member for {manager.team}");x
        NPC n = Instantiate(prefab, this.transform).GetComponent<NPC>();
        n.name = $"{this.manager.team} member {members.Count}";

        AddMember(n);

        return n;
    }

    public List<NPC> SpawnMembers(List<GameObject> prefabs)
    {
        List<NPC> spawned = new List<NPC>();

        foreach (GameObject o in prefabs)
        {
            NPC spawn = SpawnMember(o);
            spawn.transform.position += new Vector3(Random.Range(prefabs.Count * -1f, prefabs.Count), 0, Random.Range(prefabs.Count * -1f, prefabs.Count));
            spawned.Add(spawn);
        }

        return spawned;
    }

    // When 2-3 members (haven't decided specific numbers) are dead the squad is 'weakened'
    // A weakened squad will want to merge with another weakened squad
    public bool IsWeakened()
    {
        return deadMembers >= 2;
    }

    // Determines the leader by checking who is closest to the objective
    public void NewLeader(Vector3 loc)
    {
        float shortestDistance = float.MaxValue;
        int shortestIdx = -1;


        for (int i = 0; i < members.Count; i++)
        {
            if (members[i].HasArrived(loc)) continue;

            float distance = Vector3.Distance(loc, members[i].transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                shortestIdx = i;
            }
        }
        

        if (shortestIdx != -1)
        {
            NPC temp = members[0];
            members[0] = members[shortestIdx];
            members[shortestIdx] = temp;
        }
    }

    // The leader navigates to the location
    public bool NavigateTo(Vector3 loc)
    {
        // TODO: Check that transform is a legal position

        // Determines the leader (closest to location) and follows them
        NewLeader(loc);
        members[0].agent.SetDestination(loc);

        // Follow the leader
        if (members.Count > 1)
        {
            for (int i = 1; i < members.Count; i++)
            {
                members[i].agent.SetDestination(members[0].transform.position);
            }
        }

        return SquadHasArrived(loc);
    }

    public bool SquadHasArrived(Vector3 loc)
    {
        foreach (var mem in members)
        {
            if (!mem.HasArrived(loc)) return false;
            //Debug.Log($"{mem.name} has arrived");
        }
        return true;
    }

    // Squad wanders within an area
    public void WanderInSquare(CapturePoint point)
    {
        // Pick a random point within the square for each member
        // Members wander to their point

        
        for (int i = 0; i < members.Count; i++)
        {
            Vector3 position = point.RandomPoint();
            //while (Vector3.Distance(position, members[i].transform.position) > 6f)
            //{
            //    position = point.RandomPoint();
            //}

            members[i].agent.SetDestination(position);
        }
    }

    // The midpoint of squad members to determine *roughly* where they are
    public Vector3 MidpointLocation()
    {
        Vector3 location = Vector3.zero;

        for (int i = 0; i < members.Count; i++)
        {
            location += members[i].transform.position;
        }

        location /= members.Count;

        return location;
    }

    public void DetectEnemies()
    {
        // Checks what squads are in the engage radius
        HashSet<Squad> nearby = new HashSet<Squad>();
        Collider[] hits = Physics.OverlapSphere(members[0].transform.position, battleEngageRadius, enemyTeamLayer);

        foreach (Collider hit in hits)
        {
            Debug.Log($"{this} detected a {hit.name}");
            Squad ofDetected = hit.GetComponentInParent<NPC>()?.squad;
            if (ofDetected == null) continue;
            nearby.Add(ofDetected);
        }
        //Debug.Log($"Found {nearby.Count} possible squads to fight");

        // Checks if any of those squads should be challenged and issues a challenge, if challenge declined then try the next
        foreach (Squad s in nearby)
        {
            Debug.Log($"A challenge was issued from {this} to {s}");
            bool requestAccepted = s.ChallengeRequest();
            if (requestAccepted)
            {

                Combat.ChallengeAccepted(this, s);
                break;
            }
        }
    }

    public bool ChallengeRequest()
    {
        // States where fights can happen: ToObjective, Holding
        SquadBehavior.SquadStates cur = this.behavior.CurrentState();
        Debug.Log($"{cur}");
        return (this.behavior.CurrentState() == SquadBehavior.SquadStates.ToObjective || this.behavior.CurrentState() == SquadBehavior.SquadStates.Holding);
    }

    void Start()
    {
        if (manager.team == Team.Defense)
        {
            enemyTeamLayer = (1 << 10);
        } else
        {
            enemyTeamLayer = (1 << 9);
        }
    }

    void Update()
    {
        List<NPC> dead = new List<NPC>();
        // Handles deaths
        for (int i = 0; i < members.Count; i++)
        {

            if (!members[i].IsAlive())
            {
                Debug.Log($"Member {i} is dead");
                //members[i].squad = null;

                // TODO: Automatically adds any members since stats havent been made yet
                //if (manager.team == Team.Defense)
                //{
                //    manager.casualties.Add(members[i]);
                //}

                dead.Add(members[i]);
                members.RemoveAt(i);
                deadMembers++;
            }

            foreach (NPC n in dead)
            {
                members.Remove(n);
                Destroy(n.gameObject);
            }
        }

        if (members.Count == 0)
        {
            manager.squads.Remove(this);
            Destroy(this.gameObject);
        }

        // Squad gameobject follows its members
        //if (members.Count > 0) this.gameObject.transform.position = MidpointLocation();
    }
} 
