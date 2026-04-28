using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum GameState
{
    TurnStart,  // ターン開始（currentTurnを増やす準備）
    Rolling,    // プレイヤーがサイコロを振っている
    Selecting,  // 役（スコア）を選んでいる
    EnemyTurn,  // 敵のターンの処理中
    GameResult  // 全ターン終了、勝敗判定
}