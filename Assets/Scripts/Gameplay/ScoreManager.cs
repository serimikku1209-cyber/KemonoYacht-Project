using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    // GameObjectではなくCanvasGroupをアタッチする
    [SerializeField] private CanvasGroup scoreTableCanvasGroup;

    void Start()
    {
        // 最初は非表示
        HideScoreTable();
    }

    public void HideScoreTable()
    {
        if (scoreTableCanvasGroup != null)
        {
            scoreTableCanvasGroup.alpha = 0f;          // 透明にする
            scoreTableCanvasGroup.interactable = false; // ボタンを押せなくする
            scoreTableCanvasGroup.blocksRaycasts = false; // マウス反応を消す
        }
    }

    public void ShowScoreTable()
    {
        if (scoreTableCanvasGroup != null)
        {
            scoreTableCanvasGroup.alpha = 1f;          // 表示する
            scoreTableCanvasGroup.interactable = true;  // ボタンを押せるようにする
            scoreTableCanvasGroup.blocksRaycasts = true; // マウス反応を戻す
        }
    }
}
