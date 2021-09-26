using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] Message Message = null;
    [SerializeField] GameObject MessageObject = null;
    [SerializeField] GameObject GameOverObject = null;
    [SerializeField] GameObject Cards = null;
    [SerializeField] GameObject ActivePlayer1 = null;
    [SerializeField] GameObject ActivePlayer2 = null;
    [SerializeField] Player Player1Holder = null;
    [SerializeField] Player Player2Holder = null;
    [SerializeField] TextMeshProUGUI DiceAnalyticsText = null;
    [SerializeField] TextMeshProUGUI MoneyChange = null;
    [SerializeField] TextMeshProUGUI Player1Coins = null;
    [SerializeField] TextMeshProUGUI Player2Coins = null;
    [SerializeField] Dice DiceScript = null;
    [SerializeField] Animator DiceAnimator = null;
    private GameObject[] Properties = null;

    private void Start()
    {
        GetAllCards();
        Player1Holder.transform.position = Properties[0].transform.position;
        Player2Holder.transform.position = Properties[0].transform.position;
    }

    internal void MovePlayer(PlayersEnum player, int cardIndex)
    {
        switch (player)
        {
            case PlayersEnum.Player1:
                Player1Holder.MovePlayer(Properties[cardIndex].transform.position);
                break;
            case PlayersEnum.Player2:
                Player2Holder.MovePlayer(Properties[cardIndex].transform.position);
                break;
            default:
                Debug.LogWarning("UIManager - SetPlayer - no such player exists");
                break;
        }
    }

    internal void ShowMoneyChange(PlayersEnum player, int amount)
    {
        TextMeshProUGUI element = null;
        if (player == PlayersEnum.Player1)
        {
            element = Instantiate(MoneyChange, Player1Coins.transform.position, Player1Coins.transform.rotation, Player1Coins.transform);
        }
        else
        {
            element = Instantiate(MoneyChange, Player2Coins.transform.position, Player2Coins.transform.rotation, Player2Coins.transform);
        }
        if (amount >= 0)
        {
            element.color = Color.green;
            element.text = $"+{amount:N0}$";
        }
        else
        {
            element.color = Color.red;
            element.text = $"{amount:N0}$";
        }
    }

    internal void ShowMessage(string msg, string boldMsg)
    {
        MessageObject.SetActive(true);
        Message.SetMessage(msg, boldMsg);
    }

    internal void ShowDiceAnalytics(int[] diceAnalytics)
    {
        GameOverObject.SetActive(true);
        StringBuilder sb = new StringBuilder();
        int total = 0;
        for (int i = 0; i < diceAnalytics.Length; i++)
        {
            total += diceAnalytics[i];
        }
        if (total == 0)
        {
            total = 1;
        }
        for (int i = 0; i < diceAnalytics.Length-1; i++)
        {
            sb.Append($"{diceAnalytics[i] * 100 / total:00}% ");
        }
        sb.Append($"{diceAnalytics[diceAnalytics.Length - 1] * 100 / total:00}% ");
        DiceAnalyticsText.text = sb.ToString();
    }

    internal void ChangePlayer(PlayersEnum turn)
    {
        if (turn == PlayersEnum.Player1)
        {
            ActivePlayer1.SetActive(true);
            ActivePlayer2.SetActive(false);
        }
        else
        {
            ActivePlayer1.SetActive(false);
            ActivePlayer2.SetActive(true);
        }
    }

    internal void RollDice(int finalSide)
    {
        DiceScript.RollDice(finalSide);
    }

    internal void GlowDice()
    {
        DiceAnimator.Play("GlowButton");
    }

    internal void StopGlowDice()
    {
        DiceAnimator.Play("Idle");
    }

    public void OnClickOkButton() //from popoup msg
    { 
        if (GameManager.Instance.GameStatus == GameStatusEnum.GameOver)
        {
            GameManager.Instance.GameStatus = GameStatusEnum.PlayAgain;
            return;
        }
        GameManager.Instance.GameStatus = GameStatusEnum.BuyPayDone;
    }

    public void ClickOnDice() //from dice button
    {
        StopGlowDice();
        if (GameManager.Instance.GameStatus == GameStatusEnum.None)
        {
            GameManager.Instance.GameStatus = GameStatusEnum.DiceClicked;
        }
    }

    internal void UpdateCoins(int player1Coins, int player2Coins)
    {
        Player1Coins.text = $"{player1Coins:N0}$";
        Player2Coins.text = $"{player2Coins:N0}$";
    }

    internal void UpdateCardOwner(int index, PropertyEnum owner)
    {
        Properties[index].GetComponent<Card>().SetCard(owner);
    }

    internal void SetAllCards(PropertyEnum[] properties)
    {
        if (Properties.Length != properties.Length)
        {
            Debug.LogWarning("UIManager - SetAllCards - not all cards are set");
            return;
        }
        for (int i = 0; i < Properties.Length; i++)
        {
            Properties[i].GetComponent<Card>().SetCard(properties[i]);
        }
    }

    private void GetAllCards()
    {
        Transform[] childrens = Cards.GetComponentsInChildren<Transform>();
        List<GameObject> go = new List<GameObject>();
        foreach (var c in childrens)
        {
            if (c.tag == "Property")
            {
                go.Add(c.gameObject);
            }
        }
        go = go.OrderBy(s => s.name).ToList();
        Properties = go.ToArray();
    }
}