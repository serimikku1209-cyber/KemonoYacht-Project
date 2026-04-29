using UnityEngine;
using TMPro;

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
        int sum = 0;
        switch (category)
        {
            // 1～6の目は、その目の合計
            case ScoreCategory.Ones: return SumSpecificDice(dice, 1);
            case ScoreCategory.Twos: return SumSpecificDice(dice, 2);
            case ScoreCategory.Threes: return SumSpecificDice(dice, 3);
            case ScoreCategory.Fours: return SumSpecificDice(dice, 4);
            case ScoreCategory.Fives: return SumSpecificDice(dice, 5);
            case ScoreCategory.Sixes: return SumSpecificDice(dice, 6);

            case ScoreCategory.HoshikuzuHiroi: // チャンス（全合計）
                foreach (int d in dice) sum += d;
                return sum;

            case ScoreCategory.HoshiAtsume: // 4カード（仮：合計値。本来は4つ以上同じ目が必要）
                return 20; // あとで判定ロジックを書く

            case ScoreCategory.KemonoNoOyado: // フルハウス
                return 25; // 固定点

            case ScoreCategory.KomichiNoSanpo: // S.ストレート
                return 30;

            case ScoreCategory.SvelGoushin: // B.ストレート
                return 40;

            case ScoreCategory.KemonoKowars: // ヤッツィー
                return 50;

            default: return 0;
        }
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