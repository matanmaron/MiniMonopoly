using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] internal bool ShowDebug = false;
    [SerializeField] int[] Rewards = null;
    [SerializeField] int[] RewardsWeight = null;
    [SerializeField] UIManager UI = null;
    [SerializeField] AudioManager Audio = null;
    internal GameStatusEnum GameStatus;
    internal PlayersEnum Turn;
    private PropertyEnum[] properties;
    private int player1Coins = 0;
    private int player2Coins = 0;
    private int player1Position = 0;
    private int player2Position = 0;
    private int[] rewardPlaces = { 6, 12, 18 };
    private int diceShows = 0;
    private const int STARTAMOUNT = 2500;
    private const int FINEAMOUNT = 150;
    private const int BUYAMOUNT = 250;
    private const int FIXEDREWARD = 100;
    private int[] diceAnalytics = new int[6];

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Audio.PlayMusic("GameMusic");
        Turn = (PlayersEnum)UnityEngine.Random.Range(0, 2);
        if (ShowDebug) { Debug.Log($"GameManager - Start - its {Turn} turn"); }
        player1Coins = STARTAMOUNT;
        player1Position = 0;
        player2Coins = STARTAMOUNT;
        player2Position = 0;
        diceShows = 0;
        FillInProperties();
        ResetDiceAnalytics();
        UI.ChangePlayer(Turn);
        UI.UpdateCoins(player1Coins, player2Coins);
        UI.GlowDice();
        GameStatus = GameStatusEnum.None;
    }

    private void ResetDiceAnalytics()
    {
        for (int i = 0; i < diceAnalytics.Length; i++)
        {
            diceAnalytics[i] = 0;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CalculateWinnerAfterESC();
            GameStatus = GameStatusEnum.Wait;
            GameOver();
        }
        switch (GameStatus)
        {
            case GameStatusEnum.GameOver:
            case GameStatusEnum.Wait:
            case GameStatusEnum.None:
                return;
            case GameStatusEnum.DiceClicked:
                Audio.PlaySFX("Dice");
                GameStatus = GameStatusEnum.Wait;
                diceShows = GetRollDiceNumber();
                UI.RollDice(diceShows);
                break;
            case GameStatusEnum.DiceRolled:
                GameStatus = GameStatusEnum.Wait;
                Audio.PlaySFX("Move");
                MovePlayer(diceShows);
                UI.MovePlayer(Turn, Turn == PlayersEnum.Player1 ? player1Position : player2Position);
                break;
            case GameStatusEnum.PlayerMoved:
                Audio.Stop("Move");
                GameStatus = GameStatusEnum.Wait;
                LandOnProperty();
                break;
            case GameStatusEnum.BuyPayDone:
                GameStatus = GameStatusEnum.Wait;
                NextTurn();
                break;
            case GameStatusEnum.PlayAgain:
                GameStatus = GameStatusEnum.Wait;
                RestartGame();
                break;
            default:
                Debug.LogWarning("GameManager - Update - no such status exists");
                break;
        }
    }

    private void CalculateWinnerAfterESC()
    {
        int total1 = 0;
        int total2 = 0;
        total1 += player1Coins;
        total2 += player2Coins;
        string res = string.Empty;
        foreach (var item in properties)
        {
            if (item == PropertyEnum.BelongsTo1)
            {
                total1 += BUYAMOUNT;
            }
            else if(item == PropertyEnum.BelongsTo2)
            {
                total2 += BUYAMOUNT;
            }
        }
        if (total1 == total2)
        {
            res = "I'ts A TIE !";
        }
        else if (total1 > total2)
        {
            res = PlayersEnum.Player1.ToString();
        }
        else
        {
            res = PlayersEnum.Player2.ToString();
        }
        UI.ShowMessage($"You Pressed ESC. Winner BY Value - {res}", $"GAME OVER");
    }

    private void NextTurn()
    {
        diceShows = 0;
        UI.GlowDice();
        Turn = (Turn == PlayersEnum.Player1 ? PlayersEnum.Player2 : PlayersEnum.Player1);
        if (ShowDebug) { Debug.Log($"GameManager - NextTurn - its {Turn} turn"); }
        UI.ChangePlayer(Turn);
        GameStatus = GameStatusEnum.None;
    }

    private int GetCircularPosition(int currentPosition, int stepsToMove)
    {
        currentPosition += stepsToMove;
        if (currentPosition >= 24)
        {
            currentPosition -= 24;
        }
        return currentPosition;
    }

    private bool IsRewardPlace(int i)
    {
        foreach (var rp in rewardPlaces)
        {
            if (i == rp)
            {
                return true;
            }
        }
        return false;
    }

    private void GiveReward(int reward)
    {
        if (Turn == PlayersEnum.Player1)
        {
            player1Coins += reward;
        }
        else
        {
            player2Coins += reward;
        }
        Audio.PlaySFX("Reward");
        if (ShowDebug) { Debug.Log($"GameManager - GiveReward - {Turn} got {reward} as a reward"); }
        if (Settings.FastGame)
        {
            GameStatus = GameStatusEnum.BuyPayDone;
        }
        else
        {
            UI.ShowMessage($"{Turn} landed on reward!", $"Win {reward}$");
        }
        UI.ShowMoneyChange(Turn, reward);
        UI.UpdateCoins(player1Coins, player2Coins);
    }

    private int CalculateReward()
    {
        int totalWeight = 0;
        foreach (var itemWeight in RewardsWeight)
        {
            totalWeight += itemWeight;
        }
        int randomNumber = UnityEngine.Random.Range(0, totalWeight);
        for (int i = 0; i < Rewards.Length; i++)
        {
            if (randomNumber <= RewardsWeight[i])
            {
                if (ShowDebug) { Debug.Log($"GameManager - CalculateReward - reward {Rewards[i]} at {RewardsWeight[i]} chance"); }
                return Rewards[i];
            }
            else
            {
                randomNumber -= RewardsWeight[i];
            }
        }
        Debug.LogWarning($"GiveReward - cannot find reword for random {randomNumber}");
        return 0;
    }

    private void LandOnProperty()
    {
        int index = Turn == PlayersEnum.Player1 ? player1Position : player2Position;
        switch (properties[index])
        {
            case PropertyEnum.Free:
                Buy(index);
                break;
            case PropertyEnum.BelongsTo1:
                if (Turn == PlayersEnum.Player2)
                {
                    Pay(index);
                    return;
                }
                GameStatus = GameStatusEnum.BuyPayDone;
                if (ShowDebug) { Debug.Log($"GameManager - LandOnProperty - card {index} belongs to you {Turn}"); }
                break;
            case PropertyEnum.BelongsTo2:
                if (Turn == PlayersEnum.Player1)
                {
                    Pay(index);
                    return;
                }
                GameStatus = GameStatusEnum.BuyPayDone;
                if (ShowDebug) { Debug.Log($"GameManager - LandOnProperty - card {index} belongs to you {Turn}"); }
                break;
            case PropertyEnum.Reward:
                GiveReward(CalculateReward());
                break;
            case PropertyEnum.Start:
                GiveReward(FIXEDREWARD);
                break;
            default:
                break;
        }
    }

    private void Pay(int index)
    {
        var otherPlayer = Turn == PlayersEnum.Player1 ? PlayersEnum.Player2 : PlayersEnum.Player1;
        if (ShowDebug) { Debug.Log($"GameManager - Pay - card {index} belongs to {otherPlayer}"); }
        if (CanAfford(FINEAMOUNT))
        {
            if (Turn == PlayersEnum.Player1)
            {
                player1Coins -= FINEAMOUNT;
                player2Coins += FINEAMOUNT;
            }
            else
            {

                player2Coins -= FINEAMOUNT;
                player1Coins += FINEAMOUNT;
            }
            Audio.PlaySFX("Pay");
            if (Settings.FastGame)
            {
                GameStatus = GameStatusEnum.BuyPayDone;
            }
            else
            {
                UI.ShowMessage($"{Turn} landed on {otherPlayer} property", $"PAY {FINEAMOUNT}$");
            }
            UI.ShowMoneyChange(Turn, -FINEAMOUNT);
            UI.ShowMoneyChange(otherPlayer, FINEAMOUNT);
            UI.UpdateCoins(player1Coins, player2Coins);
            if (ShowDebug) { Debug.Log($"GameManager - Pay - {Turn} payed {FINEAMOUNT} to {otherPlayer} for card {index}"); }
        }
        else
        {
            if (ShowDebug) { Debug.Log($"GameManager - Pay - {Turn} cannot afford to pay card {index}"); }
            GameOver();
            UI.ShowMessage($"{Turn} landed on {otherPlayer} property BUT CAN'T PAY!", $"{FINEAMOUNT}$");
        }
    }

    private void RestartGame()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        SceneManager.LoadScene(0);
    }

    private void Buy(int index)
    {
        if (ShowDebug) { Debug.Log($"GameManager - Buy - card {index} is free for {Turn}"); }
        if (CanAfford(BUYAMOUNT))
        {
            if (Turn == PlayersEnum.Player1)
            {
                player1Coins -= BUYAMOUNT;
                properties[index] = PropertyEnum.BelongsTo1;
            }
            else
            {
                player2Coins -= BUYAMOUNT;
                properties[index] = PropertyEnum.BelongsTo2;
            }
            Audio.PlaySFX("Buy");
            if (Settings.FastGame)
            {
                GameStatus = GameStatusEnum.BuyPayDone;
            }
            else
            {
                UI.ShowMessage($"{Turn} landed on a FREE property", $"PAY {BUYAMOUNT}$");
            }
            UI.ShowMoneyChange(Turn, -BUYAMOUNT);
            UI.UpdateCoins(player1Coins, player2Coins);
            UI.UpdateCardOwner(index, properties[index]);
            if (ShowDebug) { Debug.Log($"GameManager - Buy - {Turn} bought card {index}"); }
        }
        else
        {
            if (ShowDebug) { Debug.Log($"GameManager - Buy - {Turn} cannot afford card {index}"); }
            GameOver();
            UI.ShowMessage($"{Turn} landed on a FREE property BUT CAN'T PAY!", $"PLAY AGAIN");
        }
    }

    private void GameOver()
    {
        if (ShowDebug)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"GameManager - GameOver - DiceAnalytics:");
            for (int i = 0; i < diceAnalytics.Length; i++)
            {
                sb.AppendLine($"side {i + 1}: {diceAnalytics[i]} times");
            }
            Debug.Log(sb.ToString()); 
        }
        UI.ShowDiceAnalytics(diceAnalytics);
        GameStatus = GameStatusEnum.GameOver;
    }

    private bool CanAfford(int amount)
    {
        if (Turn == PlayersEnum.Player1)
        {
            if (player1Coins - amount >= 0)
            {
                return true;
            }
            return false;
        }
        else
        {
            if (player2Coins - amount >= 0)
            {
                return true;
            }
            return false;
        }
    }

    private void FillInProperties()
    {
        properties = new PropertyEnum[24];
        properties[0] = PropertyEnum.Start;
        for (int i = 1; i < 23; i++)
        {
            if (IsRewardPlace(i))
            {
                properties[i] = PropertyEnum.Reward;
            }
            else
            {
                properties[i] = PropertyEnum.Free;
            }
        }
        UI.SetAllCards(properties);
    }

    private int GetRollDiceNumber()
    {
        int res = UnityEngine.Random.Range(1, 7);
        if (ShowDebug) { Debug.Log($"GameManager - GetRollDiceNumber - dice rolled {res}"); }
        diceAnalytics[res - 1]++;
        return res;
    }

    private void MovePlayer(int steps)
    {
        switch (Turn)
        {
            case PlayersEnum.Player1:
                player1Position = GetCircularPosition(player1Position, steps);
                if (ShowDebug) { Debug.Log($"GameManager - MovePlayer - {PlayersEnum.Player1} moved to {player1Position}"); }
                break;
            case PlayersEnum.Player2:
                player2Position = GetCircularPosition(player2Position, steps);
                if (ShowDebug) { Debug.Log($"GameManager - MovePlayer - {PlayersEnum.Player2} moved to {player2Position}"); }
                break;
            default:
                Debug.LogWarning("GameManager - MovePlayer - no such player exists");
                break;
        }
    }
}