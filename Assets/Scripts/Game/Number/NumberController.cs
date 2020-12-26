using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberController : MonoBehaviour
{
    [SerializeField]
    private Sprite[] m_sprites = null;

    [SerializeField]
    private SpriteRenderer m_spriteRenderer = null;

    public void SetNumber(int num)
    {
        m_spriteRenderer.sprite = m_sprites[num];
    }
}
