using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    [SerializeField] private Image ImgTitle;// 背景

    [SerializeField] private AudioClip BGMTitle;// BGM
    [SerializeField] private AudioClip clickSE;//クリック音
    //音楽再生に必要
    private AudioSource audioSource;

    //PlayNowをゆらす
    [SerializeField] private Image ImgPlayNow;
    [SerializeField] private float amplitude = 10f; // 揺れ幅
    [SerializeField] private float moveSpeed = 2f;  // 揺れる速さ
    [SerializeField] private float fadeSpeed = 2f;  // 点滅速さ
    private Vector3 startPos;//imageの初期位置を取得するため

    //シーン移行
    [SerializeField] private string nextSceneName = "SelectLevel";

    //開始前に用意するもの
    private void Awake()
    {

        // AudioSource を確実に用意
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;

        startPos = ImgPlayNow.rectTransform.localPosition;//imageの初期位置を取得する

    }

    // Start is called before the first frame update
    void Start()
    {

        StartCoroutine(FadeInAndBGM());
    }

    // 画像フェードイン + BGM開始
    IEnumerator FadeInAndBGM()
    {
        Color c = ImgTitle.color;
        c.a = 0;
        ImgTitle.color = c;

        while (c.a < 1f)
        {
            c.a += Time.deltaTime; // 1秒でフェードイン
            ImgTitle.color = c;
            yield return null;
        }

        audioSource.clip = BGMTitle;  // ← bgmClipではなくBGMTitle
        audioSource.loop = true;
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        // 上下ゆらゆら
        float offset = Mathf.Sin(Time.time * moveSpeed) * amplitude;
        ImgPlayNow.rectTransform.localPosition = startPos + new Vector3(0, offset, 0);

        // 点滅（アルファ値）
        float alpha = (Mathf.Sin(Time.time * fadeSpeed) + 1f) / 2f;
        Color c = ImgPlayNow.color;
        c.a = alpha;
        ImgPlayNow.color = c;

        //クリックしたときにSEがなって、シーン遷移する
        if (Input.GetMouseButtonDown(0)) // クリック時
        {
            audioSource.PlayOneShot(clickSE);

            // SEが鳴り終わるタイミングまで待ってからシーン遷移
            Invoke(nameof(GoNextScene), 0.2f);
        }


    }

    //シーン移行
    void GoNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }


}
