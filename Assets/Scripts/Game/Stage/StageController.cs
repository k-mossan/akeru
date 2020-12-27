using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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
    private GameObject m_tutorialPrefab = null;

    [SerializeField]
    private Transform m_tutorialRoot = null;

    [SerializeField]
    private SpriteRenderer m_fontSpriteRenderer = null;

    [SerializeField]
    private Sprite[] m_fontSpriteList = null;

    [SerializeField]
    private StageDataManager[] m_stageDataList = null;

    [SerializeField]
    private GameObject m_resultNumberPrefab = null;

    [SerializeField]
    private Transform m_resultScoreRoot = null;

    [SerializeField]
    private Transform m_resultBonusRoot = null;

    [SerializeField]
    private Transform m_resultTotalRoot = null;

    private NotesManager m_notesManager = null;
    private TempoManager m_tempoManager = null;
    private TutorialController m_tutorialController = null;
    private NumberManager m_resultNumberScore = null;
    private NumberManager m_resultNumberBonus = null;
    private NumberManager m_resultNumberTotal = null;
    private int m_currentStageNo = 0;
    private int m_noteNum = 0;
    private int m_openCount = -1;
    private int m_phoneCount = -1;
    private int m_totalScore = 0;
    private int m_turnMax = 0;

    public StageDataManager[] StageDataList => m_stageDataList;

    public void Init(int stageNo)
    {
        m_currentStageNo = stageNo;
    }

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

        obj = GameObject.Instantiate(m_tutorialPrefab, m_tutorialRoot);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.Euler(Vector3.zero);
        obj.transform.localScale = Vector3.one;
        m_tutorialController = obj.GetComponent<TutorialController>();

        CreateResult();

        m_turnMax = 1;
        for (int i = 0; i < m_stageDataList[m_currentStageNo].Datas.Length; ++i)
        {
            m_turnMax += 2 + m_stageDataList[m_currentStageNo].Datas[i].PhoneMax;
            m_turnMax += m_stageDataList[m_currentStageNo].Datas[i].PhoneMax > 0 ? 1 : 0;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        var disposable = new SingleAssignmentDisposable();
        disposable.Disposable = this.UpdateAsObservable().Where(_ => this.m_notesManager.gameObject.activeSelf).Subscribe(_ =>
        {
            m_notesManager.Ready(m_stageDataList[m_currentStageNo], m_tempoManager.TempoTime, () =>
            {
                StartCoroutine(coStart());
            });
            disposable.Dispose();
        });
    }

    private IEnumerator coStart()
    {
        yield return null;
        GameManager.Instance.TotalScore.Play(0);
        GameManager.Instance.Count.Play(0);
        m_fontSpriteRenderer.sprite = m_fontSpriteList[0];
        yield return new WaitForSeconds(2.0f);
        m_fontSpriteRenderer.sprite = m_fontSpriteList[1];
        yield return new WaitForSeconds(1.0f);
        m_fontSpriteRenderer.gameObject.SetActive(false);
        GameStart();
    }

    private IEnumerator coEnd()
    {
        int turnNum = m_tempoManager.TanCounter - m_turnMax;
        turnNum = turnNum > m_turnMax ? m_turnMax : turnNum;
        float bonusRate = 1.0f - (float)turnNum / (float)m_turnMax;
        int bonusMul = (int)(bonusRate * 9.0f) + 1;
        yield return null;
        m_tempoManager.SetPause(true);
        yield return new WaitForSeconds(1.0f);
        m_fontSpriteRenderer.gameObject.SetActive(true);
        m_fontSpriteRenderer.sprite = m_fontSpriteList[2];
        yield return new WaitForSeconds(2.0f);
        m_fontSpriteRenderer.sprite = m_fontSpriteList[4];
        yield return new WaitForSeconds(1.0f);
        m_resultNumberScore.Play(m_totalScore);
        yield return new WaitForSeconds(1.0f);
        m_resultNumberBonus.Play(bonusMul * 100);
        yield return new WaitForSeconds(1.0f);
        m_resultNumberTotal.Play(m_totalScore * bonusMul);
        yield return new WaitForSeconds(4.0f);
        UIGame.Instance.FadeOut();
        yield return new WaitForSeconds(1.0f);
        GameManager.Instance.NextStage();
    }

    private void CreateResult()
    {
        GameObject obj = GameObject.Instantiate(m_resultNumberPrefab, m_resultScoreRoot);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.Euler(Vector3.zero);
        obj.transform.localScale = Vector3.one;
        m_resultNumberScore = obj.GetComponent<NumberManager>();

        obj = GameObject.Instantiate(m_resultNumberPrefab, m_resultBonusRoot);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.Euler(Vector3.zero);
        obj.transform.localScale = Vector3.one;
        m_resultNumberBonus = obj.GetComponent<NumberManager>();

        obj = GameObject.Instantiate(m_resultNumberPrefab, m_resultTotalRoot);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.Euler(Vector3.zero);
        obj.transform.localScale = Vector3.one;
        m_resultNumberTotal = obj.GetComponent<NumberManager>();
    }

    private void GameStart()
    {
        ReadyNotes();
        m_tempoManager.Play();
        int prevCounter = m_tempoManager.TanCounter;
        this.UpdateAsObservable().Where(a => m_tempoManager.TanCounter > prevCounter).Subscribe(b =>
        {
            GameManager.Instance.Count.Play(m_tempoManager.TanCounter);
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
            else if (m_tutorialController && m_tutorialController.IsPlay(m_currentStageNo, m_noteNum))
            {
                m_tutorialController.Play(m_currentStageNo, m_noteNum);
                m_tempoManager.SetPause(true);
            }
            if (!m_tempoManager.PauseFlag)
            {
                prevCounter = m_tempoManager.TanCounter;
            }
        });
        //this.UpdateAsObservable().Where(_ => Input.GetMouseButton(0) && CheckTutorialControl(TutorialData.eType.LeftDoor)).Select(_ => Input.mousePosition).Subscribe(v =>
        //{
        //    CheckPlayPhone(v, KeyCode.None);
        //});
        this.UpdateAsObservable().Where(_ => Input.GetKeyDown(KeyCode.Q) && CheckTutorialControl(TutorialData.eType.RightPhone)).Select(_ => Input.mousePosition).Subscribe(v =>
        {
            m_tutorialController.Hide();
            m_tempoManager.SetPause(false);
            CheckPlayPhone(v, KeyCode.Q);
        });
        this.UpdateAsObservable().Where(_ => Input.GetKeyDown(KeyCode.W) && CheckTutorialControl(TutorialData.eType.LeftPhone)).Select(_ => Input.mousePosition).Subscribe(v =>
        {
            m_tutorialController.Hide();
            m_tempoManager.SetPause(false);
            CheckPlayPhone(v, KeyCode.W);
        });
    }

    private bool CheckTutorialControl(TutorialData.eType type)
    {
        TutorialData.eType getType = m_tutorialController ? m_tutorialController.GetType(m_currentStageNo, m_noteNum) : TutorialData.eType.None;
        return (!m_tutorialController
            || !m_tutorialController.IsPlay(m_currentStageNo, m_noteNum)
            || (m_tempoManager.PauseFlag && (getType == type || getType == TutorialData.eType.None)));
    }

    private void CheckPlayPhone(Vector3 vec, KeyCode keyCode)
    {
        if (m_notesManager.IsPhoneHit(vec, keyCode))
        {
            if (m_notesManager.IsLock())
            {
                AddScore();
            }
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
                if (Input.GetKeyDown(KeyCode.A) && (CheckTutorialControl(TutorialData.eType.LeftDoor) ||  CheckTutorialControl(TutorialData.eType.RightPhone) || CheckTutorialControl(TutorialData.eType.LeftPhone)))
                {
                    disposable.Dispose();
                    if (callback != null)
                    {
                        callback(KeyCode.A);
                        m_tempoManager.SetPause(false);
                    }
                }
                else if (Input.GetKeyDown(KeyCode.S) && (CheckTutorialControl(TutorialData.eType.RightDoor) || CheckTutorialControl(TutorialData.eType.RightPhone) || CheckTutorialControl(TutorialData.eType.LeftPhone)))
                {
                    disposable.Dispose();
                    if (callback != null)
                    {
                        m_tempoManager.SetPause(false);
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
                m_tempoManager.SetPause(false);
                m_tutorialController.Hide();
                m_noteNum++;
                AddScore();
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
                if (m_noteNum < m_notesManager.StageDataManager.Datas.Length)
                {
                    ReadyNotes();
                }
                else
                {
                    StartCoroutine(coEnd());
                }
            }, null);
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

    private void AddScore()
    {
        float rate = m_tempoManager.GetScoreRate();
        int score = (int)(100 * rate) + 1;
        m_totalScore += score;
        GameManager.Instance.TotalScore.Play(m_totalScore);
        GameManager.Instance.Score.Play(score);
        GameManager.Instance.Score.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        GameManager.Instance.Score.transform.DOScale(4.0f * rate + 1.0f, 0.2f).OnComplete(() =>
        {
            this.transform.DOScale(this.transform.localScale, 0.5f).OnComplete(() =>
            {
                GameManager.Instance.Score.Hide();
            });
        });
    }
}
