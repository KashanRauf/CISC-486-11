using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class SquadBehavior : MonoBehaviour
{

    // Static fields, serve as 'observations' for the squad's to make autonomous decisions
    // How many attackers/defenders are going to a location
    // What fights are ongoing
    public static int numSquads;
    public static int sternAttackers = 0;
    public static int sternDefenders = 0;
    public static int foodAttackers = 0;
    public static int foodDefenders = 0;
    public static int armouryAttackers = 0;
    public static int armouryDefenders = 0;

    public static string[] names = { "Food Storage", "Armoury", "Stern" };

    public static SortedSet<Combat> battleHistory;


    public Squad squad;
    private Stack<SquadStates> states;
    public SquadStates lastState;
    
    // For general navigation
    public Vector3 targetLocation;
    public bool arrivedAtPoint;

    // For Holding
    public CapturePoint toHold;
    public string locationName;
    public Bounds wanderZone;
    public float wanderingTime;

    // For battles:
    public Vector3 battleLocation;
    public bool readyForBattle;
    public Combat combatInfo;
    public string enemyDefending;

    // Following a command
    public bool commanded;

    //public Stack<SquadStates> GetStates => states;

    // Managing States
    public SquadStates CurrentState()
    {
        return states.Peek();
    }

    public bool StateChanged()
    {
        return lastState != CurrentState();
    }

    public void PushState(SquadStates newState)
    {
        //Debug.Log($"The state {newState} has been pushed on top of {states.Peek()}");
        states.Push(newState);
    }

    public SquadStates PopState()
    {
        SquadStates popped = states.Pop();
        //Debug.Log($"The state {popped} has been popped, new active state: {states.Peek()}");
        return popped;
    }

    public void ChangeState(SquadStates newState)
    {
        Debug.Log($"The state {newState} will replace {states.Peek()}");
        Debug.LogError("ChangeState has not been implemented yet");
    }


    // TODO: Resets data fields
    public void OnStateExit(SquadStates old)
    {
        switch (old)
        {
            case SquadStates.ToObjective:

                break;
            case SquadStates.BattleEngage:

                break;
            case SquadStates.Battle:

                break;
            case SquadStates.Holding:

                break;
            case SquadStates.Fleeing:

                break;
            case SquadStates.Celebrate:

                break;
        }
    }


    // Functions that combine previous method of pushing state and performing OnStateEnter
    public void SquadToObjective(Vector3 target)
    {
        PushState(SquadStates.ToObjective);
        targetLocation = target; // TODO: Maybe target location should also be a stack?
        arrivedAtPoint = false;
    }

    public void SquadBattleEngage(Combat combat, Squad other)
    {
        SquadBattle(combat);

        PushState(SquadStates.BattleEngage);
        readyForBattle = false;
        combatInfo = combat;
        battleLocation = combatInfo.battleLocation;

        // If the enemy team is holding a point, add this one's manpower to the observation data
        if (other.behavior.locationName != "")
        {
            enemyDefending = other.behavior.locationName;
            UpdatePower(enemyDefending);
        }
    }

    public void SquadBattle(Combat combat) 
    {
        PushState(SquadStates.Battle);
    }

    public void SquadHolding(CapturePoint point)
    {
        PushState(SquadStates.Holding);
        locationName = point.name;
        toHold = point;
        wanderZone = point.GetComponent<BoxCollider>().bounds;
        wanderingTime = 0f;
        UpdatePower(locationName);

        SquadToObjective(toHold.transform.position);
    }

    public void CommandHolding(CapturePoint point)
    {
        PushState(SquadStates.Command);
        commanded = true;
        SquadHolding(point);
    }

    public void CommandBattle()
    {
        PushState(SquadStates.Command);
        commanded = true;
        //SquadBattleEngage();
    }

    public void SquadFlee() { }
    public void SquadCelebrate() { }

    // Helper functions for decisions

    // Updates the observation data
    public void UpdatePower(string name)
    {
        UpdatePower(squad.MemberCount(), name);
    }

    public void UpdatePower(int amount, string name)
    {
        bool isAttacking = squad.GetTeam == Team.Attack;
        switch (name)
        {
            case "Food Storage":
                if (isAttacking) foodAttackers += amount;
                else foodDefenders += amount;
                break;
            case "Stern":
                if (isAttacking) sternAttackers += amount;
                else sternDefenders += amount;
                break;
            case "Armoury":
                if (isAttacking) armouryAttackers += amount;
                else armouryDefenders += amount;
                break;
        }
    }

    // The difference in number of attackers/defenders at a location
    public int PointContesting(string name)
    {
        if (name == "Food Storage")
        {
            return foodAttackers - foodDefenders;
        } else if (name == "Stern")
        {
            return sternAttackers - sternDefenders;
        } else if (name == "Armoury")
        {
            return armouryAttackers - armouryDefenders;
        }
        throw new System.Exception("PointContesting issue");
    }

    // If the squad can leave the position they've been assigned to attack/defend (PointContesting = either pos or neg depending on team)
    public bool SufficientManpower(string name)
    {
        if (squad.GetTeam == Team.Attack)
        {
            // Want pointcontesting to be positive, if it's 0 then leaving would mean there is no longer enough
            return PointContesting(name) > 0;
        } else
        {
            // Same logic as above, but want it to be negative
            return PointContesting(name) < 0;
        }
    }

    public int NeedsPower() 
    {
        int foodPower = PointContesting("Food Storage");
        int armPower = PointContesting("Armoury");
        int sternPower = PointContesting("Stern");
        int allocatedPower = sternAttackers + sternDefenders + foodAttackers + foodDefenders + armouryAttackers + armouryDefenders;

        int squadSize = squad.GetMembers.Count;
        int bestChoice = -1;
        int bestScore = int.MinValue;
        
        int[] powers = { PointContesting(names[0]), PointContesting(names[1]), PointContesting(names[2]) };

        // Evaluate how the situation would change depending on which point they attack
        for (int i = 0; i < 3; i++)
        {
            // Normalizes power so that negative = disadvantage and positive = advantage
            int teamPower = squad.GetTeam == Team.Attack ? powers[i] : powers[i] * -1;
            int projectedPower = teamPower + squadSize;
            int score = 0;

            // Will attacking this point swing an enemy advantage into our own?
            if (teamPower < 0 && projectedPower > 0) score = 100;
            // If not, can we even the scales somewhere?
            else if (teamPower < 0 && projectedPower == 0) score = 75;
            // Can we establish an advantage at a place that's even?
            else if (teamPower == 0) score = 50;
            // Already winning, not necessary to reinforce so it's the worst option
            else score = 10;

            // Add a bias to the places that need fewer reinforces
            score -= Mathf.Abs(teamPower);

            // Evaluate if this is the new best option
            if (score > bestScore)
            {
                bestScore = score;
                bestChoice = i;
            }
        }

        return bestChoice;
    }

    // Autonomous decision making using the static observation data
    // Returns true if it decides to leave the current post
    public bool AutonomousDecision(SquadStates cur)
    {
        Debug.Log($"{squad.GetTeam} {squad.name} is looking for something to do");
        // If we're already doing something (e.g. holding), should we stay?
        // e.g. is holding a point, but its already in possession and not under attack -> leave
        bool shouldLeave = 
            (cur == SquadStates.Idle) ||
            (cur == SquadStates.Holding && squad.GetTeam == toHold.owner && SufficientManpower(locationName)) ;
        if (!shouldLeave) return false; // Stay


        // Should we pick a fight or capture an objective?
        // Do any objectives need man-power?
        // (sum all points attackers - sum all points defenders)
        // `-> neg -> need more attackers
        // `-> 0 -> idk
        // `-> pos -> need more defenders

        // Who should we fight (want to minimize losses)
        // Find the weakest squad and pick them off

        // Which objective should we go to (in possession of enemy team or under attack by them)
        // each point attackers - each point defenders
        // `-> neg -> more attackers would help
        // `-> 0 -> there should be ongoing battles
        // `-> pos -> more defenders would help

        // Finds if there is any point that we should go to
        int target = NeedsPower();

        if (target != -1)
        {
            Debug.Log($"Current location: {locationName}, chosen location: {names[target]}");
            SquadHolding(GameStateManager.NameToObjective(names[target]));
            return true;
        }

        // Otherwise find somewhere to go, pretty much looking for a fight
        List<Vector3> placesToGo = new List<Vector3>();
        foreach (Squad s in squad.manager.squads)
        {
            if (s.behavior.CurrentState() != SquadStates.Holding) placesToGo.Add(s.MidpointLocation());
        }
        if (placesToGo.Count == 0)
        {
            foreach (CapturePoint c in GameStateManager.GetObjectives)
            {
                placesToGo.Add(c.RandomPoint());
            }
        }
        // Randomly pick one of those locations
        Debug.Log($"Looking for a fight");
        SquadToObjective(placesToGo[Random.Range(0, placesToGo.Count)]);
        return true;
    }

    void Start()
    {
        //Debug.Log("Instantiating squad's behavior");
        squad = GetComponentInParent<Squad>();
        squad.behavior = this;
        states = new Stack<SquadStates>();
        states.Push(SquadStates.Idle);

        commanded = false;
        enemyDefending = "";

    }

    void FixedUpdate()
    {
        SquadStates cur = CurrentState();

        // TODO: Add formations: offset positions for NavigateTo/etc to move in a clean unit and not a dogpile.
        //       Formations for movement and for battle, battle which spot in formation by class
        //       Formations are dependent on squad size so there should be a cap (5-7)

        if (StateChanged())
        {
            // On State Enter, resets variables needed
            //OnStateEnter(cur);
        }

        switch (cur)
        {
            case SquadStates.ToObjective:
                // Check if there's an enemy squad to fight
                //Debug.Log($"{squad.name} is looking for a fight");
                squad.DetectEnemies();
                // Check if squad has arrived at point otherwise continue going there
                if (arrivedAtPoint)
                {
                    //Debug.Log("Arrived");
                    PopState();
                } else
                {
                    arrivedAtPoint = squad.NavigateTo(targetLocation);
                }
                break;
            case SquadStates.BattleEngage:
                // Both squads will find a midpoint and gather around there to fight
                // If one of the squads is holding a point then they will fight there
                if (readyForBattle)
                {
                    PopState();
                } else
                {
                    readyForBattle = squad.NavigateTo(battleLocation);
                }
                break;
            case SquadStates.Battle:
                if (combatInfo.winner == null) break;
                Debug.Log("Someone won the fight");
                if (combatInfo.winner != squad)
                {
                    //    // Squad lost the fight and died
                    //    // Remove their manpower
                    if (enemyDefending != "") UpdatePower(squad.MemberCount() * -1, enemyDefending);
                    else if (locationName != "") UpdatePower(squad.MemberCount() * -1, locationName);
                }
                PopState();

                break;
            case SquadStates.Holding:
                // Wander the area, capturing/reclaiming the area while looking for potential battles
                if (wanderingTime <= 0f)
                {
                    // Give squad a chance to go elsewhere
                    if (AutonomousDecision(cur)) break;
                    squad.WanderInSquare(toHold);
                    wanderingTime = 7f;
                }
                else
                {
                    wanderingTime -= Time.deltaTime;
                }
                break;
            case SquadStates.Command:
                // Should only be in this state after popping the actual actions of the command
                
                break;
            case SquadStates.Fleeing:
                SquadFlee();
                break;
            case SquadStates.Celebrate:
                SquadCelebrate();
                break;
            case SquadStates.Idle:
                AutonomousDecision(cur);
                break;
        }

        lastState = cur;
    }

    // Stack-based state machine for the squad
    // This meshes well with the intended behavior, can pop 'battle' to go right back to their goal
    public enum SquadStates
    {
        //Wander,
        ToObjective,    // Captain's command or a choice by AI to go somewhere (an objective)
        BattleEngage,   // Entering a battle
        Battle,         // Currently in a battle, doesn't end unless they or opponents die (unless they are commanded?)
        Holding,        // Holding an objective for capture or defense
        Command,        // The actions on top of a command state are the orders received
        Fleeing,        // For enemies, all actions cease and they run away and escape the ship
        Celebrate,      // When battle ends, only for the team that remains
        Idle,           // Always at the bottom of the stack, has no behaviors
    }
}
