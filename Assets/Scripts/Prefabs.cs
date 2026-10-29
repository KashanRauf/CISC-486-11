using UnityEngine;

public class Prefabs : MonoBehaviour
{

    public static Prefabs prefabs { get; private set; }

    public GameObject testEnemy;
    public GameObject newSquad;

    public Prefabs GetPrefabs => prefabs;

    void Awake()
    {
        prefabs = this;
    }

}
