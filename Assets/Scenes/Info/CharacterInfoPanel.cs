using UnityEngine;
using TMPro;

public class CharacterInfoPanel : MonoBehaviour
{
    public Character character;

    public TextMeshProUGUI healthText;
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI defenseText;

    public void UpdateCharacterInfo()
    {
        if (character == null) return;

        healthText.text = $"Health: {character.Health}/{character.MaxHealth}";
        attackText.text = $"Attack: {character.AttackPower}";
        defenseText.text = $"Defense: {character.Defense}";
    }

}
