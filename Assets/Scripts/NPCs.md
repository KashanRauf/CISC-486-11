# Theoretical Implementation of NPC system

Objectives of this system:

- Ultimately, to create a system that can be used by either enemy or ally NPCs. The similarities they have means it should be possible to express them as highly-parameterized variants of the same type of entity
- Generalize the properties and features using classes and interfaces to a degree where the only major difference in their impl's would be their overall objectives/priorities and how they delegate themselves (with respect to class-specific abilities/advantages, equal(?) attention to each goal)
- Expanding functionality for the following would only take small adjustments:
    - Player could lead the attacking team 
    - Defending team could defend autonomously (requires AI for decision-making as defender)
    - Both teams could be player controlled
    - Change # of player-controlled or autonomous teams (PVP/PVPVE/PVEVE/EVE possible)


## Parts
- The NPC scripts will be split into parts for: Typing, Combat/alliance system, AI/behavior/pathfinding
- This is subject to change depending on what actually works

## Combat/Alliances
### Alliance properties
- Alliance name/enum/bool
- Objective-related behaviors
    - For either attacking or defending
    - Essentially where to go and what to do
    - Decision-making by AI or by following/refusing orders
- Combat-related behaviors
    - Choosing when to engage/ignore/run(? e.g. away from attackers/to safety)
    - Holding/taking control of a location
    - Attacking an enemy and responding to attacks
    - Using abilities and cooldown management
    - Targeting/target priority
- Navigation (pathfinding and path-following of squad/individuals)
    - To objectives (capture), locations, or entities (e.g. attack targets, to healers?)
    - Priorities: Minimize distance/time, fighting/objective/health management
    - Staying with squad (small groups within an alliance that stay together)


### Alliance info
- Defending crew: Defending the ship by killing (or fending off) enemies before objectives are captured
    - Commanded by player
- Attacking crew: Capture all objectives on the ship or kill crew members
    - No commands, autonomous decision making

## Unit Properties

### Units:
- Alliance (name/enum/bool)
- Squad (1+ units)
- Race (affects appearance and possible classes, string/enum)
- Primary class (inherit priorities of primary class but might try to combine, stat blend undetermined but probably also primary)
- Secondary class (nullable)
- State machine for AI stuff
- Mesh (based on class, has sockets for primary/secondary)
- Combat stats (inherited based on class, see google doc)

### Class:
- See google doc for values
- Maybe class is a c# class (will call "class" something like trait/archetype/whatever internally) that acts as an umbrella for all combat behavior, stats, abilities, etc.


## How to manage NPCs
- Game state manager that enforces game rules
    - Manage the enemy team
        - Spawning them in squads/waves, triggering wave deployment
        - Managing the squads
    - Manage the ally team
        - Spawning them upon round start from save data (details what the player has)
        - Grouping them into squads (groups of 1-5 NPCs), either by some system or by player decision
            - NPCs will move and operate with their squads
            - Multiple squads can be sent to the same thing (enemy/location/objective)
        - Removing them permanently when they die
        - Evaluating morale(?)
    - Handling events
        - Dialogue, cutscenes, state of objectives, win/loss conditions (death of a team, fended off enemies, ship captured)
        - Overall game flow and state
- For each team
    - List of NPCs/squads

- Squad management
    - Higher level behavior of NPCs
    - Squads have the same "task" (e.g. we need to fight this/these squads, we need to take this route to a location, we need to do something at an objective)
    - Regrouping functionality? e.g. if squads lose members they could reconvene to merge groups
    - Main priority is objectives, survival of members comes after
    - The player orders squads, not specific entities
- Individual/combat management
    - Cooldown management (e.g. healers don't heal when their squad is at full health)
    - Using a quadtree or sm for some stuff
    - Refusing an order (may break out of unit to do so)
    - Handling individual state
    - Targeting and choosing who to attack or use abilities on
    - Maintaining distance
    - Movement and animations
    - Keeping track of stats