using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class TitleScene : MonoBehaviour
{
    [SerializeField]
    private UnityEngine.UI.Image m_fadeImage = null;

    private bool m_spaceFlag = false;

    // Start is called before the first frame update
    void Start()
    {
        Sequence seq = DOTween.Sequence();
        m_fadeImage.gameObject.SetActive(true);
        seq.AppendInterval(0.5f).Append(DOTween.ToAlpha(() => m_fadeImage.color, color => m_fadeImage.color = color, 0, 1.0f)).AppendCallback(() =>
        {
            m_fadeImage.gameObject.SetActive(false);
            m_spaceFlag = true;
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (m_spaceFlag)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_spaceFlag = false;
                Sequence seq = DOTween.Sequence();
                m_fadeImage.gameObject.SetActive(true);
                seq.AppendInterval(0.5f).Append(DOTween.ToAlpha(() => m_fadeImage.color, color => m_fadeImage.color = color, 1, 1.0f)).AppendCallback(() =>
                {
                    SceneManager.LoadScene("SampleScene");
                });
            }
        }
    }
}
