using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;
using System.Drawing;

public class TurnBasedCombat : MonoBehaviour
{
    public Character player;
    public Character enemy;
    private bool isPlayerTurn = true;

    [SerializeField] private GameObject unlock;
    [SerializeField] private GameObject lost;
    [SerializeField] private GameObject battle;
    [SerializeField] private TextMeshProUGUI battleLog;
    [SerializeField] private GameObject explosionEffect;
    [SerializeField] private CombatUI combatUI;

    public DiceAnimator diceAnimator;

    private string playerAction = "";

    private Animator playerAnim;
    private Animator enemyAnim;

    private bool playerCharged = false;
    private bool enemyCharged = false;



    private void Start()
    {
        if (UpgradeManager.instance.HasUpgrade("battleDamageBoost"))
        {
            player.damageMultiplier = 1.5f;
        }

        if (UpgradeManager.instance.HasUpgrade("battleDefenseBoost"))
        {
            player.defenseMultiplier = 1.5f;
        }

        player.Health = player.MaxHealth;
        enemy.Health = enemy.MaxHealth;

        player.TryGetComponent(out playerAnim);
        enemy.TryGetComponent(out enemyAnim);

        StartCoroutine(CombatLoop());
    }

    private IEnumerator CombatLoop()
    {
        while (player.IsAlive && enemy.IsAlive)
        {
            if (isPlayerTurn)
            {
                yield return PlayerTurn();
                enemy.IsDefending = false;
            }
            else
            {
                yield return EnemyTurn();
                player.IsDefending = false;
            }

            isPlayerTurn = !isPlayerTurn;
            yield return new WaitForSeconds(1f);
        }

        if (player.IsAlive)
        {
            LogMessage("Player Wins!");
            HandleDefeat(enemy);
            yield return new WaitForSeconds(2f);
            GameCurrency.instance.AddCoins(50);
            unlock.SetActive(true);
        }
        else
        {
            LogMessage("Enemy Wins!");
            HandleDefeat(player);
            yield return new WaitForSeconds(2f);
            lost.SetActive(true);
        }

        battle.SetActive(false);
    }

    public void SetPlayerAction(string action)
    {
        playerAction = action;
    }

