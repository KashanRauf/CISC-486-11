using UnityEngine;

public class Prefabs : MonoBehaviour
{

    public static Prefabs prefabs { get; private set; }

    //public GameObject testEnemy;
    public GameObject ranged;
    public GameObject melee;
    public GameObject medic;
    public GameObject brute;
    public GameObject newSquad;
    public GameObject combat;

    public Prefabs GetPrefabs => prefabs;

    void Awake()
    {
        prefabs = this;
    }

}
