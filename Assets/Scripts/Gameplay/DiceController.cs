using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DiceController : MonoBehaviour
{
    [Header("UI参照")]
    public Button rollButton;       // ロールボタン本体
    public TextMeshProUGUI rollButtonText;    // ボタンのテキスト（「ROLL」「STOP」切り替え用）
    public void UpdateButtonText(string newText)
    {
        rollButtonText.text = newText;
    }

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

    // --- 追加：回数管理用 ---
    private int rollCount = 0;
    private const int MaxRollCount = 3;

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
        // ロール中、または3回振り切った後はキープ変更不可
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

    // ロールボタンが押された時の処理
    public void OnDiceButtonClick()
    {
        if (rollCount >= MaxRollCount) return; // 3回制限

        if (!isRolling)
        {
            StartCoroutine(RollDiceRoutine());
        }
        else
        {
            isRolling = false; // ストップ！
        }
    }

    private IEnumerator RollDiceRoutine()
    {
        isRolling = true;
        UpdateRollButtonUI(); // テキストを「STOP」にする

        while (isRolling)
        {
            for (int i = 0; i < diceButtons.Length; i++)
            {
                if (!isKept[i])
                {
                    int randomIndex = Random.Range(0, 6);
                    currentDiceValues[i] = randomIndex;
                    diceNumberImages[i].sprite = numberSprites[randomIndex];
                }
            }
            yield return new WaitForSeconds(0.05f);
        }

        // --- 確定処理 ---
        rollCount++; // 回数を増やす

        for (int i = 0; i < diceButtons.Length; i++)
        {
            UpdateDiceVisual(i);
        }

        UpdateRollButtonUI(); // テキストを戻す or ボタン無効化
    }

    // ボタンの状態やテキストを更新する
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
                rollButton.interactable = false; // ボタンを無効化
            }
            else
            {
                if (rollButtonText != null) rollButtonText.text = $"ROLL ({rollCount}/{MaxRollCount})";
            }
        }
    }

    // ターン終了時に外部（GameManagerなど）から呼ぶリセット関数
    public void ResetTurn()
    {
        rollCount = 0;
        for (int i = 0; i < isKept.Length; i++) isKept[i] = false;
        rollButton.interactable = true;
        UpdateRollButtonUI();
    }
}