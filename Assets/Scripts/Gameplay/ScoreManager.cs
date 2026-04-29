using UnityEngine;
using TMPro;

// 役の定義（クラスの外に出すことで、どこからでも ScoreCategory と呼べる）
public enum ScoreCategory
{
    Ones, Twos, Threes, Fours, Fives, Sixes,
    HoshikuzuHiroi,
    HoshiAtsume,
    KemonoNoOyado,
    KomichiNoSanpo,
    SvelGoushin,
    KemonoKowars
}

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup scoreTableCanvasGroup;
    [SerializeField] private DiceController diceController;

    [Header("スコアテキスト参照")]
    public TextMeshProUGUI[] scoreTexts; // Inspectorで各役のテキストを順番に入れておくと楽だぞ！

    void Start()
    {
        HideScoreTable();
    }

    public void OnScoreSelected(ScoreCategory category)
    {
        Debug.Log(category + " が選択された！");

        // TODO: ここで点数計算（現在は仮）
        // int score = CalculateScore(category, diceController.GetCurrentDiceValues());

        HideScoreTable();

        if (diceController != null)
        {
            diceController.ResetTurn();
        }
    }

    public void HideScoreTable()
    {
        if (scoreTableCanvasGroup != null)
        {
            scoreTableCanvasGroup.alpha = 0f;
            scoreTableCanvasGroup.interactable = false;
            scoreTableCanvasGroup.blocksRaycasts = false;
        }
    }

    public void ShowScoreTable()
    {
        if (scoreTableCanvasGroup != null)
        {
            scoreTableCanvasGroup.alpha = 1f;
            scoreTableCanvasGroup.interactable = true;
            scoreTableCanvasGroup.blocksRaycasts = true;
        }
    }
}