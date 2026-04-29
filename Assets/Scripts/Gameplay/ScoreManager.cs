using UnityEngine;
using TMPro;
using System.Linq; // これを冒頭に追記してくれ（並べ替えや集計が楽になる！）

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

    [Header("スコアテキスト（上から順にアタッチ）")]
    // 0:Ones, 1:Twos ... 5:Sixes, 6:Hoshikuzu, 7:HoshiAtsume... の順
    public TextMeshProUGUI[] scoreTexts;

    // スコアが確定済みかどうかを記録する
    private bool[] isScoreFixed = new bool[12];

    void Start()
    {
        HideScoreTable();
        // テキストを初期化（空にする）
        foreach (var text in scoreTexts) if (text != null) text.text = "";
    }

    public void OnScoreSelected(ScoreCategory category)
    {
        int categoryIndex = (int)category;

        // すでに記入済みなら何もしない
        if (isScoreFixed[categoryIndex])
        {
            Debug.Log("そこはもう記入済みだぞ！");
            return;
        }

        // ダイスの目を取得 (1~6の数値)
        int[] diceValues = diceController.GetCurrentDiceValues();

        // 点数計算
        int score = CalculateScore(category, diceValues);

        // スコアボードに反映
        scoreTexts[categoryIndex].text = score.ToString();
        scoreTexts[categoryIndex].color = Color.yellow; // 確定したことがわかるように色を変える
        isScoreFixed[categoryIndex] = true;

        // パネルを閉じてターン終了
        HideScoreTable();
        diceController.ResetTurn();
    }

    private int CalculateScore(ScoreCategory category, int[] dice)
    {
        // これを追記！ ダイスの中に各数字がいくつあるか数える処理だ
        var counts = dice.GroupBy(d => d).ToDictionary(g => g.Key, g => g.Count());
        // ストレート判定しやすくするために昇順に並べ替えた配列も用意しておく
        int[] sortedDice = dice.OrderBy(d => d).ToArray();

        switch (category)
        {
            // --- 前半（アッパーセクション） ---
            case ScoreCategory.Ones: return SumSpecificDice(dice, 1);
            case ScoreCategory.Twos: return SumSpecificDice(dice, 2);
            case ScoreCategory.Threes: return SumSpecificDice(dice, 3);
            case ScoreCategory.Fours: return SumSpecificDice(dice, 4);
            case ScoreCategory.Fives: return SumSpecificDice(dice, 5);
            case ScoreCategory.Sixes: return SumSpecificDice(dice, 6);

            // --- 後半（独自役名：ロワーセクション） ---
            // --- 後半（独自役名：ロワーセクション） ---
            case ScoreCategory.HoshikuzuHiroi: // 星屑ひろい（チャンス）：全合計
                return dice.Sum();

            case ScoreCategory.HoshiAtsume: // 星あつめ（4カード）：同じ目4つ以上
                // いずれかの目が4つ以上あれば全合計、なければ0点
                return counts.Values.Any(c => c >= 4) ? dice.Sum() : 0;

            case ScoreCategory.KemonoNoOyado: // けもののお宿（フルハウス）：3つ同点 + 2つ同点
                // 3つのペアと2つのペアがあるかチェック
                bool hasThree = counts.Values.Any(c => c == 3);
                bool hasTwo = counts.Values.Any(c => c == 2);
                // ヤッツィー（5つ同点）もフルハウスとして認めるのが一般的
                bool isYacht = counts.Values.Any(c => c == 5);
                return (hasThree && hasTwo) || isYacht ? 25 : 0;

            case ScoreCategory.KomichiNoSanpo: // こみちのさんぽ（S.ストレート）：4つ連番
                // 重複を除いて並べ替え
                int[] distinctDice = sortedDice.Distinct().ToArray();
                if (CheckStraight(distinctDice, 4)) return 30;
                return 0;

            case ScoreCategory.SvelGoushin: // スヴェル行進（B.ストレート）：5つ連番
                int[] distinctDiceB = sortedDice.Distinct().ToArray();
                if (CheckStraight(distinctDiceB, 5)) return 40;
                return 0;

            case ScoreCategory.KemonoKowars: // けものこうぉーず！（ヤッツィー）：全部同じ目
                return counts.Values.Any(c => c == 5) ? 50 : 0;

            default:
                return 0;
        }
    }
    /// <summary>
    /// 連番（ストレート）になっているか判定するヘルパー
    /// </summary>
    private bool CheckStraight(int[] distinctDice, int requiredLength)
    {
        if (distinctDice.Length < requiredLength) return false;

        int consecutiveCount = 1;
        int maxConsecutive = 1;

        for (int i = 0; i < distinctDice.Length - 1; i++)
        {
            if (distinctDice[i + 1] == distinctDice[i] + 1)
            {
                consecutiveCount++;
                maxConsecutive = Mathf.Max(maxConsecutive, consecutiveCount);
            }
            else
            {
                consecutiveCount = 1;
            }
        }
        return maxConsecutive >= requiredLength;
    }



    private int SumSpecificDice(int[] dice, int target)
    {
        int sum = 0;
        foreach (int d in dice) if (d == target) sum += d;
        return sum;
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