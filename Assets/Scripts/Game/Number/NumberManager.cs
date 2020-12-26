using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberManager : MonoBehaviour
{
    [SerializeField]
    private GameObject m_numberPrefab = null;

    [SerializeField]
    private Transform m_numberRoot = null;

    private readonly int m_maxKeta = 10;
    private NumberController[] m_numberControllers = null;

    private void Awake()
    {
        m_numberControllers = new NumberController[m_maxKeta];
        for (int i = 0; i < m_maxKeta; ++i)
        {
            GameObject obj = GameObject.Instantiate(m_numberPrefab, m_numberRoot);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.Euler(Vector3.zero);
            obj.transform.localScale = Vector3.one;
            m_numberControllers[i] = obj.GetComponent<NumberController>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    public void Play(int num)
    {
        gameObject.SetActive(true);
        for (int i = 0; i < m_maxKeta; ++i)
        {
            m_numberControllers[i].gameObject.SetActive(false);
        }
        int keta = 0;
        while (true)
        {
            m_numberControllers[keta].gameObject.SetActive(true);
            m_numberControllers[keta].SetNumber(num % 10);
            ++keta;
            if (num < 10)
            {
                break;
            }
            num /= 10;
        }
        float width = 0.5f;
        float totalWidth = keta * width;
        float halfPos = totalWidth / 2.0f;
        float tyosei = width / 2.0f;// (keta % 2) == 1 ? width / 2.0f : width;
        for (int i = 0; i < keta; ++i)
        {
            Vector3 pos = m_numberControllers[i].transform.localPosition;
            pos.x = (halfPos - width * i) - tyosei;
            m_numberControllers[i].transform.localPosition = pos;
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
