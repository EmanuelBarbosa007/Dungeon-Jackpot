using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
{
    public void GoToMainMenu()
    {
        StartCoroutine(ReturnToMainMenu());
    }

    private IEnumerator ReturnToMainMenu()
    {
        yield return new WaitForSeconds(0.5f);

        if (GameCurrency.instance != null)
        {
            GameCurrency.instance.SaveGame();
        }

        SceneManager.LoadScene("MainMenu");
    }


}
