using UnityEngine;
using UnityEngine.UI;

public class CombatUI : MonoBehaviour
{
    public TurnBasedCombat combat;
    public Button[] actionButtons;

    public void OnAttackButton()
    {
        combat.SetPlayerAction("attack");
    }

    public void OnDefendButton()
    {
        combat.SetPlayerAction("defend");
    }

    public void OnChargeButton()
    {
        combat.SetPlayerAction("charge");
    }


    public void SetButtonsInteractable(bool state)
    {
        foreach (var btn in actionButtons)
        {
            btn.interactable = state;
        }
    }
}
