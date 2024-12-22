using TMPro;
using UnityEngine;

public class BonusShop : MonoBehaviour
{
    public TextMeshProUGUI[] _moneyText;

    private int playerMoney;
    private int destroyAllTargetsCount;
    private int bombImmunityCount;

    private const int BonusCost = 2000;

    void Start()
    {
        // Загружаем данные из PlayerPrefs
        playerMoney = PlayerPrefs.GetInt("PlayerCurrency", 0);
        destroyAllTargetsCount = PlayerPrefs.GetInt("DestroyAllTargetsCount", 0);
        bombImmunityCount = PlayerPrefs.GetInt("BombImmunityCount", 0);

        UpdateUI();
    }

    public void BuyDestroyAllTargets()
    {
        if (playerMoney >= BonusCost)
        {
            playerMoney -= BonusCost;
            destroyAllTargetsCount++;
            SaveData();
            UpdateUI();
        }
    }

    public void BuyBombImmunity()
    {
        if (playerMoney >= BonusCost)
        {
            playerMoney -= BonusCost;
            bombImmunityCount++;
            SaveData();
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        foreach (var moneyText in _moneyText)
        {
            moneyText.text = $"Score: {playerMoney}";
        }
    }

    private void SaveData()
    {
        // Сохраняем данные в PlayerPrefs
        PlayerPrefs.SetInt("PlayerCurrency", playerMoney);
        PlayerPrefs.SetInt("DestroyAllTargetsCount", destroyAllTargetsCount);
        PlayerPrefs.SetInt("BombImmunityCount", bombImmunityCount);
        PlayerPrefs.Save();
    }
}
