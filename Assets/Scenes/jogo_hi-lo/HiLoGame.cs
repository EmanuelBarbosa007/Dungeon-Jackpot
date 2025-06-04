using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HiLoGame : MonoBehaviour
{
    public Image cardImage;
    public Button dealButton, lowerButton, higherButton, cashOutButton;
    public TMP_Text resultText, balanceText, multiplierText;
    public TMP_InputField betInput;

    private List<int> deck = new List<int> { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 };
    private int currentCard, nextCard;
    private float multiplier = 0.6f;
    private int betAmount = 0;
    private int potentialWinnings = 0;
    private bool gameStarted = false;
    private bool usedSecondChance = false;

    void Start()
    {
        ResetGame();
        dealButton.onClick.AddListener(StartGame);
        lowerButton.onClick.AddListener(() => CheckGuess(false));
        higherButton.onClick.AddListener(() => CheckGuess(true));
        cashOutButton.onClick.AddListener(CashOut);
    }

    void ResetGame()
    {
        ShuffleDeck();
        gameStarted = false;
        usedSecondChance = false;
        multiplier = 0.6f;
        potentialWinnings = 0;
        UpdateUI();
        EnableGameButtons(false);
        resultText.text = "Enter your bet and press 'Start'";
    }

    void StartGame()
    {
        if (!int.TryParse(betInput.text, out betAmount) || betAmount <= 0)
        {
            resultText.text = "Invalid Bet!";
            return;
        }

        if (!GameCurrency.instance.SpendCoins(betAmount))
        {
            resultText.text = "Insufficient Balance!";
            return;
        }

        gameStarted = true;
        currentCard = DrawCard();
        UpdateCardUI(currentCard);

        // Verifica se upgrade está ativo
        float initialMultiplier = 0.6f;
        if (UpgradeManager.instance != null && UpgradeManager.instance.HasUpgrade("hiloMultiplicador"))
            initialMultiplier = 0.8f;

        multiplier = initialMultiplier;
        potentialWinnings = Mathf.RoundToInt(betAmount * multiplier);
        EnableGameButtons(true);
        resultText.text = "Guess: Higher or Lower?";
        UpdateUI();
    }

    void CheckGuess(bool guessedHigher)
    {
        nextCard = DrawCard(currentCard);
        UpdateCardUI(nextCard);

        if ((guessedHigher && nextCard > currentCard) || (!guessedHigher && nextCard < currentCard))
        {
            float incremento = 0.2f;
            if (UpgradeManager.instance != null && UpgradeManager.instance.HasUpgrade("hiloMultiplicador"))
                incremento = 0.3f;

            multiplier += incremento;
            potentialWinnings = Mathf.RoundToInt(betAmount * multiplier);
            resultText.text = "Got it! Multiplier: x" + multiplier.ToString("0.0");
            currentCard = nextCard;
        }
        else
        {
            if (!usedSecondChance && UpgradeManager.instance != null && UpgradeManager.instance.HasUpgrade("hiloSegundaChance"))
            {
                resultText.text = "Wrong, but got a second chance! Try again!";
                usedSecondChance = true;
                return;
            }

            resultText.text = "Wrong! Lost everything!";
            ResetGame();
            return;
        }

        UpdateUI();
    }

    void CashOut()
    {
        if (!gameStarted) return;

        GameCurrency.instance.AddCoins(potentialWinnings);
        ResetGame();
    }

    int DrawCard(int? excludeCard = null)
    {
        int newCard;
        do
        {
            newCard = deck[Random.Range(0, deck.Count)];
        } while (excludeCard.HasValue && newCard == excludeCard.Value);

        return newCard;
    }


    void ShuffleDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            int temp = deck[i];
            int randomIndex = Random.Range(0, deck.Count);
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    void UpdateCardUI(int cardValue)
    {
        string imagePath = "Cartas/Copas" + cardValue;
        Sprite newSprite = Resources.Load<Sprite>(imagePath);

        if (newSprite != null)
        {
            cardImage.sprite = newSprite;
        }
        else
        {
            Debug.LogError("Imagem não encontrada: " + imagePath);
        }
    }

    void EnableGameButtons(bool enable)
    {
        lowerButton.interactable = enable;
        higherButton.interactable = enable;
        cashOutButton.interactable = enable;
    }

    void UpdateUI()
    {
        balanceText.text = "Balance: " + GameCurrency.instance.currentCoins;
        multiplierText.text = "Multiplier: x" + multiplier.ToString("0.0");
    }
}
