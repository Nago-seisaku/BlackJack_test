using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BlackJackManeger : MonoBehaviour
{
    public int shuffleCount = 100;
    public GameObject cardPrefab;
    public GameObject player;
    public GameObject dealer;
    public GameObject inputBetsDialog;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI pointsText;
    public TextMeshProUGUI betText;
    public TextMeshProUGUI turnText;
    public TextMeshProUGUI playerScoreText;
    public TextMeshProUGUI dealerScoreText;
    public InputField betInputField;

    private int cardIndex = 0;
    private List<int> cards;
    private int playerScore;
    private int dealerScore;
    private int turn;
    private bool playerFlag;
    private bool dealerFlag;
    private int playerPoints = 100;
    private int currentBet = 0;

    private Card dealerHiddenCard;

    // Start is called before the first frame update
    void Start()
    {
        SetCard();
        UpdatePointsText();
        ShowBetDialog();
    }
    private void ShowBetDialog()
    {
        inputBetsDialog.SetActive(true);
    }
    public void OnButtonDown_Bet()
    {
        int bet;
        if (int.TryParse(betInputField.text, out bet) && bet > 0 && bet <= playerPoints)
        {
            currentBet = bet;
            betText.text = "Bet：" + currentBet;
            inputBetsDialog.SetActive(false);
            DealCards();
        }
        else
        {
            betInputField.text = "";
            betInputField.placeholder.GetComponent<Text>().text = "Invalid bet. Try again.";
        }
    }
    //カードを生成する
    private void SetCard()
    {

        if (cards == null)
        {
            cards = new List<int>();
        }
        else
        {
            cards.Clear();
        }
        for (int i = 0;i<52;i++) 
        {
            cards.Add(i);
        }
        ShuffleCard();
        cardIndex = 0;
        playerScore = 0;
        dealerScore = 0;
        turn = 0;
        UpdateTurnText();
        playerScoreText.text = "PlayerScore：" + playerScore.ToString();
        dealerScoreText.text = "DealerScore：" + dealerScore.ToString();
    }
    //シャッフルする
    private void ShuffleCard()
    {

        for (int i = 0; i < shuffleCount; i++)
        {
            int index1 = Random.Range(0, 52);
            int index2 = Random.Range(0, 52);
            int tmp = cards[index1];
            cards[index1] = cards[index2];
            cards[index2] = tmp;
        }
    }
    //カードを配る
    public void DealCards()
    {
        ClearCards(dealer);
        ClearCards(player);

        DealCard(dealer, false);
        DealCard(dealer, true);

        for(int i = 0; i < 2; i++)
        {
            DealCard(player, true);
        }

        PlayerTurn();
    }
    private void ClearCards(GameObject hand)
    {
        foreach (Transform card in hand.transform)
        {
            Destroy(card.gameObject);
        }
    }

    // Deal a single card
    private void DealCard(GameObject hand, bool faceUp)
    {
        Card cardObj = Instantiate(cardPrefab, hand.transform).GetComponent<Card>();
        cardObj.SetCard(cards[cardIndex], faceUp);
        Debug.Log($"Dealt card: {cards[cardIndex]} to {hand.name}, Face Up: {faceUp}");
        cardObj.num = cards[cardIndex] % 13 + 1;
        if (hand == player)
        {
            playerScore += GetCardValue(cardObj.num, ref playerScore);
            playerScoreText.text = "PlayerScore：" + playerScore.ToString();
        }
        else
        {
            dealerScore += GetCardValue(cardObj.num, ref dealerScore);
            if (!faceUp && hand == dealer)
            {
                dealerHiddenCard = cardObj;
            }
        }

        cardIndex++;
    }
    private int GetCardValue(int num, ref int score)
    {
        int value = num > 10 ? 10 : num;
        if (value == 1 && score + 11 <= 21)
        {
            return 11;
        }
        return value;
    }
    //プレイヤーの行動
    private void PlayerTurn()
    {
        playerScoreText.text = "PlayerScore：" + playerScore.ToString();
        playerFlag = ScoreCheck(playerScore);
        turn = 1;
        UpdateTurnText();
        if (playerFlag == false)
        {
            Result();
        }
    }
    //ディーラーの行動
    private void DealerTurn()
    {
        turn = 2;
        UpdateTurnText();
        if (dealerHiddenCard != null)
        {
            dealerHiddenCard.SetCard(dealerHiddenCard.cardIndex, true);
            dealerHiddenCard = null;
        }
        while (dealerScore < 17)
        {
            Hit(dealer);
            dealerFlag = ScoreCheck(dealerScore);
        }
        dealerFlag = ScoreCheck(dealerScore);
        Result();
    }

    //値チェック
    private bool ScoreCheck(int score)
    {
        return score <= 21;
    }

    //勝敗処理
    private void Result()
    {
        turn = 3;
        UpdateTurnText();
        dealerScoreText.text = "DealerScore：" + dealerScore.ToString();
        //プレイヤーがバストした時の処理
        if (!playerFlag)
        {
            if (dealerHiddenCard != null)
            {
                dealerHiddenCard.SetCard(dealerHiddenCard.cardIndex, true);
                dealerHiddenCard = null;
            }
            dealerFlag = ScoreCheck(dealerScore);

            resultText.text = "Bust…";
            playerPoints -= currentBet;
        }
        //勝ち
        else if ((playerFlag && dealerFlag && playerScore > dealerScore) || (playerFlag && !dealerFlag))
        {
            resultText.text = "Win!";
            playerPoints += currentBet;
        }
        //負け
        else if ((playerFlag && dealerFlag && playerScore < dealerScore))
        {
            resultText.text = "Lose…";
            playerPoints -= currentBet;
        }
        //引き分け
        else
        {
            resultText.text = "Draw";
        }

        /*
        //デバッグ用
        else
        { if ((playerFlag && dealerFlag && playerScore == dealerScore))
            resultText.text = "コードを見直せ";
        }
        */

        UpdatePointsText();

        if (playerPoints <= 0)
        {
            StartCoroutine(GameOver());
        }
        else
        {
            StartCoroutine(NextGame());
        }
    }
    private void UpdateTurnText()
    {
        switch (turn)
        {
            case 0:
                turnText.text = "Turn";
                break; 
            case 1:
                turnText.text = "PlayerTurn";
                break;
            case 2:
                turnText.text = "DealerTurn";
                break;
            case 3:
                turnText.text = "Result";
                break;
        }
    }
    private void UpdatePointsText()
    {
        pointsText.text = "Points：" + playerPoints;
    }
    private IEnumerator NextGame()
    {
        yield return new WaitForSeconds(2);
        resultText.text = " ";
        SetCard();
        ShowBetDialog();
    }
    private IEnumerator GameOver()
    {
        resultText.text = "Game Over";
        yield return new WaitForSeconds(2);

        SceneManager.LoadScene("Home");
    }
    //カードをもう一枚増やす
    public void Hit(GameObject hand)
    {
        DealCard(hand, true);
        if (hand == player)
        {
            playerFlag = ScoreCheck(playerScore);
            if (!playerFlag)
            {
                Result();
            }
        }
        else
        {
            dealerFlag = ScoreCheck(dealerScore);
        }
    }
    //プレイヤーがもう一枚めくる
    public void OnButtonDown_Hit()
    {
        if(playerFlag && turn == 1)
        {
            Hit(player);
        }
    }
    //カードをそのままにし、ディーラーの処理に移行する
    public void OnButtonDown_Stand()
    {
        if (playerFlag && turn == 1)
        {
            DealerTurn();
        }
    }
    //ホームに戻る
    public void OnButtonDown_Home()
    {
        SceneManager.LoadScene("Home");
    }
    void Update()
    {
        // Esc キーが押されたかどうかを確認
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // ゲームを終了する
            QuitGame();
        }
    }

    // ゲームを終了するメソッド
    private void QuitGame()
    {
#if UNITY_EDITOR
        // エディター内で実行されている場合は Play モードを停止する
        UnityEditor.EditorApplication.isPlaying = false;
#else
            // エディター外で実行されている場合はアプリケーションを終了する
            Application.Quit();
#endif
    }
}