using UnityEngine;
using UnityEngine.UI;

public class InfoPanelController : MonoBehaviour
{
    public GameObject infoPanel;
    public GameObject[] pages;
    public Button leftButton;
    public Button rightButton;
    public Button infoToggleButton;

    public CharacterInfoPanel characterInfoPanel; 

    private int currentPage = 0;
    private bool isPanelOpen = false;

    void Start()
    {
        infoPanel.SetActive(false);
        UpdatePages();

        if (leftButton != null)
            leftButton.onClick.AddListener(PreviousPage);

        if (rightButton != null)
            rightButton.onClick.AddListener(NextPage);

        if (infoToggleButton != null)
            infoToggleButton.onClick.AddListener(TogglePanel);
    }


    void TogglePanel()
    {
        isPanelOpen = !isPanelOpen;
        infoPanel.SetActive(isPanelOpen);
        if (isPanelOpen)
        {
            UpdatePages();

            if (characterInfoPanel != null)
                characterInfoPanel.UpdateCharacterInfo();
        }
    }

    void PreviousPage()
    {
        currentPage = (currentPage - 1 + pages.Length) % pages.Length;
        UpdatePages();
    }

    void NextPage()
    {
        currentPage = (currentPage + 1) % pages.Length;
        UpdatePages();
    }

    void UpdatePages()
    {
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(i == currentPage);
        }
    }
}
