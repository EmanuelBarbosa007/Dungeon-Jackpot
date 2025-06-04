using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
    [SerializeField] private string scene;

    public void restart()
    {
        SceneManager.LoadScene(scene);
    }
}
