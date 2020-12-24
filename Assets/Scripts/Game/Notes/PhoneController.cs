using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneController : MonoBehaviour
{
    public enum eType
    {
        None = 0,
        Left,
        Right,
        Max
    }

    [SerializeField]
    private Sprite[] m_sprites = null;

    [SerializeField]
    private SpriteRenderer m_spriteRenderer = null;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    public void Play(eType type)
    {
        gameObject.SetActive(type != eType.None);
        m_spriteRenderer.sprite = m_sprites[(int)type];
    }
}
