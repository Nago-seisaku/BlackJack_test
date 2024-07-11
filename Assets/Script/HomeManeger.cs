using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeManeger : MonoBehaviour
{
    public void OnButtonDown_BlackJack()
    {
        SceneManager.LoadScene("BlackJack");
    }
    void Update()
    {
        // Esc キーが押されたかどうかを確認
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // ゲームを終了する
            QuitGame();
        }
    }

    // ゲームを終了するメソッド
    private void QuitGame()
    {
#if UNITY_EDITOR
        // エディター内で実行されている場合は Play モードを停止する
        UnityEditor.EditorApplication.isPlaying = false;
#else
            // エディター外で実行されている場合はアプリケーションを終了する
            Application.Quit();
#endif
    }
}
