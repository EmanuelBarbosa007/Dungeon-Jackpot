using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameCurrency : MonoBehaviour
{
    public static GameCurrency instance;
    public int currentCoins = 1000;
    public TMP_Text currencyText;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        LoadGame();
    }

    public void AddCoins(int amount)
    {
        currentCoins += amount;
        UpdateUI();
    }

    public bool SpendCoins(int amount)
    {
        if (currentCoins >= amount)
        {
            currentCoins -= amount;
            UpdateUI();
            return true;
        }
        return false;
    }

    private void UpdateUI()
    {
        if (currencyText != null)
        {
            currencyText.text = "Moedas: " + currentCoins;
        }
    }

    public void SaveGame()
    {
        int currentLevel = SceneManager.GetActiveScene().buildIndex;
        string sceneName = SceneManager.GetActiveScene().name;


        if (sceneName == "Bootstrap")
        {
            return;
        }

        SaveSystem.SaveGame(currentLevel, currentCoins);
    }

    private void LoadGame()
    {
        GameData data = SaveSystem.LoadGame();
        if (data != null)
        {
            currentCoins = data.currentCoins;


            if (data.currentLevel != 0)
            {
                SceneManager.LoadScene(data.currentLevel);
            }
            else
            {
                SceneManager.LoadScene("MainMenu");
            }
        }
        else
        {
            currentCoins = 1000;
            SceneManager.LoadScene("MainMenu");
        }

        UpdateUI();
        SaveGame(); 
    }
}
