using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private TMP_Text coinText;
    [SerializeField] private Slider moraleSlider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (SaveManager.CurrentSave == null && SaveManager.SaveExists())
        {
            SaveManager.LoadGame();
        }

        UpdateCoinDisplay();
        UpdateMoraleDisplay();
        
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

    public int GetUpgradePrice()
    {
        if (SaveManager.CurrentSave == null)
        {
            return -1;
        }

        int morale = SaveManager.CurrentSave.morale;

        if (morale < 10)
        {
            return -1;
        }
        else if (morale < 40)
        {
            return 60;
        }
        else if (morale <= 70)
        {
            return 50;
        }
        else
        {
            return 40;
        }
    }

    public int GetMoraleItemPrice(string itemType)
    {
        if (SaveManager.CurrentSave == null)
        {
            return -1;
        }

        int morale = SaveManager.CurrentSave.morale;

        if (itemType == "Beer")
        {
            if (morale < 10) return 500;
            if (morale < 40) return 400;
            return 300;
        }
        else if (itemType == "Wine")
        {
            if (morale < 10) return 900;
            if (morale < 40) return 800;
            return 650;
        }

        return -1;
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

    public void PurchaseUpgrade(string upgradeType)
    {
        if (SaveManager.CurrentSave == null)
        {
            Debug.LogWarning("No save data loaded.");
            return;
        }

        // Check if this is a valid upgrade
        if (upgradeType != "FoodStorage" &&
            upgradeType != "WeaponStorage")
        {
            Debug.LogWarning("Invalid upgrade: " + upgradeType);
            return;
        }

        int price = GetUpgradePrice();

        if(price < 0)
        {
            Debug.Log("This upgrade is unavailable.");
            return;
        }

        if (!SaveManager.SpendMoney(price))
        {
            Debug.Log("Not enough coins!");
            return;
        }

        // apply purchased upgrade to save data
        if (upgradeType == "FoodStorage")
        {
            SaveManager.CurrentSave.foodStorageLevel++;
        }
        else
        {
            SaveManager.CurrentSave.weaponStorageLevel++;
        }

        SaveManager.SaveGame();
        UpdateCoinDisplay();

        Debug.Log("Purchased " + upgradeType + " for " + price + " coins!");

    }

    public void PurchaseMoraleItem(string itemType)
    {
        int price = GetMoraleItemPrice(itemType);

        if (price < 0)
        {
            Debug.LogWarning("Invalid morale item");
            return;
        }

        if (!SaveManager.SpendMoney(price))
        {
            Debug.Log("Not enough coins!");
            return;
        }

        if (itemType == "Beer")
        {
            SaveManager.ChangeMorale(20);
        }
        else if (itemType == "Wine")
        {
            SaveManager.ChangeMorale(50);
        }

        UpdateCoinDisplay();

        Debug.Log("Purchased " + itemType + " for " + price + " coins!");
        UpdateMoraleDisplay();
    }

    private void UpdateCoinDisplay()
    {
        if (coinText == null || SaveManager.CurrentSave == null)
        {
            return;
        }

        coinText.text = "COINS: " + SaveManager.CurrentSave.money;
    }

    private void UpdateMoraleDisplay()
    {
        if (moraleSlider == null || SaveManager.CurrentSave == null)
        {
            return;
        }

        moraleSlider.minValue = 0;
        moraleSlider.maxValue = 100;
        moraleSlider.value = SaveManager.CurrentSave.morale;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
