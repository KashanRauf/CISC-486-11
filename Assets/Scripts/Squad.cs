using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class Squad : NPCComponent
{
    private List<NPC> members = new List<NPC>();

    public int deadMembers { get; private set; }

    public List<NPC> GetMembers => members;
    public TeamManager manager;

    public int MemberCount()
    {
        return members.Count;
    }

    // The member with index 0 will be treated as the 'leader'
    // Their NavMeshAgent will have them go to the planned location of the Squad, and other members will follow

    public int GetMemberIndex(NPC member)
    {
        return members.IndexOf(member);
    }

    public void AddMember(NPC mem)
    {
        members.Add(mem);
        mem.squad = this;
    }

    public void RemoveMember(NPC mem)
    {
        members.Add(mem);
        mem.squad = null;
    }

    // When 2-3 members (haven't decided specific numbers) are dead the squad is 'weakened'
    // A weakened squad will want to merge with another weakened squad
    public bool IsWeakened()
    {
        return deadMembers >= 2;
    }



    void Start()
    {
        foreach(NPC m in members)
        {
            m.squad = this;
        }
    }

    void Update()
    {
        // Handles deaths
        for (int i = 0; i < members.Count; i++)
        {
            if (!members[i].IsAlive())
            {
                members[i].squad = null;

                if (manager.team == Team.Defense)
                {
                    manager.casualties.Add(members[i]);
                }

                members.RemoveAt(i);
                deadMembers++;
            }
        }
    }
} 
