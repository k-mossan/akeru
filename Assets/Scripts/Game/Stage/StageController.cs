using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

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

    private NotesManager m_notesManager = null;
    private TempoManager m_tempoManager = null;

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
    }

    // Start is called before the first frame update
    void Start()
    {
        this.UpdateAsObservable().Where(_ => Input.GetKeyDown(KeyCode.A)).Subscribe(_ =>
        {
            var disposable = new SingleAssignmentDisposable();
            disposable.Disposable = this.UpdateAsObservable().Where(a => m_notesManager.NotesList.Any(v => v.IsStandBy())).Subscribe(b =>
            {
                var notes = m_notesManager.NotesList.First(v => v.IsStandBy());
                notes.Play();
                disposable.Dispose();
            });
        });
        m_tempoManager.Play();

        int prevCounter = m_tempoManager.TanCounter;
        this.UpdateAsObservable().Where(a => m_tempoManager.TanCounter > prevCounter).Subscribe(b =>
        {
            NotesController openNotes = m_notesManager.NotesList.FirstOrDefault(v => v.IsOpen());
            if (openNotes)
            {
                openNotes.EndMove(m_tempoManager.TempoTime);
            }
            int readyCount = m_notesManager.NotesList.Count(notes => notes.IsReady());
            if (readyCount < 2)
            {
                for (int i = 0; i < 2 - readyCount; ++i)
                {
                    NotesController notes = m_notesManager.NotesList.FirstOrDefault(v => v.IsHide());
                    if (notes)
                    {
                        notes.Ready(Random.Range(0, 2));
                    }
                }
            }
            readyCount = m_notesManager.NotesList.Count(notes => notes.IsReady());
            if (readyCount > 0 && !m_notesManager.NotesList.Any(notes => notes.IsStandBy() || notes.IsStartMove() || notes.IsPlay() || (notes.IsOpen())))
            {
                NotesController notes = m_notesManager.NotesList.First(v => v.IsReady());
                notes.StartMove(m_tempoManager.TempoTime);
            }
            prevCounter = m_tempoManager.TanCounter;
        });
    }
}
