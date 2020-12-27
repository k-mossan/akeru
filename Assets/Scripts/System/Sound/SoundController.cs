using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    public enum eType
    {
        BISI = 0,
        BASI,
        PINPON,
        YES,
        TAN,
    }

    [SerializeField]
    private AudioClip[] m_audioClipList = null;

    [SerializeField]
    private AudioSource m_audioSource = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play(eType type)
    {
        m_audioSource.clip = m_audioClipList[(int)type];
        m_audioSource.Play();
    }

    public bool IsPlay()
    {
        return m_audioSource.isPlaying;
    }
}
