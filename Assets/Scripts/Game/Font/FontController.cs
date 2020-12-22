using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FontController : MonoBehaviour
{
    [System.Serializable]
    public struct PlayData
    {
        public Vector3 pos;
        public Vector3 scale;
    }
    
    [SerializeField]
    private Animator m_animator = null;

    [SerializeField]
    private PlayData[] m_playDataList = null;

    private readonly int[] m_hashs =
    {
        Animator.StringToHash("000"),
        Animator.StringToHash("001")
    };

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play(int index)
    {
        gameObject.SetActive(true);
        transform.localPosition = m_playDataList[index].pos;
        transform.localScale = m_playDataList[index].scale;
        m_animator.SetTrigger(m_hashs[index]);
    }
}