    private IEnumerator PlayerTurn()
    {
        LogMessage("Waiting for player action...");
        combatUI.SetButtonsInteractable(true);

        while (playerAction == "")
            yield return null;

        string action = playerAction;
        playerAction = "";
        combatUI.SetButtonsInteractable(false);

        int roll = Random.Range(1, 21);
        yield return StartCoroutine(diceAnimator.RollDice(roll));
        yield return new WaitForSeconds(1f);

        if (action == "charge")
        {
            if (roll <= 3)
            {
                playerCharged = false;
                LogMessage("Charge Failed!");
            }
            else
            {
                playerCharged = true;
                LogMessage("Player is charging for next turn!");
            }
        }

        else if (action == "attack")
        {
            TriggerAnimation(playerAnim, "Attack");

            if (roll <= 3)
            {
                LogMessage("Attack Failed!");
            }
            else
            {
                int damage;

                if (roll < 17)
                {
                    float defensePercent = Mathf.Clamp(enemy.Defense, 0, 100) / 100f;
                    damage = Mathf.CeilToInt(player.AttackPower * (1f - defensePercent));

                    if (enemy.IsDefending)
                    {
                        damage = Mathf.FloorToInt(damage * 0.7f);
                        LogMessage($"Enemy defended! Reduced damage: {damage}");
                    }
                    else
                    {
                        yield return new WaitForSeconds(1f);
                        TriggerAnimation(enemyAnim, "TakeHit");
                    }
                }
                else if (roll < 20)
                {
                    damage = player.AttackPower;

                    if (!enemy.IsDefending)
                    {
                        yield return new WaitForSeconds(1f);
                        TriggerAnimation(enemyAnim, "TakeHit");
                    }

                    LogMessage($"Critical hit! Damage: {damage}");
                }
                else // roll == 20
                {
                    damage = player.AttackPower * 2; 
                    LogMessage("GOT 20! Super Critical! Ignoring defense!");

                    if (playerCharged)
                    {
                        damage *= 2;
                        LogMessage("CHARGED Super Critical! Total Damage x4!");
                        playerCharged = false;
                    }

                    yield return new WaitForSeconds(1f);
                    TriggerAnimation(enemyAnim, "TakeHit");


                    enemy.TakeDamage(damage);
                    LogMessage($"Enemy took {damage} damage. Health: {enemy.Health}");
                    yield break;
                }

                
                if (playerCharged)
                {
                    damage *= 2;
                    LogMessage("Charged Attack! Damage Doubled!");
                    playerCharged = false;
                }

                enemy.TakeDamage(damage);
                LogMessage($"Enemy took {damage} damage. Health: {enemy.Health}");
            }
        }

        else if (action == "defend")
        {
            if (roll <= 3)
            {
                player.IsDefending = false;
                LogMessage("Defense Failed!");
            }
            else if (roll < 17)
            {
                player.IsDefending = true;
                float healPercent = 0.10f;
                if (playerCharged)
                {
                    healPercent *= 2f;
                    LogMessage("Charged Defense! Effects Doubled!");
                    playerCharged = false;
                }

                int healAmount = Mathf.CeilToInt(player.MaxHealth * healPercent);
                player.Health = Mathf.Min(player.Health + healAmount, player.MaxHealth);
                LogMessage($"Medium Defense! +{healAmount} HP");
            }
            else if (roll < 20)
            {
                player.IsDefending = true;
                float healPercent = 0.25f;
                if (playerCharged)
                {
                    healPercent *= 2f;
                    LogMessage("Charged Defense! Effects Doubled!");
                    playerCharged = false;
                }

                int healAmount = Mathf.CeilToInt(player.MaxHealth * healPercent);
                player.Health = Mathf.Min(player.Health + healAmount, player.MaxHealth);
                LogMessage($"Maximum Defense! +{healAmount} HP");
            }
            else // roll == 20
            {
                player.IsDefending = true;
                float healPercent = 0.50f;
                if (playerCharged)
                {
                    healPercent *= 2f;
                    playerCharged = false;
                }

                int healAmount = Mathf.CeilToInt(player.MaxHealth * healPercent);
                player.Health = Mathf.Min(player.Health + healAmount, player.MaxHealth);
                LogMessage($"GOT 20! Full Defense and healed {healAmount} HP");
            }

            if (player.healthBar != null)
                player.healthBar.UpdateHealthBar();
        }

        yield return new WaitForSeconds(1.5f);

        enemy.IsDefending = false;
        UpdateAnimations();
    }



