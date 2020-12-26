using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class NotesManager : MonoBehaviour
{
    [SerializeField]
    private GameObject m_notesPrefab = null;

    [SerializeField]
    private Transform m_notesRoot = null;

    private readonly int m_notesMax = 3;
    private readonly int m_randMax = 2;
    private readonly int m_phoneMax = 3;
    private List<NotesController> m_notesList = new List<NotesController>();
    private StageDataManager m_stageDataManager = null;
    private int m_stageIndex = 0;

    public List<NotesController> NotesList => m_notesList;
    public StageDataManager StageDataManager => m_stageDataManager;

    private void Awake()
    {
        CreateNotes();
        CreateNotes();
    }

    public NotesController CreateNotes()
    {
        GameObject obj = GameObject.Instantiate(m_notesPrefab, m_notesRoot);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.Euler(Vector3.zero);
        obj.transform.localScale = Vector3.one;
        NotesController notes = obj.GetComponent<NotesController>();
        m_notesList.Add(notes);
        return notes;
    }

    // Start is called before the first frame update
    void Start()
    {
        this.UpdateAsObservable().Where(_ => m_notesList.All(notes => notes.IsHide())).Subscribe(_ =>
        {
            gameObject.SetActive(false);
        });
    }

    public void Ready(StageDataManager stageDataManager, float moveTime, System.Action callback)
    {
        m_stageDataManager = stageDataManager;

        gameObject.SetActive(true);
        StageData[] stageList = m_stageDataManager.Datas;
        m_notesList[0].Ready(stageList[m_stageIndex].NotesIndex, stageList[m_stageIndex].PhoneType, stageList[m_stageIndex].PhoneMax);
        m_stageIndex++;
        m_notesList[1].Ready(stageList[m_stageIndex].NotesIndex, stageList[m_stageIndex].PhoneType, stageList[m_stageIndex].PhoneMax);
        m_stageIndex++;
        var firstDisposable = new SingleAssignmentDisposable();
        firstDisposable.Disposable = this.UpdateAsObservable().Where(_ => m_notesList[0].IsReady()).Subscribe(_ =>
        {
            m_notesList[0].StartMove(moveTime);
            firstDisposable.Dispose();
        });
        var secondDisposable = new SingleAssignmentDisposable();
        secondDisposable.Disposable = this.UpdateAsObservable().Where(_ => m_notesList[0].IsStandBy()).Subscribe(_ =>
        {
            if (callback != null)
            {
                callback();
            }
            secondDisposable.Dispose();
        });
    }

    public bool IsStanBy()
    {
        return m_notesList.Any(notes => notes.IsStandBy());
    }

    public bool IsLock()
    {
        return m_notesList.Any(v => v.IsLock());
    }

    public bool Open(KeyCode keyCode)
    {
        NotesController openNotes = m_notesList.First(notes => notes.IsStandBy());
        if (openNotes.CallFlag || openNotes.PhoneMax == 0)
        {
            if (openNotes.Index == 0 && keyCode == KeyCode.A || openNotes.Index == 1 && keyCode == KeyCode.S)
            {
                openNotes.Play();
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (openNotes.Index == 0 && keyCode == KeyCode.A || openNotes.Index == 1 && keyCode == KeyCode.S)
            {
                if (keyCode == KeyCode.A)
                {
                    openNotes.LockLeft();
                }
                else if (keyCode == KeyCode.S)
                {
                    openNotes.LockRight();
                }
            }
            return false;
        }
        return true;
    }

    public bool Opening()
    {
        return m_notesList.Any(v => v.IsPlay());
    }

    public bool IsOpen()
    {
        return m_notesList.Any(v => v.IsOpen());
    }

    public void Play(float time, System.Action destroyCallbak, System.Action endCallback)
    {
        NotesController openNotes = m_notesList.FirstOrDefault(v => v.IsPlay() || v.IsOpen());
        if (openNotes)
        {
            var disposable = new SingleAssignmentDisposable();
            disposable.Disposable = this.UpdateAsObservable().Where(_ => openNotes.IsHide()).Subscribe(_ =>
            {
                m_notesList.Remove(openNotes);
                Destroy(openNotes.gameObject);
                openNotes = null;
                disposable.Dispose();
                if (destroyCallbak != null)
                {
                    destroyCallbak();
                }
            });
            openNotes.EndMove(time);
        }
        NotesController readyNotes = m_notesList.FirstOrDefault(v => v.IsReady());
        if (readyNotes)
        {
            var disposable = new SingleAssignmentDisposable();
            disposable.Disposable = this.UpdateAsObservable().Where(a => readyNotes.IsStandBy()).Subscribe(a =>
            {
                NotesController newNotes = CreateNotes();
                disposable.Dispose();
                disposable = new SingleAssignmentDisposable();
                disposable.Disposable = this.UpdateAsObservable().Where(b => newNotes.IsHide()).Subscribe(b =>
                {
                    if (m_stageIndex < m_stageDataManager.Datas.Length)
                    {
                        StageData data = m_stageDataManager.Datas[m_stageIndex];
                        newNotes.Ready(data.NotesIndex, data.PhoneType, data.PhoneMax);
                        m_stageIndex++;
                    }
                    else if (endCallback != null)
                    {
                        endCallback();
                    }
                    disposable.Dispose();
                });
            });
            readyNotes.StartMove(time);
        }
    }

    public bool IsPhoneHit(Vector3 vec, KeyCode keyCode)
    {
        NotesController notes = m_notesList.FirstOrDefault(v => v.IsStandBy());
        if (notes)
        {
            return notes.IsPhoneHit(vec, keyCode);
        }
        return false;
    }

    public void PlayPhone()
    {
        NotesController notes = m_notesList.FirstOrDefault(v => v.IsStandBy());
        if (notes)
        {
            notes.PlayPhone();
        }
    }

    public bool IsPlayPhone()
    {
        return m_notesList.Any(v => v.PhoneFlag);
    }

    public void ClearPhoneFlag()
    {
        m_notesList.ForEach(v => v.ClearPhoneFlag());
    }

    public void PlayCall()
    {
        NotesController notes = m_notesList.FirstOrDefault(v => v.PhoneFlag);
        if (notes)
        {
            notes.PlayCall();
        }
    }
}
