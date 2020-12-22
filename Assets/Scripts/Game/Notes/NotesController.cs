using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class NotesController : MonoBehaviour
{
    [SerializeField]
    private GameObject m_doorPrefab = null;

    [SerializeField]
    private Transform m_doorRoot = null;

    [SerializeField]
    private GameObject m_fontPrefab = null;

    [SerializeField]
    private Transform m_fontRoot = null;

    private readonly int m_doorMax = 3;
    private readonly int m_fontMax = 3;
    private DoorController[] m_doorList = null;
    private FontController[] m_fontList = null;

    private void Awake()
    {
        m_doorList = new DoorController[m_doorMax];
        m_fontList = new FontController[m_fontMax];
        for (int i = 0; i < m_doorMax; ++i)
        {
            GameObject obj = GameObject.Instantiate(m_doorPrefab, m_doorRoot);
            obj.transform.localPosition = new Vector3(0.0f, 0.0f, -0.1f * (float)(i + 1));
            obj.transform.localRotation = Quaternion.Euler(Vector3.zero);
            obj.transform.localScale = Vector3.one;
            m_doorList[i] = obj.GetComponent<DoorController>();
        }
        for (int i = 0; i < m_fontMax; ++i)
        {
            GameObject obj = GameObject.Instantiate(m_fontPrefab, m_fontRoot);
            obj.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
            obj.transform.localRotation = Quaternion.Euler(Vector3.zero);
            obj.transform.localScale = Vector3.one;
            m_fontList[i] = obj.GetComponent<FontController>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        var disposable = new SingleAssignmentDisposable();
        disposable.Disposable = this.UpdateAsObservable().Where(a => m_doorList.All(door => !door.gameObject.activeSelf)).Subscribe(b =>
        {
            disposable.Dispose();
            disposable = new SingleAssignmentDisposable();
            disposable.Disposable = this.UpdateAsObservable().Where(c => m_fontList.All(font => !font.gameObject.activeSelf)).Subscribe(d =>
            {
                disposable.Dispose();
                gameObject.SetActive(false);
            });
        });
    }

    public void Ready(int index)
    {
        gameObject.SetActive(true);
        m_doorList[0].Ready(index);
    }

    public void Play(int index)
    {
        m_doorList[0].Play(() =>
        {
            m_fontList[0].Play(index);
        });
    }
}
