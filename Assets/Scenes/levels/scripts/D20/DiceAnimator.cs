using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DiceAnimator : MonoBehaviour
{
    public Image diceImage;
    public Sprite[] rollSprites;
    public Sprite[] numberSprites;

    public float rollSpeed = 0.1f;
    public int rollCycles = 10;

    public IEnumerator RollDice(int result)
    {
        
        for (int i = 0; i < rollCycles; i++)
        {
            diceImage.sprite = rollSprites[Random.Range(0, rollSprites.Length)];
            yield return new WaitForSeconds(rollSpeed);
        }

        // Mostra o resultado
        diceImage.sprite = numberSprites[result - 1];
    }
}

