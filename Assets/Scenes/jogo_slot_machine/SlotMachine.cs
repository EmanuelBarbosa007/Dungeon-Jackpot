using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotMachine : MonoBehaviour
{
    public Image[] reels;
    public Sprite[] symbols;
    public Button spinButton;
    public TMP_InputField betInput;
    public TMP_Text balanceText;
    public TMP_Text resultText;

    private int[] finalSymbols;
    private bool isSpinning = false;
    private int betAmount;

    private void Start()
    {
        spinButton.onClick.AddListener(SpinReels);
        UpdateBalanceUI();
    }

    void SpinReels()
    {
        if (isSpinning) return;

        if (!int.TryParse(betInput.text, out betAmount) || betAmount <= 0)
        {
            resultText.text = "Invalid Bet!";
            return;
        }

        if (GameCurrency.instance.SpendCoins(betAmount))
        {
            UpdateBalanceUI();
        }
        else
        {
            resultText.text = "Insufficient Balance!";
            return;
        }

        isSpinning = true;
        resultText.text = "Spinning...";
        finalSymbols = new int[reels.Length];

        StartCoroutine(SpinReel(0, 4f));
        StartCoroutine(SpinReel(1, 6f));
        StartCoroutine(SpinReel(2, 10f));
    }

    IEnumerator SpinReel(int reelIndex, float spinTime)
    {
        float elapsedTime = 0f;
        float speed = 0.05f;

        while (elapsedTime < spinTime)
        {
            reels[reelIndex].sprite = symbols[Random.Range(0, symbols.Length)];
            yield return new WaitForSeconds(speed);
            elapsedTime += speed;
            speed *= 1.1f;
        }

        int finalSymbolIndex = Random.Range(0, symbols.Length);
        reels[reelIndex].sprite = symbols[finalSymbolIndex];
        finalSymbols[reelIndex] = finalSymbolIndex;

        if (reelIndex == reels.Length - 1)
        {
            CalculateResult();
            isSpinning = false;
        }
    }

    void CalculateResult()
    {
        int s1 = finalSymbols[0];
        int s2 = finalSymbols[1];
        int s3 = finalSymbols[2];

        bool jackpot = (s1 == s2 && s2 == s3);
        bool twoMatch = (s1 == s2 || s1 == s3 || s2 == s3);

        if (jackpot)
        {
            float jackpotMultiplier = 2.5f;

            if (UpgradeManager.instance != null && UpgradeManager.instance.HasUpgrade("slotJackpotGeneroso"))
                jackpotMultiplier = 5f;

            int winnings = Mathf.RoundToInt(betAmount * jackpotMultiplier);
            GameCurrency.instance.AddCoins(winnings);
            resultText.text = "Jackpot! Won " + winnings + " coins!";
        }
        else if (twoMatch)
        {

            float comboMultiplier = 1.5f;

            if (UpgradeManager.instance != null && UpgradeManager.instance.HasUpgrade("slotMaisCombos"))
                comboMultiplier = 2f;

            int winnings = Mathf.RoundToInt(betAmount * comboMultiplier);
            GameCurrency.instance.AddCoins(winnings);
            resultText.text = "Won " + winnings + " coins!";
        }
        else
        {
            resultText.text = "You Lost! Try Again!";
        }

        UpdateBalanceUI();
    }

    void UpdateBalanceUI()
    {
        balanceText.text = "Balance: " + GameCurrency.instance.currentCoins;
    }
}
