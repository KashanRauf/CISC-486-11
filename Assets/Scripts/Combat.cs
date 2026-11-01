using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static SquadBehavior;

public class Combat : MonoBehaviour
{
    public Squad squad1;
    public Squad squad2;
    public Vector3 battleLocation;
    public bool squadsAreReady;
    public List<NPC> fighters;
    public Squad winner = null;


    public static void ChallengeAccepted(Squad challenger, Squad challenged)
    {
        Combat c = Instantiate(Prefabs.prefabs.combat).GetComponent<Combat>();
        c.squad1 = challenger;
        c.squad2 = challenged;
        c.squadsAreReady = false;
        c.winner = null;
        c.ChooseBattleLocation();

        c.fighters = new List<NPC>();
        foreach (NPC n in c.squad1.GetMembers)
        {
            c.fighters.Add(n);
        }
        foreach (NPC n in c.squad2.GetMembers)
        {
            c.fighters.Add(n);
        }

        challenger.behavior.SquadBattleEngage(c);
        challenged.behavior.SquadBattleEngage(c);
    }

    public bool CheckSquadsReady()
    {
        //Debug.Log($"squad1 {squad1.behavior.CurrentState()}, squad2 {squad2.behavior.CurrentState()}");
        if (squad1.behavior.CurrentState() == SquadBehavior.SquadStates.BattleEngage || squad2.behavior.CurrentState() == SquadBehavior.SquadStates.BattleEngage)
        {
            squadsAreReady = false;
            return false;
        } else if (squad1.behavior.CurrentState() == SquadBehavior.SquadStates.Battle && squad2.behavior.CurrentState() == SquadBehavior.SquadStates.Battle)
        {
            squadsAreReady = true;
            return true;
        } else
        {
            Debug.LogError("Battle.CheckSquadsReady has encountered an error");
            return false;
        }
    }

    public void ChooseBattleLocation()
    {
        // Set the battle location for them to meet at
        // Check if either squad is holding a point otherwise meet in the middle
        if (squad1.behavior.CurrentState() == SquadStates.Holding)
            battleLocation = squad1.behavior.toHold.transform.position;
        else if (squad2.behavior.CurrentState() == SquadStates.Holding)
            battleLocation = squad2.behavior.toHold.transform.position;
        else
            battleLocation = (squad1.MidpointLocation() + squad2.MidpointLocation()) / 2;
    }

    //void Awake()
    //{
    //    ChooseBattleLocation();
    //    this.transform.position = battleLocation;
    //}

    void FixedUpdate()
    {
        if (!squadsAreReady)
        {
            //Debug.Log("Fighters not ready");
            CheckSquadsReady();
            return;
        } 

        if (winner != null)
        {
            return;
        }

        //Debug.Log("Fight is ongoing");
        // Make them fight
        foreach (NPC fighter in fighters)
        {
            Debug.Log($"{fighter} must wait {fighter.untilNextAction}");
            if (fighter.IsAlive() && fighter.untilNextAction <= 0)
            {
                //Debug.Log(fighter.numTargets);
                if (fighter.numTargets > 1)
                {
                    PerformMultiAttack(fighter);
                } else if (fighter.numTargets == -1)
                {
                    PerformAreaAttack(fighter);
                } else
                {
                    PerformSingleAttack(fighter);
                }
            } else
            {
                fighter.untilNextAction -= Time.deltaTime * 1.4f;
            }
        }

        List<NPC> dead = new List<NPC>();
        foreach (NPC fighter in fighters)
        {
            
            if (!fighter.IsAlive()) 
            {
                dead.Add(fighter);
                //Debug.Log(fighter + " is dead " + fighter.health + " " + fighter.IsAlive());
            }
        }

        foreach (NPC d in dead)
        {
            if (fighters.Count > 2)
                fighters.Remove(d);
            else
                d.health += 150;
            d.transform.position += Vector3.down;
        }

        // Checks if a team has won
        bool flag = true;

        //Debug.Log("FUCK " + fighters.Count);
        Team last = fighters[0].squad.manager.team;
        for (int i = 1; i < fighters.Count; i++)
        {
            if (fighters[i].squad.manager.team != last) { flag = false; break; }
        }
        if (flag)
        {
            winner = fighters[0].squad;
        }
    }

    public void PerformMultiAttack(NPC f)
    {
        NPC[] targets = ChooseTargets(f.targets, f.numTargets);
        Debug.Log(targets.Length);
        foreach (NPC n in targets)
        {
            n.health -= f.attackDamage;
            //if (!n.IsAlive()) fighters.Remove(n);
        }
        f.untilNextAction = f.cooldownTime;
    }

    public void PerformAreaAttack(NPC f)
    {
        // Targets everyone of the target team
        foreach (NPC n in fighters)
        {
            if (n.squad.manager.team == f.targets)
            {
                if (f.primary == NPC.CombatClass.Medic)
                {
                    n.health += f.attackDamage;
                    n.ClampHealth();
                } else
                {
                    n.health -= f.attackDamage;
                    //if (!n.IsAlive()) 
                    //{ 
                    //    fighters.Remove(n); 
                    //}
                }
            }
        }
        f.untilNextAction = f.cooldownTime;
    }

    public void PerformSingleAttack(NPC f)
    {
        NPC n = ChooseTarget(f.targets);
        n.health -= f.attackDamage;
        //if (!n.IsAlive()) fighters.Remove(n);
        f.untilNextAction = f.cooldownTime;
    }

    public NPC[] ChooseTargets(Team t, int num)
    {
        // Prioritization stuff for later
        //switch (f.primary)
        //{
        //    case NPC.CombatClass.Melee:
        //        break;
        //    case NPC.CombatClass.Ranged:
        //        break;
        //    case NPC.CombatClass.Medic:
        //        break;
        //    case NPC.CombatClass.Repair:
        //        break;
        //    case NPC.CombatClass.Brute:
        //        break;
        //}

        // For now we just choose randomly
        NPC[] targets = new NPC[num];
        for (int i = 0; i < targets.Length; i++)
        {
            // Duplicates are ok, they'll get hit multiple times
            targets[i] = ChooseTarget(t);
        }

        return targets;
    }

    public NPC ChooseTarget(Team t)
    {
        // TODO: Wasteful to compute this every time
        List<NPC> choices = new List<NPC>();
        foreach (NPC n in fighters)
        {
            if (n.squad.manager.team == t) choices.Add(n);
        }

        int pick = Random.Range(0, choices.Count);
        return choices[pick];
    }

}
