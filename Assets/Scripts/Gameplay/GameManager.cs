using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    // Resultシーンへ渡す勝敗データ（true=勝ち / false=負け）
    // staticにすることでシーンをまたいでも値が残る
    public static bool isWin;
    // Turnは1～12
    private int currentTurn = 1;
    private int maxTurns = 12;
    private int playerFinalScore = 0;

    //Gameの開始と終了をとる
    private GameState currentState;

    // インスペクターで GamePerformance オブジェクトをドラッグ＆ドロップして紐付ける
    [SerializeField] private GamePerformance performance;

    private void Awake()
    {
        if (performance != null) performance.Init();
    }

    void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        // 演出スクリプトに「この難易度で演出始めて！」と丸投げする
        StartCoroutine(performance.PlayStartSequence(SelectLevel.difficulty));
    }

    void OnButtonClick()
    {
        Debug.Log("コードから紐づけたボタンが押されました！");
    }

    // スコアを受け取ってターンを進める（差し替え箇所）
    public void NextTurn(int playerCurrentScore)
    {
        playerFinalScore = playerCurrentScore; // 常に最新のスコアを保持
        Debug.Log($"ターン {currentTurn} 完了。現在のスコア: {playerFinalScore}");

        if (currentTurn >= maxTurns)
        {
            // 直接 JudgeWinner を呼ばず、コルーチンで「間」を作る
            StartCoroutine(EndGameSequence());
        }
        else
        {
            currentTurn++;
            Debug.Log($"次のターンへ進みます: 第 {currentTurn} ターン");
            // 今後ここに ChangeState(GameState.EnemyTurn) などを追加予定
        }
    }
    // ゲーム終了時の演出用シーケンス
    private IEnumerator EndGameSequence()
    {
        Debug.Log("全ターン終了。少し待機してから判定します...");

        // 1.5秒ほど待機（ここでスコアが書き込まれたのを確認させる）
        yield return new WaitForSeconds(1.5f);

        JudgeWinner();
    }

    public void ChangeState(GameState newState) {
    
        currentState = newState; 
        Debug.Log($"状態が {newState} に変わりました"); 
    
    }

    public void JudgeWinner() {
        Debug.Log("勝敗判定開始...");

        // 現段階の仕様：敵は0点固定
        int enemyScore = 0;

        // プレイヤーが1点でも取っていれば勝ち（敵0点想定）
        bool result = playerFinalScore > enemyScore;

        Debug.Log($"結果: Player({playerFinalScore}) vs CPU({enemyScore})");
        GameEnd(result);

    }

    // ゲーム終了時に呼ぶ
    // win には true(勝ち) / false(負け) が入る
    public void GameEnd(bool win)
    {
        // 受け取った勝敗を保存
        isWin = win;

        Debug.Log(win ? "勝利！Resultシーンへ" : "敗北...Resultシーンへ");
        // 結果画面へ移動
        SceneManager.LoadScene("Result");
    }
}
