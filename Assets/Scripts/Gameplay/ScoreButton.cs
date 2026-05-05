using UnityEngine;
using UnityEngine.UI;

public class ScoreButton : MonoBehaviour
{
    // これで Inspector にプルダウンが出るようになるぞ
    public ScoreCategory category;
    private ScoreManager scoreManager;
    private Button btn;

    void Start()
    {
        scoreManager = FindObjectOfType<ScoreManager>();
        btn = GetComponent<Button>();

        if (btn != null && scoreManager != null)
        {
            btn.onClick.AddListener(() =>
            {
                scoreManager.OnScoreSelected(category);
                // 選択されたら自分をグレーアウト
                DisableButton();
            });
        }
    }

    // ボタンをグレーにして押せなくする
    public void DisableButton()
    {
        if (btn == null) btn = GetComponent<Button>();
        btn.interactable = false; // 物理的に押せなくする

        // Imageの色をグレーにする（ColorBlockを使う方法もあるが、シンプルにImageの色を変える）
        Image img = GetComponent<Image>();
        if (img != null)
        {
            img.color = Color.gray;
        }
    }
}