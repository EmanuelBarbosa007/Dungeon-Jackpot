using UnityEngine;
using UnityEngine.UI;

public class Song : MonoBehaviour
{
    private bool estadosom;
    [SerializeField] private AudioSource fundomusical;

    [SerializeField] private Sprite somligadosprite;
    [SerializeField] private Sprite somdesligadosprite;
    [SerializeField] private Image muteimage;

    private void Start()
    {
        estadosom = PlayerPrefs.GetInt("SomAtivo", 1) == 1;
        float volumeSalvo = PlayerPrefs.GetFloat("VolumeMusica", 1f);

        fundomusical.volume = volumeSalvo;
        fundomusical.enabled = estadosom;

        AtualizarIconeMute();
    }

    public void Ligardesligarsom()
    {
        estadosom = !estadosom;
        fundomusical.enabled = estadosom;

        PlayerPrefs.SetInt("SomAtivo", estadosom ? 1 : 0);
        PlayerPrefs.Save();

        AtualizarIconeMute();
    }

    public void volumemusical(float value)
    {
        fundomusical.volume = value;

        PlayerPrefs.SetFloat("VolumeMusica", value);
        PlayerPrefs.Save();
    }

    private void AtualizarIconeMute()
    {
        if (muteimage != null)
        {
            muteimage.sprite = estadosom ? somligadosprite : somdesligadosprite;
        }
    }
}
