using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SquadSelect : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] Image[] members = new Image[1];
    public Squad squad;
    public CommandMenu parent;

    public void SetTitle(SquadBehavior s, int num)
    {
        // Title includes the squad's ID and current status
        string status;
        switch (s.CurrentState()) {
            case SquadBehavior.SquadStates.Battle:
                status = "Fighting an enemy squad";
                break;
            case SquadBehavior.SquadStates.BattleEngage:
                status = "Picking a fight";
                break;
            case SquadBehavior.SquadStates.ToObjective:
                status = $"Going to the {s.locationName}";
                break;
            case SquadBehavior.SquadStates.Holding:
                status = $"Defending the {s.locationName}";
                break;
            default:
                status = "Unknown";
                break;
        }

        title.text = $"Squad {num+1}: {status}";
    }

    public void SetChibis(List<NPC> npcs)
    {
        //Debug.Log($"Setting chibis: {npcs.Count}");
        members = new Image[npcs.Count];
        for (int i = 0; i < members.Length; i++)
        {
            members[i] = Instantiate(Prefabs.prefabs.memberChibi, this.transform, false).GetComponent<Image>();
            members[i].transform.position += new Vector3(65*i, 0, 0);
            switch (npcs[i].primary)
            {
                case NPC.CombatClass.Medic:
                    members[i].sprite = CommandMenu.GetChibis[0];
                    break;
                case NPC.CombatClass.Melee:
                    members[i].sprite = CommandMenu.GetChibis[1];
                    break;
                case NPC.CombatClass.Ranged:
                    members[i].sprite = CommandMenu.GetChibis[2];
                    break;
                case NPC.CombatClass.Repair:
                    members[i].sprite = CommandMenu.GetChibis[3];
                    break;
            }
        }
    }

    public void SetParent(CommandMenu c)
    {
        parent = c;
    }

    public void ViewSquad()
    {
        Debug.Log(parent);
        Debug.Log(squad);
        parent.SelectSquad(squad);
    }
}
