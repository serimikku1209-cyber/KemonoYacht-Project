using UnityEngine;
using TMPro;
using System.Linq;

// 役の定義（クラスの外に置くことで、ScoreButtonからも直接見えるようにする）
public enum ScoreCategory
{
    Ones, Twos, Threes, Fours, Fives, Sixes,
    HoshikuzuHiroi, // チャンス
    HoshiAtsume,    // 4カード
    KemonoNoOyado,  // フルハウス
    KomichiNoSanpo, // S.ストレート
    SvelGoushin,    // B.ストレート
    KemonoKowars    // ヤッツィー
}

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup scoreTableCanvasGroup;
    [SerializeField] private DiceController diceController;

    [Header("スコアテキスト（0〜11番まで順番にアタッチ）")]
    public TextMeshProUGUI[] scoreTexts;

    [Header("集計用テキスト")]
    public TextMeshProUGUI upperTotalText; // 1~6の小計用
    public TextMeshProUGUI grandTotalText; // 最終的な合計用

    private bool[] isScoreFixed = new bool[12];
    private int[] scores = new int[12]; // 各項目の数値を保持

    [Header("ボーナス演出用")]
    public GameObject bonusBadge; // ここに作った吹き出しオブジェクトをアタッチしてくれ！

    void Start()
    {
        HideScoreTable();
        // 初期化
        foreach (var text in scoreTexts) if (text != null) text.text = "";
        if (upperTotalText != null) upperTotalText.text = "0";
        if (grandTotalText != null) grandTotalText.text = "0";
    }

    /// <summary>
    /// スコアボタンが押された時のメイン処理
    /// </summary>
    public void OnScoreSelected(ScoreCategory category)
    {
        int categoryIndex = (int)category;

        // すでに記入済みなら何もしない
        if (isScoreFixed[categoryIndex]) return;

        // ダイスの目を取得 (DiceControllerから)
        int[] diceValues = diceController.GetCurrentDiceValues();

        // 点数計算を実行
        int score = CalculateScore(category, diceValues);

        // スコアを保存・表示
        scores[categoryIndex] = score;
        scoreTexts[categoryIndex].text = score.ToString();
        scoreTexts[categoryIndex].color = Color.yellow;
        isScoreFixed[categoryIndex] = true;

        // 合計点の再計算
        UpdateTotalScores();

        // パネルを閉じてターン終了
        HideScoreTable();
        diceController.ResetTurn();
    }

    /// <summary>
    /// 役に応じた点数計算
    /// </summary>
    private int CalculateScore(ScoreCategory category, int[] dice)
    {
        var counts = dice.GroupBy(d => d).ToDictionary(g => g.Key, g => g.Count());
        int[] sortedDice = dice.OrderBy(d => d).ToArray();

        switch (category)
        {
            case ScoreCategory.Ones: return dice.Where(d => d == 1).Sum();
            case ScoreCategory.Twos: return dice.Where(d => d == 2).Sum();
            case ScoreCategory.Threes: return dice.Where(d => d == 3).Sum();
            case ScoreCategory.Fours: return dice.Where(d => d == 4).Sum();
            case ScoreCategory.Fives: return dice.Where(d => d == 5).Sum();
            case ScoreCategory.Sixes: return dice.Where(d => d == 6).Sum();

            case ScoreCategory.HoshikuzuHiroi:
                return dice.Sum();

            case ScoreCategory.HoshiAtsume:
                return counts.Values.Any(c => c >= 4) ? dice.Sum() : 0;

            case ScoreCategory.KemonoNoOyado:
                bool hasThree = counts.Values.Any(c => c == 3);
                bool hasTwo = counts.Values.Any(c => c == 2);
                bool isYacht = counts.Values.Any(c => c == 5);
                return (hasThree && hasTwo) || isYacht ? 25 : 0;

            case ScoreCategory.KomichiNoSanpo:
                return CheckStraight(dice, 4) ? 30 : 0;

            case ScoreCategory.SvelGoushin:
                return CheckStraight(dice, 5) ? 40 : 0;

            case ScoreCategory.KemonoKowars:
                return counts.Values.Any(c => c == 5) ? 50 : 0;

            default: return 0;
        }
    }

    /// <summary>
    /// 合計スコアの更新
    /// </summary>
    private void UpdateTotalScores()
    {
        // 1〜6の合計
        int upperSum = 0;
        for (int i = 0; i <= 5; i++) upperSum += scores[i];

        // ボーナス判定（63点以上で+35）
        int bonus = (upperSum >= 63) ? 35 : 0;

        // UI表示（ボーナスがある時だけカッコ書き）
        if (bonusBadge != null)
        {
            // ボーナスがあれば表示、なければ非表示
            bonusBadge.SetActive(bonus > 0);
        }

        if (upperTotalText != null)
        {
            upperTotalText.text = upperSum.ToString();
        }

        // 全体の総計
        int grandTotal = scores.Sum() + bonus;
        if (grandTotalText != null) grandTotalText.text = grandTotal.ToString();
    }

    private bool CheckStraight(int[] dice, int length)
    {
        var sortedDistinct = dice.Distinct().OrderBy(d => d).ToArray();
        int consecutive = 1;
        int maxConsecutive = 1;
        for (int i = 0; i < sortedDistinct.Length - 1; i++)
        {
            if (sortedDistinct[i + 1] == sortedDistinct[i] + 1) consecutive++;
            else consecutive = 1;
            maxConsecutive = Mathf.Max(maxConsecutive, consecutive);
        }
        return maxConsecutive >= length;
    }

    public void HideScoreTable() => SetCanvasGroup(false);
    public void ShowScoreTable() => SetCanvasGroup(true);

    private void SetCanvasGroup(bool show)
    {
        if (scoreTableCanvasGroup == null) return;
        scoreTableCanvasGroup.alpha = show ? 1f : 0f;
        scoreTableCanvasGroup.interactable = show;
        scoreTableCanvasGroup.blocksRaycasts = show;
    }
}