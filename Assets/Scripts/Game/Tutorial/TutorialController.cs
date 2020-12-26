using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    [SerializeField]
    private TutorialData m_data = null;

    [SerializeField]
    private SpriteRenderer m_spriteRenderer = null;

    public bool Play(int stage, int notes)
    {
        gameObject.SetActive(true);
        Sprite sprite = m_data.GetSprite(stage, notes);
        if (sprite)
        {
            m_spriteRenderer.sprite = sprite;
            return true;
        }
        return false;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public bool IsPlay(int stage, int notes)
    {
        Sprite sprite = m_data.GetSprite(stage, notes);
        if (sprite)
        {
            return true;
        }
        return false;
    }

    public TutorialData.eType GetType(int stage, int notes)
    {
        return m_data.GetType(stage, notes);
    }
}
