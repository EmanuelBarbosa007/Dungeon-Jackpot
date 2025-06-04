using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static string previousScene;

    public static void GoToShop()
    {
        previousScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("Loja");
    }

    public static void ReturnToPreviousScene()
    {
        if (!string.IsNullOrEmpty(previousScene))
        {
            SceneManager.LoadScene(previousScene);
        }
        else
        {
            Debug.LogWarning("Nenhuma cena anterior foi guardada.");
        }
    }
}
