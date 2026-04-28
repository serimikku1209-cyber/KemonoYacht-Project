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
        if (SelectLevel.difficulty == "")
        {
            Debug.Log("difficulty空です");
        }
        else
        {
            Debug.Log("difficulty: " + SelectLevel.difficulty);
        }

        StartGame();

    }

    public void StartGame()
    {
        Debug.Log("ゲーム開始！"); // ここにDiceControllerの初期化などを呼ぶ予定 }

        // 演出スクリプトに「この難易度で演出始めて！」と丸投げする
        StartCoroutine(performance.PlayStartSequence(SelectLevel.difficulty));
    }

    void OnButtonClick()
    {
        Debug.Log("コードから紐づけたボタンが押されました！");
    }

    public void NextTurn() { 
        
        currentTurn++; Debug.Log($"ターンが進みました: {currentTurn}"); 
    }

    public void ChangeState(GameState newState) {
    
        currentState = newState; 
        Debug.Log($"状態が {newState} に変わりました"); 
    
    }

    public void JudgeWinner() {
        Debug.Log("勝敗判定を行います");

        //********テスト用ランダムで勝敗送る********
        // 画面クリック（PC）またはタップ（スマホ）
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("clicked");
            // 0か1をランダムで取得（0=負け / 1=勝ち）
            bool randomResult = Random.Range(0, 2) == 1;

            GameEnd(randomResult);
        }
        //********ここまで********
        //********テスト用********

    }

    // ゲーム終了時に呼ぶ
    // win には true(勝ち) / false(負け) が入る
    public void GameEnd(bool win)
    {
        // 受け取った勝敗を保存
        isWin = win;

        // 結果画面へ移動
        SceneManager.LoadScene("Result");
    }
}
