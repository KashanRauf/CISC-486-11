using UnityEngine;

public class NPCComponent : MonoBehaviour
{
    
    public Squad GetSquad()
    {
        if (this.GetType() == typeof(Squad))
        {
            return (Squad) this;
        }

        return GetComponentInParent<Squad>();
    }


    public TeamManager GetManager()
    {
        return GetComponentInParent<TeamManager>();
    }
}
