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
        // Esc �L�[�������ꂽ���ǂ������m�F
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // �Q�[�����I������
            QuitGame();
        }
    }

    // �Q�[�����I�����郁�\�b�h
    private void QuitGame()
    {
#if UNITY_EDITOR
        // �G�f�B�^�[���Ŏ��s����Ă���ꍇ�� Play ���[�h���~����
        UnityEditor.EditorApplication.isPlaying = false;
#else
            // �G�f�B�^�[�O�Ŏ��s����Ă���ꍇ�̓A�v���P�[�V�������I������
            Application.Quit();
#endif
    }
}
