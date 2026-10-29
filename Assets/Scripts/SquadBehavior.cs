using System.Collections.Generic;
using UnityEngine;

public class SquadBehavior : MonoBehaviour
{

    private Stack<SquadStates> states;
    public Transform targetLocation;

    public Stack<SquadStates> GetStates => states;
    public Squad squad;

    public SquadStates lastState;


    void Start()
    {
        Debug.Log("Instantiating squad's behavior");
        squad = GetComponentInParent<Squad>();
        states = new Stack<SquadStates>();
        states.Push(SquadStates.ToObjective);
        targetLocation = FindAnyObjectByType<PlayerController>().transform;
    }

    void Update()
    {
        SquadStates cur = CurrentState();

        // TODO: Add formations: offset positions for NavigateTo/etc to move in a clean unit and not a dogpile.
        //       Formations for movement and for battle, battle which spot in formation by class
        //       Formations are dependent on squad size so there should be a cap (5-7)

        switch (cur)
        {
            case SquadStates.ToObjective:
                // Make the leader NPC go towards a point and make squad members follow
                squad.NavigateTo(targetLocation);
                break;
            case SquadStates.BattleEngage:
                // Both squads will find a midpoint and gather around there to fight
                break;
            case SquadStates.Battle:
                break;
            case SquadStates.Holding:
                // Today's plan is to create the objective control and capture system
                break;
            case SquadStates.Fleeing:
                break;
            case SquadStates.Celebrate:
                break;
        }

        lastState = cur;
    }

    public SquadStates CurrentState()
    {
        return states.Peek();
    }

    public bool StateChanged()
    {
        return lastState != CurrentState();
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
    }
}
