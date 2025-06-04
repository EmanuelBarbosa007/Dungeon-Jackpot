using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeShop : MonoBehaviour
{
    [System.Serializable]
    public class UpgradeItem
    {
        public string upgradeID;
        public int price;
        public Button buyButton;
        public TMP_Text priceText;
        public GameObject upgradeImage;
    }

    public List<UpgradeItem> upgrades;
    public TMP_Text saldoText;

    void Start()
    {
        UpdateUI();

        foreach (var item in upgrades)
        {
            string id = item.upgradeID;
            item.priceText.text = item.price + " Coins";

            if (UpgradeManager.instance.HasUpgrade(id))
            {
                item.buyButton.interactable = false;
                item.priceText.text = "Purchased!";
            }
            else
            {
                item.buyButton.onClick.AddListener(() => BuyUpgrade(item));
            }
        }
    }

    void BuyUpgrade(UpgradeItem item)
    {
        if (UpgradeManager.instance.HasUpgrade(item.upgradeID))
            return;

        if (GameCurrency.instance.SpendCoins(item.price))
        {
            UpgradeManager.instance.UnlockUpgrade(item.upgradeID);
            item.buyButton.interactable = false;
            item.priceText.text = "Purchased!";
            UpdateUI();
        }
        else
        {
            item.priceText.text = "Insufficient balance!";
        }
    }

    void UpdateUI()
    {
        saldoText.text = "Balance: " + GameCurrency.instance.currentCoins;
    }
}
