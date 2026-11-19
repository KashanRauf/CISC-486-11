using UnityEngine;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private TMP_Text coinText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (SaveManager.CurrentSave == null && SaveManager.SaveExists())
        {
            SaveManager.LoadGame();
        }

        UpdateCoinDisplay();
        
    }

    public int GetCrewPrice(string crewType)
    {
        if (SaveManager.CurrentSave == null)
        {
            return -1;
        }

        int morale = SaveManager.CurrentSave.morale;

            if (morale < 10)
            {
                if (crewType == "Medic")
                {
                    return -1;
                }

                return 40;
            }
            else if (morale < 40)
            {
                return 30;
            }
            else if (morale <= 70)
            {
                return 25;
            }
            else
            {
                return 15;
            }
    }

    public void PurchaseCrew(string crewType)
    {
        if(SaveManager.CurrentSave == null)
        {
            Debug.LogWarning("No save data loaded.");
            return;
        }

        // Check that the requiested crew type exists
        if (crewType != "Melee" &&
                crewType != "Ranged" &&
                crewType != "Medic" &&
                crewType != "Repair")
            {
                Debug.LogWarning("Invalid crew type: " + crewType);
                return;
            }

        int price = GetCrewPrice(crewType);

        // A negative price means the item is unavailable
        if (price < 0)
        {
            Debug.Log("This crew member is unavailable");
            return;
        }

        // SpendMoney returns false if the player cannot afford the purchase
        if (!SaveManager.SpendMoney(price))
        {
            Debug.Log("Not enough coins!");
            return;
        }

        SaveManager.AddCrew(crewType);

        Debug.Log("Purchased " + crewType + " for " + price + " coins!");
        UpdateCoinDisplay();
    }

    private void UpdateCoinDisplay()
    {
        if (coinText == null || SaveManager.CurrentSave == null)
        {
            return;
        }

        coinText.text = "COINS: " + SaveManager.CurrentSave.money;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
