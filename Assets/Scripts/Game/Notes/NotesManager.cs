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
    private List<NotesController> m_notesList = new List<NotesController>();

    public List<NotesController> NotesList => m_notesList;

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

    public void Ready(float moveTime, System.Action callback)
    {
        gameObject.SetActive(true);
        m_notesList[0].Ready(Random.Range(0, m_randMax), (PhoneController.eType)Random.Range(0, (int)PhoneController.eType.Max));
        m_notesList[1].Ready(Random.Range(0, m_randMax), (PhoneController.eType)Random.Range(0, (int)PhoneController.eType.Max));
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

    public void Open()
    {
        NotesController openNotes = m_notesList.First(notes => notes.IsStandBy());
        openNotes.Play();
    }

    public bool Opening()
    {
        return m_notesList.Any(v => v.IsPlay());
    }

    public bool IsOpen()
    {
        return m_notesList.Any(v => v.IsOpen());
    }

    public void Play(float time, System.Action destroyCallbak)
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
                    newNotes.Ready(Random.Range(0, m_randMax), (PhoneController.eType)Random.Range(0, (int)PhoneController.eType.Max));
                    disposable.Dispose();
                });
            });
            readyNotes.StartMove(time);
        }
    }

    public bool IsPhoneHit(Vector3 vec)
    {
        NotesController notes = m_notesList.FirstOrDefault(v => v.IsStandBy());
        if (notes)
        {
            return notes.IsPhoneHit(vec);
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
}
