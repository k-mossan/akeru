﻿using System.Collections;
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
            });
            disposable.Dispose();
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

    private void ReadyOpen(System.Action callback)
    {
        var disposable = new SingleAssignmentDisposable();
        disposable.Disposable = this.UpdateAsObservable().Where(a => m_notesManager.IsStanBy()).Subscribe(a =>
        {
            disposable.Dispose();
            disposable = new SingleAssignmentDisposable();
            disposable.Disposable = this.UpdateAsObservable().Where(b => Input.GetKeyDown(KeyCode.A)).Subscribe(b =>
            {
                disposable.Dispose();
                if (callback != null)
                {
                    callback();
                }
            });
        });
    }

    private void ReadyNotes()
    {
        ReadyOpen(() =>
        {
            if (m_notesManager.Open())
            {
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
