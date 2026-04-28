using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DiceController : MonoBehaviour
{
    [Header("背景画像（土台）")]
    public Sprite normalBaseSprite; // 通常時の四角い枠
    public Sprite keptBaseSprite;   // キープ時の四角い枠

    [Header("数字の画像（1〜6）")]
    public Sprite[] numberSprites = new Sprite[6];

    [Header("ダイスボタン本体（土台のImage制御用）")]
    public Button[] diceButtons; // 親：Dice1〜5

    [Header("数字を表示するImage（子のオブジェクト）")]
    public Image[] diceNumberImages; // 子：Number

    private int[] currentDiceValues = new int[5]; // 0〜5で保持
    private bool[] isKept = new bool[5];
    private bool isRolling = false;

    void Start()
    {
        // 全ボタンにイベント登録
        for (int i = 0; i < diceButtons.Length; i++)
        {
            int index = i;
            if (diceButtons[i] != null)
            {
                diceButtons[i].onClick.AddListener(() => OnDiceClick(index));
            }
        }
    }

    public void OnDiceClick(int index)
    {
        if (isRolling) return;

        isKept[index] = !isKept[index];
        UpdateDiceVisual(index);
    }

    private void UpdateDiceVisual(int index)
    {
        int faceIndex = currentDiceValues[index];

        // 1. 土台（背景）の切り替え
        Image baseImage = diceButtons[index].GetComponent<Image>();
        if (baseImage != null)
        {
            baseImage.sprite = isKept[index] ? keptBaseSprite : normalBaseSprite;
        }

        // 2. 数字の表示切り替え
        if (diceNumberImages[index] != null)
        {
            diceNumberImages[index].sprite = numberSprites[faceIndex];
        }
    }

    public void OnDiceButtonClick()
    {
        if (!isRolling) StartCoroutine(RollDiceRoutine());
        else isRolling = false; // ストップ処理（トグル）
    }

    private IEnumerator RollDiceRoutine()
    {
        isRolling = true;

        while (isRolling)
        {
            for (int i = 0; i < diceButtons.Length; i++)
            {
                // キープされていないダイスだけ回す
                if (!isKept[i])
                {
                    int randomIndex = Random.Range(0, 6);
                    currentDiceValues[i] = randomIndex;

                    // ロール中は常に通常背景（またはロール中専用背景）で見せる
                    diceNumberImages[i].sprite = numberSprites[randomIndex];
                }
            }
            yield return new WaitForSeconds(0.05f);
        }

        // 停止後に全ての見た目を最終確定
        for (int i = 0; i < diceButtons.Length; i++)
        {
            UpdateDiceVisual(i);
        }
    }
}