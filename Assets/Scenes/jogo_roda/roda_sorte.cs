using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Roulette : MonoBehaviour
{
    public float RotatePower;
    public float StopPower;
    public Button spinButton;
    public TMP_Text balanceText;
    public TMP_Text resultText;

    private Rigidbody2D rbody;
    private int inRotate;
    private float t;
    private int baseBetAmount = 40;


    private void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
        spinButton.onClick.AddListener(() => Rotate());
        UpdateBalanceUI();
    }


    private void Update()
    {
        if (Mathf.Abs(rbody.angularVelocity) > 0)
        {
            rbody.angularVelocity = Mathf.MoveTowards(rbody.angularVelocity, 0, StopPower * Time.deltaTime);
        }


        if (Mathf.Abs(rbody.angularVelocity) < 0.1f && inRotate == 1)
        {
            rbody.angularVelocity = 0;
            t += Time.deltaTime;
            if (t >= 0.5f)
            {
                GetReward();
                inRotate = 0;
                t = 0;
            }
        }
    }

    public void Rotate(bool isFree = false)
    {
        if (inRotate == 0)
        {
            int betAmount = baseBetAmount;

            if (UpgradeManager.instance != null && UpgradeManager.instance.HasUpgrade("rodaApostaReduzida"))
                betAmount = 25;

            if (!isFree && !GameCurrency.instance.SpendCoins(betAmount))
            {
                resultText.text = "Insufficient Balance!";
                return;
            }

            UpdateBalanceUI();
            RotatePower = Random.Range(1000f, 1500f);
            StopPower = Random.Range(250f, 500f);

            rbody.AddTorque(-RotatePower);
            inRotate = 1;
            resultText.text = isFree ? "Free extra spin!" : "Spinning...";
        }
    }

    public void GetReward()
    {
        float rot = transform.eulerAngles.z;
        int segment = Mathf.FloorToInt((rot + 11.25f) / 22.5f) % 16;

        int[] premios = { 90, 25, 70, 5, 100, 40, 60, 45, 80, 10, 50, 30, 75, 0, 55, 20 };
        float finalAngle = segment * 22.5f;
        transform.eulerAngles = new Vector3(0, 0, finalAngle);

        int prize = premios[segment];

        // Verificar upgrade Sempre Ganha
        if (prize == 0 && UpgradeManager.instance != null && UpgradeManager.instance.HasUpgrade("rodaSempreGanha"))
        {
            resultText.text = "No prize! FREE SPIN...";
            StartCoroutine(Reroll());
            return;
        }

        Win(prize);
    }

    IEnumerator Reroll()
    {
        yield return new WaitForSeconds(1f);
        Rotate(isFree: true);
    }

    public void Win(int prize)
    {
        GameCurrency.instance.AddCoins(prize);
        resultText.text = "Prize: " + prize + " coins!";
        UpdateBalanceUI();
    }

    void UpdateBalanceUI()
    {
        balanceText.text = "Balance: " + GameCurrency.instance.currentCoins;
    }

}
