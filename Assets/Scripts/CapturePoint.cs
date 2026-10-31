using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class CapturePoint : MonoBehaviour
{
    public Team owner;
    public float maxHealth = 400f;
    public float captureHealth;
    public float captureRate;

    private BoxCollider collider;

    HashSet<NPC> attackers = new HashSet<NPC>();
    HashSet<NPC> defenders = new HashSet<NPC>();


    public void ClampHealth()
    {
        // If owned by defenders, captureHealth can't be greater than maxHealth
        // If owned by attackers, captureHealth can't be greater than maxHealth/2
        if (owner == Team.Defense)
        {
            captureHealth = Mathf.Min(captureHealth, maxHealth);
        } else
        {
            captureHealth = Mathf.Min(captureHealth, maxHealth / 2);
        }
    }

    public bool SquadAtPoint(Squad s)
    {
        if (s.manager.team == Team.Attack)
        {
            foreach (NPC n in s.GetMembers)
            {
                if (!attackers.Contains(n))
                {
                    return false;
                }
            }

            return true;
        } else
        {
            foreach (NPC n in s.GetMembers)
            {
                if (!defenders.Contains(n))
                {
                    return false;
                }
            }

            return true;
        }
    }

    public void OnCapture()
    {
        owner = Team.Attack;
        captureHealth = maxHealth / 2;
        Debug.Log("Attackers have claimed the capture point");
        CalculateCaptureProgress();
    }

    public void OnReclaim()
    {
        owner = Team.Defense;
        captureHealth = maxHealth;
        Debug.Log("Defenders have reclaimed the capture point");
        CalculateCaptureProgress();
    }

    // Checks if there are attackers occupying the point
    public bool UnderAttack()
    {
        bool flag = false;

        foreach (NPC n in attackers)
        {
            Team t = n.squad.manager.team;
            SquadBehavior.SquadStates s = n.squad.behavior.CurrentState();
            if (t == Team.Attack && (s != SquadBehavior.SquadStates.ToObjective))
            {
                flag = true;
            }
        }

        return flag;
    }

    // Checks if there are defenders occupying the point
    public bool UnderRepair()
    {
        bool flag = false;

        foreach (NPC n in defenders)
        {
            Team t = n.squad.manager.team;
            SquadBehavior.SquadStates s = n.squad.behavior.CurrentState();
            if (t == Team.Defense && (s != SquadBehavior.SquadStates.ToObjective))
            {
                flag = true;
            }
        }

        return flag;
    }

    // Only called when an NPC enters/leaves area or when the point's owner changes
    private void CalculateCaptureProgress()
    {
        // Instead of being deadlocked when both teams are contesting (this should pretty much always mean they're fighting):
        // Attackers can still capture when the point is owned by defenders
        // Defenders can still repair when the point is owned by attackers

        float damage = 0;

        if (owner == Team.Defense)
        {
            foreach (NPC a in attackers)
            {
                damage += a.attackDamage;
            }
            // TODO: Repairers can reduce attacker damage
        } else
        {
            foreach (NPC d in defenders)
            {
                damage += d.attackDamage/2;
                // TODO: Repairers have a bonus to their progress
            }
        }

        captureRate = damage;
    }

    public Vector3 RandomPoint()
    {
        Bounds bounds = this.collider.bounds;
        //Debug.Log($"Wander in square: min ({bounds.min.x},{bounds.min.z}), max ({bounds.max.x},{bounds.max.z})");
        Vector3 randomPoint = new Vector3(Random.Range(bounds.min.x, bounds.max.x), bounds.center.y, Random.Range(bounds.min.z, bounds.max.z));

        // Turn that into a point on the NavMesh
        NavMeshHit hit;
        NavMesh.SamplePosition(randomPoint, out hit, 12f, 1);

        //Debug.Log($"Calculated position: ({hit.position.x}, {hit.position.y}, {hit.position.z})");

        return hit.position;
    }

    private HashSet<NPC> GetTeamSet(NPC n)
    {
        if (n.squad.manager.team == Team.Defense)
        {
            return defenders;
        }

        return attackers;
    }

    private void OnTriggerEnter(Collider other)
    {
        NPC coll = other.GetComponent<NPC>();

        GetTeamSet(coll).Add(coll);

        CalculateCaptureProgress();

        Debug.Log("An NPC has entered the point, capture rate: " + captureRate);
    }

    private void OnTriggerExit(Collider other)
    {
        NPC coll = other.GetComponent<NPC>();
        
        if (!GetTeamSet(coll).Remove(coll))
        {
            Debug.LogError("Unexpected behavior: an NPC left the zone without being added to the list in the first place");
        }

        CalculateCaptureProgress();
        Debug.Log("An NPC has left the point, capture rate: " + captureRate);
    }

    void Awake()
    {
        collider = GetComponent<BoxCollider>();
        owner = Team.Defense;
        captureHealth = maxHealth;
        captureRate = 0;
    }
    
    void FixedUpdate()
    {
        captureHealth -= captureRate * Time.deltaTime;

        //if (captureRate > 0)
        //{
        //    Debug.Log(captureHealth);
        //}

        ClampHealth();

        if (captureHealth < 0)
        {
            if (owner == Team.Defense)
            {
                OnCapture();
            } else
            {
                OnReclaim();
            }
        }
    }
}
