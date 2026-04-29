using UnityEngine;
using UnityEngine.UI;

public class ScoreButton : MonoBehaviour
{
    // これで Inspector にプルダウンが出るようになるぞ
    public ScoreCategory category;

    private ScoreManager scoreManager;

    void Start()
    {
        // シーン内の ScoreManager を探す
        scoreManager = FindObjectOfType<ScoreManager>();

        Button btn = GetComponent<Button>();
        if (btn != null && scoreManager != null)
        {
            // ボタンが押されたときに役の種類を伝えて実行
            btn.onClick.AddListener(() => scoreManager.OnScoreSelected(category));
        }
    }
}