    private IEnumerator EnemyTurn()
    {
        LogMessage("Enemy's Turn...");
        yield return new WaitForSeconds(1f);

        int roll = Random.Range(1, 21);
        yield return StartCoroutine(diceAnimator.RollDice(roll));
        yield return new WaitForSeconds(1f);

        float actionChoice = Random.value;

        if (actionChoice < 0.2f) // Enemy tries to charge
        {
            if (roll <= 3)
            {
                enemyCharged = false;
                LogMessage("Enemy tried to charge but failed!");
            }
            else
            {
                enemyCharged = true;
                LogMessage("Enemy is charging for next turn!");
            }
        }

        else if (actionChoice < 0.5f) // Enemy defends
        {
            if (roll <= 3)
            {
                enemy.IsDefending = false;
                LogMessage("Enemy tried to defend but failed!");
            }
            else if (roll < 17)
            {
                enemy.IsDefending = true;
                float healPercent = 0.10f;
                if (enemyCharged)
                {
                    healPercent *= 2f;
                    LogMessage("Enemy used Charged Defense! Effects Doubled!");
                    enemyCharged = false;
                }

                int healAmount = Mathf.CeilToInt(enemy.MaxHealth * healPercent);
                enemy.Health = Mathf.Min(enemy.Health + healAmount, enemy.MaxHealth);
                LogMessage($"Enemy defends moderately! +{healAmount} HP");
            }
            else if (roll < 20)
            {
                enemy.IsDefending = true;
                float healPercent = 0.25f;
                if (enemyCharged)
                {
                    healPercent *= 2f;
                    LogMessage("Enemy used Charged Defense! Effects Doubled!");
                    enemyCharged = false;
                }

                int healAmount = Mathf.CeilToInt(enemy.MaxHealth * healPercent);
                enemy.Health = Mathf.Min(enemy.Health + healAmount, enemy.MaxHealth);
                LogMessage($"Enemy defends perfectly! +{healAmount} HP");
            }
            else // roll == 20
            {
                enemy.IsDefending = true;
                float healPercent = 0.50f;
                if (enemyCharged)
                {
                    healPercent *= 2f;
                    enemyCharged = false;
                }

                int healAmount = Mathf.CeilToInt(enemy.MaxHealth * healPercent);
                enemy.Health = Mathf.Min(enemy.Health + healAmount, enemy.MaxHealth);
                LogMessage($"Enemy 20! Full Defense and healed {healAmount} HP");
            }

            if (enemy.healthBar != null)
                enemy.healthBar.UpdateHealthBar();
        }

        else // Enemy attacks
        {
            TriggerAnimation(enemyAnim, "Attack");

            if (roll <= 3)
            {
                LogMessage("Enemy attack failed!");
            }
            else
            {
                int damage;

                if (roll < 17)
                {
                    float defensePercent = Mathf.Clamp(player.Defense, 0, 100) / 100f;
                    damage = Mathf.CeilToInt(enemy.AttackPower * (1f - defensePercent));

                    if (player.IsDefending)
                    {
                        damage = Mathf.FloorToInt(damage * 0.7f);
                        LogMessage($"Player defended! Reduced damage: {damage}");
                    }
                    else
                    {
                        yield return new WaitForSeconds(1f);
                        TriggerAnimation(playerAnim, "TakeHit");
                    }
                }
                else if (roll < 20)
                {
                    damage = enemy.AttackPower;
                    LogMessage("Enemy lands critical hit!");

                    if (!player.IsDefending)
                    {
                        yield return new WaitForSeconds(1f);
                        TriggerAnimation(playerAnim, "TakeHit");
                    }
                }
                else // roll == 20
                {
                    damage = enemy.AttackPower * 2;
                    LogMessage("ENEMY GOT 20! Super Critical! Ignoring defense!");

                    if (enemyCharged)
                    {
                        damage *= 2;
                        enemyCharged = false;
                    }

                    yield return new WaitForSeconds(1f);
                    TriggerAnimation(playerAnim, "TakeHit");

                    player.TakeDamage(damage);
                    LogMessage($"Player took {damage} damage. Health: {player.Health}");
                    yield break;
                }

                if (enemyCharged)
                {
                    damage *= 2;
                    enemyCharged = false;
                }

                player.TakeDamage(damage);
                LogMessage($"Player took {damage} damage. Health: {player.Health}");
            }
        }

        yield return new WaitForSeconds(1.5f);

        player.IsDefending = false;
        UpdateAnimations();
    }




    private void LogMessage(string message)
    {
        if (battleLog != null)
        {
            battleLog.text = message;
        }
    }

    private void HandleDefeat(Character character)
    {
        if (explosionEffect != null)
        {
            Vector3 position = character.transform.position;
            GameObject explosion = Instantiate(explosionEffect, position, Quaternion.identity);
            explosion.transform.localScale = Vector3.one * 6f;
        }

        Destroy(character.gameObject);
    }

    private void UpdateAnimations()
    {
        if (playerAnim != null)
            playerAnim.SetBool("Block", player.IsDefending);

        if (enemyAnim != null)
            enemyAnim.SetBool("Block", enemy.IsDefending);
    }

    private void TriggerAnimation(Animator anim, string trigger)
    {
        if (anim != null)
            anim.SetTrigger(trigger);
    }
}
