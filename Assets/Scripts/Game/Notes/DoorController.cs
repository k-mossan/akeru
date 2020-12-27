using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class DoorController : MonoBehaviour
{
    [SerializeField]
    private Animator m_animator = null;

    private readonly int[] m_openedHashs = {
        Animator.StringToHash("Opened00"),
        Animator.StringToHash("Opened01"),
        Animator.StringToHash("Opened02"),
        Animator.StringToHash("Opened03"),
    };
    private readonly int[] m_openHashs =
    {
        Animator.StringToHash("Open00"),
        Animator.StringToHash("Open01"),
        Animator.StringToHash("Open02"),
        Animator.StringToHash("Open03"),
    };
    private readonly int[] m_closeHashs =
    {
        Animator.StringToHash("Close00"),
        Animator.StringToHash("Close01"),
        Animator.StringToHash("Close02"),
        Animator.StringToHash("Close03"),
    };

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    public bool IsReady(int index)
    {
        return m_animator.GetCurrentAnimatorStateInfo(0).shortNameHash == m_closeHashs[index];
    }

    public void Ready(int index)
    {
        gameObject.SetActive(true);
        m_animator.SetTrigger(m_closeHashs[index]);
    }

    public bool IsPlay(int index)
    {
        return m_animator.GetCurrentAnimatorStateInfo(0).shortNameHash == m_openHashs[index];
    }

    public void Play(int index, System.Action callback)
    {
        SoundManager.Instance.Play((SoundController.eType)index);
        m_animator.SetTrigger(m_openedHashs[index]);
        var disposable = new SingleAssignmentDisposable();
        disposable.Disposable = this.UpdateAsObservable().Where(a => m_animator.GetCurrentAnimatorStateInfo(0).shortNameHash != m_openedHashs[index]).Subscribe(b =>
        {
            disposable.Dispose();
            disposable = new SingleAssignmentDisposable();
            disposable.Disposable = this.UpdateAsObservable().Where(c => m_animator.GetCurrentAnimatorStateInfo(0).shortNameHash == m_openedHashs[index]).Subscribe(d =>
            {
                disposable.Dispose();
                if (callback != null)
                {
                    callback();
                }
            });
        });
    }

    public bool IsOpen(int index)
    {
        return m_animator.GetCurrentAnimatorStateInfo(0).shortNameHash == m_openedHashs[index];
    }
}
