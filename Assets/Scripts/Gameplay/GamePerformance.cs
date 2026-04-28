using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePerformance : MonoBehaviour
{
    [SerializeField] private Image gameStartImage; // 表示する画像
    [SerializeField] private AudioClip gameStartSE;
    [SerializeField] private AudioClip bgmEasy;
    [SerializeField] private AudioClip bgmNormal;

    private AudioSource audioSource;

    public void Init()
    {
        // AudioSourceの準備
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }


    // ゲーム開始時の全演出を管理するコルーチン
    public IEnumerator PlayStartSequence(string difficulty)
    {
        // 1. SE再生（音量をあげて再生）
        audioSource.PlayOneShot(gameStartSE, 5.0f);

        // 3. 少し待つ（パッと消えるとかっこ悪いので）
        yield return new WaitForSeconds(2.5f);

        // 4. 画像をフェードアウト
        yield return StartCoroutine(FadeImage(1, 0, 0.5f));

        // 5. BGM再生開始
        PlayBGM(difficulty);
    }

    // フェード処理の共通化（アルファ値を start から end へ duration 秒かけて変化させる）
    private IEnumerator FadeImage(float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0;
        Color c = gameStartImage.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            gameStartImage.color = c;
            yield return null;
        }
    }

    private void PlayBGM(string difficulty)
    {
        audioSource.clip = (difficulty == "EASY") ? bgmEasy : bgmNormal;

        audioSource.loop = true;
        audioSource.Play();
    }

}
