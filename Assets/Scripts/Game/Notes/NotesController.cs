using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using DG.Tweening;

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

    [SerializeField]
    private GameObject m_phonePrefab = null;

    [SerializeField]
    private Transform m_phoneRoot = null;

    public readonly float m_maxZ = 50;
    private readonly int m_doorMax = 3;
    private readonly int m_fontMax = 3;
    private readonly int[] m_fontIndexes =
    {
        0,
        1,
        0,
        1
    };

    private DoorController[] m_doorList = null;
    private FontController[] m_fontList = null;
    private PhoneController m_phone = null;
    private int m_index = 0;

    public int Index => m_index;

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
        {
            GameObject obj = GameObject.Instantiate(m_phonePrefab, m_phoneRoot);
            obj.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
            obj.transform.localRotation = Quaternion.Euler(Vector3.zero);
            obj.transform.localScale = Vector3.one;
            m_phone = obj.GetComponent<PhoneController>();
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

    public bool IsHide()
    {
        return !gameObject.activeSelf;
    }

    public bool IsReady()
    {
        return m_doorList[0].IsReady(m_index);
    }

    public void Ready(int index, PhoneController.eType phoneType)
    {
        Vector3 pos = transform.localPosition;
        pos.z = m_maxZ;
        transform.localPosition = pos;

        gameObject.SetActive(true);
        m_doorList[0].Ready(index);
        m_phone.Play(phoneType);
        m_index = index;
    }

    public bool IsStartMove()
    {
        Vector3 pos = transform.localPosition;
        return pos.z < m_maxZ && pos.z > 0.0f;
    }

    public void StartMove(float time)
    {
        transform.DOLocalMoveZ(0.0f, time).SetEase(Ease.OutQuad);
    }

    public bool IsEndMove()
    {
        Vector3 pos = transform.localPosition;
        return pos.z < 0.0f;
    }

    public void EndMove(float time)
    {
        Vector3 pos = transform.localPosition;
        pos.z = -0.01f;
        transform.localPosition = pos;
        transform.DOLocalMoveZ(-10.0f, time).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }

    public bool IsStandBy()
    {
        return IsReady() && transform.localPosition.z < 0.01f;
    }

    public bool IsPlay()
    {
        return m_doorList[0].IsPlay(m_index);
    }

    public void Play()
    {
        m_doorList[0].Play(m_index, null);
        m_fontList[0].Play(m_fontIndexes[m_index]);
    }

    public bool IsOpen()
    {
        return m_doorList[0].IsOpen(m_index) && !IsEndMove() && !IsHide();
    }

    public bool IsPhoneHit(Vector3 vec)
    {
        var pos = Camera.main.ScreenToWorldPoint(vec);
        RaycastHit2D[] hits = Physics2D.RaycastAll(pos, Vector2.zero);
        if (hits != null)
        {
            return true;
        }
        return false;
    }

    public void PlayPhone()
    {
        FontController font = m_fontList.FirstOrDefault(v => !v.gameObject.activeSelf);
        if (font)
        {
            font.Play(3);
        }
    }
}
