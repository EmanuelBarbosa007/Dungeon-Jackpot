using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string novolevel;
    [SerializeField] private GameObject painelMenuPrincipal;
    [SerializeField] private GameObject painelOptions;
    [SerializeField] private GameObject painelCreditos;

    public void Start()
    {
        GameObject btnContinue = GameObject.Find("BtnContinuar");
        if (btnContinue != null)
            btnContinue.SetActive(SaveSystem.SaveExists());
    }

    public void StartGame()
    {
        SaveSystem.DeleteSave();

        if (UpgradeManager.instance != null)
        {
            UpgradeManager.instance.ResetUpgrades();
        }

        if (GameCurrency.instance != null)
        {
            GameCurrency.instance.currentCoins = 1000;
        }

        SceneManager.LoadScene(novolevel);
    }


    public void LoadGame()
    {
        GameData data = SaveSystem.LoadGame();
        if (data != null)
        {
            SceneManager.LoadScene(data.currentLevel);
        }
    }


    public void abriropcoes()
    {
        painelMenuPrincipal.SetActive(false);
        painelCreditos.SetActive(false);
        painelOptions.SetActive(true);

    }

    public void fecharopcoes()
    {
        painelOptions.SetActive(false);
        painelCreditos.SetActive(false);
        painelMenuPrincipal.SetActive(true);
    }

    public void abrircreditos()
    {
        painelOptions.SetActive(false);
        painelMenuPrincipal.SetActive(false);
        painelCreditos.SetActive(true);
    }

    public void fecharcreditos()
    {
        painelOptions.SetActive(false);
        painelCreditos.SetActive(false);
        painelMenuPrincipal.SetActive(true);
    }

    public void sair()
    {
        Debug.Log("End Game");
        Application.Quit();
    }

    public void mudarscreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }
}
