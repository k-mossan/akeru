using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using UniRx.Triggers;
using DG.Tweening;

public class StageController : MonoBehaviour
{
    [SerializeField]
    private GameObject m_notesManagerPrefab = null;

    [SerializeField]
    private Transform m_notesManagerRoot = null;

    [SerializeField]
    private GameObject m_tempoManagerPrefab = null;

    [SerializeField]
    private Transform m_tempoManagerRoot = null;

    [SerializeField]
    private SpriteRenderer m_fontSpriteRenderer = null;

    [SerializeField]
    private Sprite[] m_fontSpriteList = null;

    private StageDataManager m_stageDataManager = null;
    private NotesManager m_notesManager = null;
    private TempoManager m_tempoManager = null;
    private int m_openCount = -1;
    private int m_phoneCount = -1;

    private void Awake()
    {
        GameObject obj = GameObject.Instantiate(m_notesManagerPrefab, m_notesManagerRoot);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.Euler(Vector3.zero);
        obj.transform.localScale = Vector3.one;
        m_notesManager = obj.GetComponent<NotesManager>();

        obj = GameObject.Instantiate(m_tempoManagerPrefab, m_tempoManagerRoot);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.Euler(Vector3.zero);
        obj.transform.localScale = Vector3.one;
        m_tempoManager = obj.GetComponent<TempoManager>();

        m_stageDataManager = new StageDataManager(0);
    }

    // Start is called before the first frame update
    void Start()
    {
        var disposable = new SingleAssignmentDisposable();
        disposable.Disposable = this.UpdateAsObservable().Where(_ => this.m_notesManager.gameObject.activeSelf).Subscribe(_ =>
        {
            m_notesManager.Ready(m_stageDataManager, m_tempoManager.TempoTime, () =>
            {
                StartCoroutine(coStart());
            });
            disposable.Dispose();
        });
    }

    private IEnumerator coStart()
    {
        yield return null;
        m_fontSpriteRenderer.sprite = m_fontSpriteList[0];
        yield return new WaitForSeconds(2.0f);
        m_fontSpriteRenderer.sprite = m_fontSpriteList[1];
        yield return new WaitForSeconds(1.0f);
        m_fontSpriteRenderer.gameObject.SetActive(false);
        GameStart();
    }

    private IEnumerator coEnd()
    {
        yield return null;
        yield return new WaitForSeconds(3.0f);
        m_fontSpriteRenderer.gameObject.SetActive(true);
        m_fontSpriteRenderer.sprite = m_fontSpriteList[2];
        yield return new WaitForSeconds(2.0f);
        m_fontSpriteRenderer.gameObject.SetActive(false);
        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene("SampleScene");
    }

    private void GameStart()
    {
        ReadyNotes();
        m_tempoManager.Play();
        int prevCounter = m_tempoManager.TanCounter;
        this.UpdateAsObservable().Where(a => m_tempoManager.TanCounter > prevCounter).Subscribe(b =>
        {
            if (m_notesManager.Opening() || m_notesManager.IsOpen())
            {
                PlayNotes();
            }
            else if (m_notesManager.IsPlayPhone())
            {
                if (IsPlayTiming(m_phoneCount))
                {
                    if (!m_notesManager.IsLock())
                    {
                        m_notesManager.PlayCall();
                    }
                    m_notesManager.ClearPhoneFlag();
                    m_phoneCount = -1;
                }
            }
            prevCounter = m_tempoManager.TanCounter;
        });
        this.UpdateAsObservable().Where(_ => Input.GetMouseButton(0)).Select(_ => Input.mousePosition).Subscribe(v =>
        {
            CheckPlayPhone(v, KeyCode.None);
        });
        this.UpdateAsObservable().Where(_ => Input.GetKeyDown(KeyCode.Q)).Select(_ => Input.mousePosition).Subscribe(v =>
        {
            CheckPlayPhone(v, KeyCode.Q);
        });
        this.UpdateAsObservable().Where(_ => Input.GetKeyDown(KeyCode.E)).Select(_ => Input.mousePosition).Subscribe(v =>
        {
            CheckPlayPhone(v, KeyCode.E);
        });
    }

    private void CheckPlayPhone(Vector3 vec, KeyCode keyCode)
    {
        if (m_notesManager.IsPhoneHit(vec, keyCode))
        {
            m_notesManager.PlayPhone();
            m_phoneCount = m_tempoManager.Counter % 4;
        }
    }

    private void ReadyOpen(System.Action<KeyCode> callback)
    {
        var disposable = new SingleAssignmentDisposable();
        disposable.Disposable = this.UpdateAsObservable().Where(a => m_notesManager.IsStanBy()).Subscribe(a =>
        {
            disposable.Dispose();
            disposable = new SingleAssignmentDisposable();
            disposable.Disposable = this.UpdateAsObservable().Where(b => Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S)).Subscribe(b =>
            {
                disposable.Dispose();
                if (callback != null)
                {
                    if (Input.GetKeyDown(KeyCode.A))
                    {
                        callback(KeyCode.A);
                    }
                    else
                    {
                        callback(KeyCode.S);
                    }
                }
            });
        });
    }

    private void ReadyNotes()
    {
        ReadyOpen((keyCode) =>
        {
            if (m_notesManager.Open(keyCode))
            {
                float rate = m_tempoManager.GetScoreRate();
                GameManager.Instance.Score.Play((int)(100 * rate));
                GameManager.Instance.Score.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                GameManager.Instance.Score.transform.DOScale(4.0f * rate + 1.0f, 0.2f).OnComplete(() =>
                {
                    this.transform.DOScale(this.transform.localScale, 0.5f).OnComplete(() =>
                    {
                        GameManager.Instance.Score.Hide();
                    });
                });
                m_openCount = m_tempoManager.Counter % 4;
            }
            else
            {
                ReadyNotes();
            }
        });
    }

    private void PlayNotes()
    {
        var disposable = new SingleAssignmentDisposable();
        disposable.Disposable = this.UpdateAsObservable().Where(
            a => IsPlayTiming(m_openCount)).Subscribe(a =>
        {
            disposable.Dispose();
            m_notesManager.Play(m_tempoManager.TempoTime, () =>
            {
                ReadyNotes();
            }, () =>
            {
                StartCoroutine(coEnd());
            });
            m_openCount = -1;
        });
    }

    private bool IsPlayTiming(int count)
    {
        return (count == 0 && (m_tempoManager.Counter % 4) == 3)
            || (count == 1 && (m_tempoManager.Counter % 4) == 3)
            || (count == 2 && (m_tempoManager.Counter % 4) == 1)
            || (count == 3 && (m_tempoManager.Counter % 4) == 1);
    }
}
