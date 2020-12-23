using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UniRx;
using UniRx.Triggers;

public class TempoManager : MonoBehaviour
{
    [SerializeField]
    private GameObject m_tempoPrefab = null;

    [SerializeField]
    private Transform m_tempoRoot = null;

    [SerializeField]
    private Transform m_barTransform = null;

    private readonly int m_tempoMax = 3;
    private readonly float m_tempoTime = 0.5f;
    private TempoController[] m_tempoList = null;
    private float m_timer = 0.0f;
    private int m_counter = 0;
    private int m_tanCounter = 0;

    public float TempoTime => m_tempoTime;
    public int Counter => m_counter;
    public int TanCounter => m_tanCounter;

    private void Awake()
    {
        m_tempoList = new TempoController[m_tempoMax];
        for (int i = 0; i < m_tempoMax; ++i)
        {
            GameObject obj = GameObject.Instantiate(m_tempoPrefab, m_tempoRoot);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.Euler(Vector3.zero);
            obj.transform.localScale = Vector3.one;
            m_tempoList[i] = obj.GetComponent<TempoController>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Play()
    {
        this.UpdateAsObservable().Subscribe(_ =>
        {
            int amari = 0;
            m_timer += Time.deltaTime;
            if (m_timer > m_tempoTime)
            {
                amari = m_counter % 4;
                if (amari == 0 || amari == 2)
                {
                    if (m_tempoList.Any(tempo => tempo.IsStundBy()))
                    {
                        m_tempoList.First(tempo => tempo.IsStundBy()).Play();
                    }
                    ++m_tanCounter;
                }
                m_timer -= m_tempoTime;
                ++m_counter;
            }
            float y = m_timer * m_timer;
            const float center = 1.0f;
            Vector3 pos = m_barTransform.localPosition;
            amari = m_counter % 4;
            float rate = m_timer / m_tempoTime;
            switch (amari)
            {
                case 0:
                    rate = m_timer / m_tempoTime;
                    pos.x = rate * rate * center;
                    break;
                case 1:
                    rate = 1.0f - m_timer / m_tempoTime;
                    pos.x = center + (1.0f - rate * rate) * center;
                    break;
                case 2:
                    rate = m_timer / m_tempoTime;
                    pos.x = center * 2.0f - rate * rate * center;
                    break;
                case 3:
                    rate = 1.0f - m_timer / m_tempoTime;
                    pos.x = center - (1.0f - rate * rate) * center;
                    break;
            }
            m_barTransform.localPosition = pos;
        });
    }
}
