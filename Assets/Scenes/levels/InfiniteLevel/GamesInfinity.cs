using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamesInfinity : MonoBehaviour
{
    public Button wheelButton;
    public Button slotButton;
    public Button hiloButton;

    void Start()
    {
        wheelButton.onClick.AddListener(() => LoadScene("roda_da_sorte_INF"));
        slotButton.onClick.AddListener(() => LoadScene("slot_machine_INF"));
        hiloButton.onClick.AddListener(() => LoadScene("HI-LO"));
    }

    void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
