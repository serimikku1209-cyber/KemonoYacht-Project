using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class DiceController : MonoBehaviour
{
    [Header("UI参照")]
    public Button rollButton;
    public TextMeshProUGUI rollButtonText;

    [Header("背景画像（土台）")]
    public Sprite normalBaseSprite;
    public Sprite keptBaseSprite;

    [Header("数字の画像（1〜6）")]
    public Sprite[] numberSprites = new Sprite[6];

    [Header("ダイスボタン本体")]
    public Button[] diceButtons;

    [Header("数字を表示するImage")]
    public Image[] diceNumberImages;

    private int[] currentDiceValues = new int[5];
    private bool[] isKept = new bool[5];
    private bool isRolling = false;

    private int rollCount = 0;
    private const int MaxRollCount = 3;

    [Header("ScoreManager参照")]
    public ScoreManager scoreManager;


    void Start()
    {
        for (int i = 0; i < diceButtons.Length; i++)
        {
            int index = i;
            if (diceButtons[i] != null)
            {
                diceButtons[i].onClick.AddListener(() => OnDiceClick(index));
            }
        }
        UpdateRollButtonUI();
    }

    public void OnDiceClick(int index)
    {
        // 修正：3回振り切った後は、ダイスを触っても反応させない
        if (isRolling || rollCount >= MaxRollCount) return;

        isKept[index] = !isKept[index];
        UpdateDiceVisual(index);
    }

    private void UpdateDiceVisual(int index)
    {
        int faceIndex = currentDiceValues[index];
        Image baseImage = diceButtons[index].GetComponent<Image>();
        if (baseImage != null)
        {
            baseImage.sprite = isKept[index] ? keptBaseSprite : normalBaseSprite;
        }
        if (diceNumberImages[index] != null)
        {
            diceNumberImages[index].sprite = numberSprites[faceIndex];
        }
    }

    public void OnDiceButtonClick()
    {
        // 修正：3回振った後にボタンを押した場合はスコア選択フェーズへ
        if (rollCount >= MaxRollCount)
        {
            Debug.Log("スコア選択へ進みます");
            // ここに ScoreManager.ShowScoreOptions() などを呼ぶ処理を書く予定
            if (scoreManager != null)
            {
                scoreManager.ShowScoreTable(); // スコアパネルを表示！
            }
            return;
        }

        if (!isRolling)
        {
            StartCoroutine(RollDiceRoutine());
        }
        else
        {
            isRolling = false;
        }
    }

    private IEnumerator RollDiceRoutine()
    {
        isRolling = true;
        UpdateRollButtonUI();

        while (isRolling)
        {
            for (int i = 0; i < diceButtons.Length; i++)
            {
                if (!isKept[i])
                {
                    
                    int randomIndex = UnityEngine.Random.Range(0, 6);
                    currentDiceValues[i] = randomIndex;
                    diceNumberImages[i].sprite = numberSprites[randomIndex];
                }
            }
            yield return new WaitForSeconds(0.05f);
        }

        rollCount++;

        // 修正：3回目に達したら、強制的に全てのダイスを「キープ状態」にする
        if (rollCount >= MaxRollCount)
        {
            for (int i = 0; i < isKept.Length; i++)
            {
                isKept[i] = true;
            }
        }

        for (int i = 0; i < diceButtons.Length; i++)
        {
            UpdateDiceVisual(i);
        }

        UpdateRollButtonUI();
    }

    private void UpdateRollButtonUI()
    {
        if (isRolling)
        {
            if (rollButtonText != null) rollButtonText.text = "STOP";
        }
        else
        {
            if (rollCount >= MaxRollCount)
            {
                // 修正：テキストを SELECT SCORE に変えるが、ボタンは無効化（interactable = false）しない
                if (rollButtonText != null) rollButtonText.text = "SELECT SCORE";
            }
            else
            {
                if (rollButtonText != null) rollButtonText.text = $"ROLL ({rollCount}/{MaxRollCount})";
            }
        }
    }

    public void ResetTurn()
    {
        rollCount = 0;
        for (int i = 0; i < isKept.Length; i++) isKept[i] = false;
        // interactable は常に true で良くなったのでここでの変更は不要
        UpdateRollButtonUI();

        // ターン開始時にダイスの見た目もリセット（任意）
        for (int i = 0; i < diceButtons.Length; i++) UpdateDiceVisual(i);
    }
}