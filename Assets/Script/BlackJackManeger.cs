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
            betText.text = "Bet�F" + currentBet;
            inputBetsDialog.SetActive(false);
            DealCards();
        }
        else
        {
            betInputField.text = "";
            betInputField.placeholder.GetComponent<Text>().text = "Invalid bet. Try again.";
        }
    }
    //�J�[�h�𐶐�����
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
        playerScoreText.text = "PlayerScore�F" + playerScore.ToString();
        dealerScoreText.text = "DealerScore�F" + dealerScore.ToString();
    }
    //�V���b�t������
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
    //�J�[�h��z��
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
            playerScoreText.text = "PlayerScore�F" + playerScore.ToString();
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
    //�v���C���[�̍s��
    private void PlayerTurn()
    {
        playerScoreText.text = "PlayerScore�F" + playerScore.ToString();
        playerFlag = ScoreCheck(playerScore);
        turn = 1;
        UpdateTurnText();
        if (playerFlag == false)
        {
            Result();
        }
    }
    //�f�B�[���[�̍s��
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

    //�l�`�F�b�N
    private bool ScoreCheck(int score)
    {
        return score <= 21;
    }

    //���s����
    private void Result()
    {
        turn = 3;
        UpdateTurnText();
        dealerScoreText.text = "DealerScore�F" + dealerScore.ToString();
        //�v���C���[���o�X�g�������̏���
        if (!playerFlag)
        {
            if (dealerHiddenCard != null)
            {
                dealerHiddenCard.SetCard(dealerHiddenCard.cardIndex, true);
                dealerHiddenCard = null;
            }
            dealerFlag = ScoreCheck(dealerScore);

            resultText.text = "Bust�c";
            playerPoints -= currentBet;
        }
        //����
        else if ((playerFlag && dealerFlag && playerScore > dealerScore) || (playerFlag && !dealerFlag))
        {
            resultText.text = "Win!";
            playerPoints += currentBet;
        }
        //����
        else if ((playerFlag && dealerFlag && playerScore < dealerScore))
        {
            resultText.text = "Lose�c";
            playerPoints -= currentBet;
        }
        //��������
        else
        {
            resultText.text = "Draw";
        }

        /*
        //�f�o�b�O�p
        else
        { if ((playerFlag && dealerFlag && playerScore == dealerScore))
            resultText.text = "�R�[�h��������";
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
        pointsText.text = "Points�F" + playerPoints;
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
    //�J�[�h�������ꖇ���₷
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
    //�v���C���[�������ꖇ�߂���
    public void OnButtonDown_Hit()
    {
        if(playerFlag && turn == 1)
        {
            Hit(player);
        }
    }
    //�J�[�h�����̂܂܂ɂ��A�f�B�[���[�̏����Ɉڍs����
    public void OnButtonDown_Stand()
    {
        if (playerFlag && turn == 1)
        {
            DealerTurn();
        }
    }
    //�z�[���ɖ߂�
    public void OnButtonDown_Home()
    {
        SceneManager.LoadScene("Home");
    }
    void Update()
    {
        // Esc �L�[�������ꂽ���ǂ������m�F
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // �Q�[�����I������
            QuitGame();
        }
    }

    // �Q�[�����I�����郁�\�b�h
    private void QuitGame()
    {
#if UNITY_EDITOR
        // �G�f�B�^�[���Ŏ��s����Ă���ꍇ�� Play ���[�h���~����
        UnityEditor.EditorApplication.isPlaying = false;
#else
            // �G�f�B�^�[�O�Ŏ��s����Ă���ꍇ�̓A�v���P�[�V�������I������
            Application.Quit();
#endif
    }
}