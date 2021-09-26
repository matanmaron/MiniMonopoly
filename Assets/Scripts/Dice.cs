using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Dice : MonoBehaviour
{
    [SerializeField] Image Img = null;
    private Sprite[] sides = null;

    void Start()
    {
        sides = Resources.LoadAll<Sprite>("Dice/");
    }

    internal void RollDice(int finalside)
    {
        StartCoroutine(Roll(finalside));
    }

    IEnumerator Roll(int finalside)
    {
        int randomSide = 0; 
        for (int i = 0; i < 20; i++)
        {
            randomSide = Random.Range(1, 7);
            Img.sprite = sides[randomSide - 1];
            yield return new WaitForSeconds(0.05f);
        }
        Img.sprite = sides[finalside - 1];
        yield return null;
        GameManager.Instance.GameStatus = GameStatusEnum.DiceRolled;
    }
}
