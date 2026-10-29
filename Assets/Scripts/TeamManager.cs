using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TeamManager : MonoBehaviour
{
    [SerializeField] public Team team;
    [SerializeField] public List<Squad> squads = new List<Squad>();

    //public List<NPC> casualties;


    [ContextMenu("Create Squad")]
    public Squad CreateSquad()
    {
        Squad squad = Instantiate(
            Prefabs.prefabs.newSquad, 
            this.transform)
            .GetComponent<Squad>();
        squad.manager = this;
        squads.Add(squad);
        

        squad.name = $"{team} squad {squads.Count}";
        //squad.SpawnMember(Prefabs.prefabs.testEnemy);

        List<GameObject> toSpawn = new List<GameObject>(new GameObject[] { Prefabs.prefabs.testEnemy, Prefabs.prefabs.testEnemy, Prefabs.prefabs.testEnemy });
        squad.SpawnMembers(toSpawn);

        return squad;
    }

    public Squad CreateSquadWithMembers(List<NPC> m)
    {
        return new Squad();
    }

    public Squad MergeSquads(Squad s1, Squad s2)
    {
        // Disbands the input squads and creates a new one with members of both teams
        List<NPC> m = new List<NPC>();
        m.AddRange(s1.GetMembers);
        m.AddRange(s2.GetMembers);

        // Disband the squads

        return CreateSquadWithMembers(m);
    }

    // For triggering a win/loss event when all members of a team die (the same events can be triggered by attackers fleeing or objectives being lost)
    //public bool IsTeamAlive()
    //{
    //    return false;
    //}

    // Returns a list of the squads (index) that are down at least X amount of members, in order of most members lost first
    public List<int> WeakenedSquads()
    {
        List<int> weakened = new List<int>();
        for (int i = 0; i < squads.Count; i++)
        {
            if (squads[i].IsWeakened()) weakened.Add(i);
        }

        return weakened;
    }

    void Start()
    {
        //Debug.Log($"Spawning a squad for {team}");
        CreateSquad();

        // Need to keep track of which members have died so that it's reflected in later battles
        //if (team == Team.Defense)
        //{
        //    casualties = new List<NPC>();
        //    casualties.Clear();
        //}
    }
}


public enum Team
{
    Attack,
    Defense
}

public enum Debuffs
{
    LowMorale, // Possibility of ignoring commands, reduced stats(?)
    Controlled, // Siren effect
    Stern, // Reduces morale
}

public enum Buffs
{
    Weapons, // For the team possessing the weapons storage (or just enemies), attack bonus
    MassHeal, // When food storage is captured, heals enemies completely (single use), defenders not currently in battle are gradually healed by a small amount
}