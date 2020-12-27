using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundManager : SingletonMonoBehaviour<SoundManager>
{
    [SerializeField]
    private GameObject m_soundPrefab = null;

    [SerializeField]
    private Transform m_soundRoot = null;

    [SerializeField]
    private int m_soundNum = 10;

    private SoundController[] m_soundControllerList = null;

    private void Start()
    {
        m_soundControllerList = new SoundController[m_soundNum];
        for (int i = 0; i < m_soundNum; ++i)
        {
            GameObject obj = GameObject.Instantiate(m_soundPrefab, m_soundRoot);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.Euler(Vector3.zero);
            obj.transform.localScale = Vector3.one;
            m_soundControllerList[i] = obj.GetComponent<SoundController>();
        }
    }

    public void Play(SoundController.eType type)
    {
        SoundController sound = m_soundControllerList.FirstOrDefault(s => !s.IsPlay());
        if (sound)
        {
            sound.Play(type);
        }
    }
}
