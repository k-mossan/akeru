using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UIGame : UISystem<UIGame>
{
    [SerializeField]
    private UnityEngine.UI.Image m_fadeImage = null;

    // Start is called before the first frame update
    void Start()
    {
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.5f).Append(DOTween.ToAlpha(() => m_fadeImage.color, color => m_fadeImage.color = color, 0, 1.0f)).AppendCallback(() =>
        {
            m_fadeImage.gameObject.SetActive(false);
        });
    }
}
