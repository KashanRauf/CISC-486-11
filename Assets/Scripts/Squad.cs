using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class Squad : NPCComponent
{
    [SerializeField] private List<NPC> members = new List<NPC>();

    public int deadMembers { get; private set; }

    public List<NPC> GetMembers => members;
    public TeamManager manager;

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
        n.name = $"Squad member {members.Count}";

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

    // The leader navigates to the location
    public void NavigateTo(Transform t)
    {
        // TODO: Check that transform is a legal position

        //Debug.Log($"{manager.team} squad's member count: {MemberCount()}");

        // One member of the group navigates to the target
        members[0].agent.destination = t.position;

        // Follow the leader
        if (members.Count > 1)
        {
            for (int i = 1; i < members.Count; i++)
            {
                members[i].agent.destination = members[0].transform.position;
            }
        }
    }

    void Start()
    {
        //foreach(NPC m in members)
        //{
        //    m.squad = this;
        //}
    }

    void Update()
    {
        // Handles deaths
        for (int i = 0; i < members.Count; i++)
        {

            if (!members[i].IsAlive())
            {
                Debug.Log($"Member {i} is dead");
                members[i].squad = null;

                // TODO: Automatically adds any members since stats havent been made yet
                //if (manager.team == Team.Defense)
                //{
                //    manager.casualties.Add(members[i]);
                //}

                members.RemoveAt(i);
                deadMembers++;
            }
        }
    }
} 
