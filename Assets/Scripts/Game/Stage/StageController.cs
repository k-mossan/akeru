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
    private int m_openCount = 0;

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
        var disposable = new SingleAssignmentDisposable();
        disposable.Disposable = this.UpdateAsObservable().Where(_ => this.m_notesManager.gameObject.activeSelf).Subscribe(_ =>
        {
            m_notesManager.Ready(m_tempoManager.TempoTime, () =>
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
                    prevCounter = m_tempoManager.TanCounter;
                });
            });
            disposable.Dispose();
        });
    }

    private void ReadyNotes()
    {
        var disposable = new SingleAssignmentDisposable();
        disposable.Disposable = this.UpdateAsObservable().Where(a => m_notesManager.IsStanBy()).Subscribe(a =>
        {
            disposable.Dispose();
            disposable = new SingleAssignmentDisposable();
            disposable.Disposable = this.UpdateAsObservable().Where(b => Input.GetKeyDown(KeyCode.A)).Subscribe(b =>
            {
                m_notesManager.Open();
                m_openCount = m_tempoManager.Counter % 4;
                disposable.Dispose();
            });
        });
    }

    private void PlayNotes()
    {
        var disposable = new SingleAssignmentDisposable();
        disposable.Disposable = this.UpdateAsObservable().Where(
            a => (m_openCount == 0 && (m_tempoManager.Counter % 4) == 3)
            || (m_openCount == 1 && (m_tempoManager.Counter % 4) == 3)
            || (m_openCount == 2 && (m_tempoManager.Counter % 4) == 1)
            || (m_openCount == 3 && (m_tempoManager.Counter % 4) == 1)).Subscribe(a =>
        {
            m_notesManager.Play(m_tempoManager.TempoTime, () =>
            {
                ReadyNotes();
            });
            disposable.Dispose();
        });
    }
}
