using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempoController : MonoBehaviour
{
    [SerializeField]
    private GameObject m_fontPrefab = null;

    [SerializeField]
    private Transform m_fontRoot = null;

    private FontController m_font = null;

    private void Awake()
    {
        GameObject obj = GameObject.Instantiate(m_fontPrefab, m_fontRoot);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.Euler(Vector3.zero);
        obj.transform.localScale = Vector3.one;
        m_font = obj.GetComponent<FontController>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    public void Play()
    {
        m_font.Play(2);
    }

    public bool IsStundBy()
    {
        return !m_font.gameObject.activeSelf;
    }
}
