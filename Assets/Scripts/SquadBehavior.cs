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
    public Transform targetLocation;
    public bool arrivedAtPoint;

    // For Holding
    public CapturePoint toHold;
    public Bounds wanderZone;
    public float wanderingTime;

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
    public void SquadToObjective(Transform target)
    {
        PushState(SquadStates.ToObjective);
        targetLocation = target;
        arrivedAtPoint = false;
    }

    public void SquadBattleEngage() { }
    public void SquadBattle() { }

    public void SquadHolding(CapturePoint point)
    {
        PushState(SquadStates.Holding);
        toHold = point;
        wanderZone = point.GetComponent<BoxCollider>().bounds;
        wanderingTime = 0f;
        SquadToObjective(toHold.transform);
    }

    public void SquadFlee() { }
    public void SquadCelebrate() { }

    void Start()
    {
        //Debug.Log("Instantiating squad's behavior");
        squad = GetComponentInParent<Squad>();
        states = new Stack<SquadStates>();
        //states.Push(SquadStates.Holding);
        //states.Push(SquadStates.ToObjective);
        //targetLocation = FindAnyObjectByType<PlayerController>().transform;
        states.Push(SquadStates.Idle);
        Debug.Log("Objective: " + GameStateManager.GetObjectives[0]);
        SquadHolding(GameStateManager.GetObjectives[0]);
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

                SquadBattleEngage();
                break;
            case SquadStates.Battle:
                SquadBattle();
                break;
            case SquadStates.Holding:
                // Wander the area, capturing/reclaiming the area while looking for potential battles
                //Debug.Log(wanderingTime);
                if (wanderingTime <= 0f)
                {
                    // Designate a new place to wander to
                    Debug.Log("Squad will now wander");
                    squad.WanderInSquare(toHold);
                    wanderingTime = 5f;
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
