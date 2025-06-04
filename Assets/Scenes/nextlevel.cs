using UnityEngine;
using UnityEngine.SceneManagement;

public class next : MonoBehaviour
{
    [SerializeField] private string nomegame;

    public void nextlevel()
    {
        SceneManager.LoadScene(nomegame);
    }
}