using UnityEngine;

public class SquadBehavior : MonoBehaviour
{
    // Stack-based state machine for the squad
    // This meshes well with the intended behavior, can pop 'battle' to go right back to their goal
    public enum SquadStates
    {
        //Wander,
        BattleEngage,   // Entering a battle
        Battle,         // Currently in a battle, doesn't end unless they or opponents die (unless they are commanded?)
        Holding,        // Holding an objective for capture or defense
        ToObjective,    // Captain's command or a choice by AI to go somewhere (an objective)
        Fleeing,        // For enemies, all actions cease and they run away and escape the ship
        Celebrate,      // When battle ends, only for the team that remains
    }
}
