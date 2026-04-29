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
        if (rollCount >= MaxRollCount)
        {
            Debug.Log("スコア選択へ進みます");
            if (scoreManager != null)
            {
                scoreManager.ShowScoreTable();
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
        UpdateRollButtonUI();

        for (int i = 0; i < diceButtons.Length; i++) UpdateDiceVisual(i);
    }

    // ここを修正：new int にしたぞ！
    public int[] GetCurrentDiceValues()
    {
        int[] values = new int[6];
        for (int i = 0; i < currentDiceValues.Length; i++)
        {
            // 内部インデックス(0~5)に1を足して、実際の目(1~6)にする
            values[i] = currentDiceValues[i] + 1;
        }
        return values;
    }
}