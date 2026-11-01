using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class SquadBehavior : MonoBehaviour
{

    public Squad squad;
    private Stack<SquadStates> states;
    public SquadStates lastState;
    
    // For general navigation
    public Vector3 targetLocation;
    public bool arrivedAtPoint;

    // For Holding
    public CapturePoint toHold;
    public Bounds wanderZone;
    public float wanderingTime;

    // For battles:
    public Vector3 battleLocation;
    public bool readyForBattle;
    public Combat combatInfo;

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
        Debug.Log($"The state {newState} has been pushed on top of {states.Peek()}");
        states.Push(newState);
    }

    public SquadStates PopState()
    {
        SquadStates popped = states.Pop();
        Debug.Log($"The state {popped} has been popped, new active state: {states.Peek()}");
        return popped;
    }

    public void ChangeState(SquadStates newState)
    {
        Debug.Log($"The state {newState} will replace {states.Peek()}");
        Debug.LogError("ChangeState has not been implemented yet");
    }


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

    public void SquadBattleEngage(Combat combat)
    {
        SquadBattle(combat);

        PushState(SquadStates.BattleEngage);
        readyForBattle = false;
        combatInfo = combat;
        battleLocation = combatInfo.battleLocation;

    }

    public void SquadBattle(Combat combat) 
    {
        PushState(SquadStates.Battle);
    }

    public void SquadHolding(CapturePoint point)
    {
        PushState(SquadStates.Holding);
        toHold = point;
        wanderZone = point.GetComponent<BoxCollider>().bounds;
        wanderingTime = 0f;
        SquadToObjective(toHold.transform.position);
    }

    public void SquadFlee() { }
    public void SquadCelebrate() { }

    void Start()
    {
        //Debug.Log("Instantiating squad's behavior");
        squad = GetComponentInParent<Squad>();
        squad.behavior = this;
        states = new Stack<SquadStates>();
        //states.Push(SquadStates.Holding);
        //states.Push(SquadStates.ToObjective);
        //targetLocation = FindAnyObjectByType<PlayerController>().transform;
        states.Push(SquadStates.Idle);
        //Debug.Log("Objective: " + GameStateManager.GetObjectives[0]);
        int squadID = int.Parse(squad.name);
        if (squadID == 2 && squad.manager.team == Team.Defense)
        {
            squadID -= 1;
        }
        SquadHolding(GameStateManager.GetObjectives[squadID - 1]);

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
                if (combatInfo.winner != squad)
                {
                    // Squad lost the fight and died
                }
                // Squad won and can carry on
                Debug.Log("Someone won the fight");
                PopState();

                break;
            case SquadStates.Holding:
                // Wander the area, capturing/reclaiming the area while looking for potential battles
                if (wanderingTime <= 0f)
                {
                    squad.WanderInSquare(toHold);
                    wanderingTime = 7f;
                }
                else
                {
                    wanderingTime -= Time.deltaTime;
                }
                break;
            case SquadStates.Fleeing:
                SquadFlee();
                break;
            case SquadStates.Celebrate:
                SquadCelebrate();
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
        Fleeing,        // For enemies, all actions cease and they run away and escape the ship
        Celebrate,      // When battle ends, only for the team that remains
        Idle,           // Always at the bottom of the stack, has no behaviors
    }
}
