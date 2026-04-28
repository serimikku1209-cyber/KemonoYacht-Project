using System.Collections; 
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectLevel : MonoBehaviour
{
    public static string difficulty;

    [Header("音の設定")]
    [SerializeField] private AudioClip clickSE;
    [SerializeField] private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void SelectEasy()
    {
        difficulty = "EASY";
        // コルーチンをスタートさせる書き方に変える
        StartCoroutine(PlayAndLoadCoroutine());
    }

    public void SelectNormal()
    {
        difficulty = "NORMAL";
        // コルーチンをスタートさせる書き方に変える
        StartCoroutine(PlayAndLoadCoroutine());
    }

    // IEnumerator にして、名前もコルーチンっぽく変更
    private IEnumerator PlayAndLoadCoroutine()
    {
        // 1. 音を鳴らす
        if (audioSource != null && clickSE != null)
        {
            audioSource.PlayOneShot(clickSE);
        }

        // 2. 0.5秒待つ（ここでお好みの秒数を指定！）
        yield return new WaitForSeconds(0.2f);

        // 3. 待った後にシーン移動
        SceneManager.LoadScene("Match");
    }
}