using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class DoorController : MonoBehaviour
{
    [SerializeField]
    private Animator m_animator = null;

    private readonly int m_openHash = Animator.StringToHash("Open");
    private readonly int[] m_openHashs =
    {
        Animator.StringToHash("Open00"),
        Animator.StringToHash("Open01"),
    };
    private readonly int[] m_closeHashs =
    {
        Animator.StringToHash("Close00"),
        Animator.StringToHash("Close01"),
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

    public void Play(System.Action callback)
    {
        m_animator.SetTrigger(m_openHash);
        var disposable = new SingleAssignmentDisposable();
        disposable.Disposable = this.UpdateAsObservable().Where(a => m_animator.GetCurrentAnimatorStateInfo(0).shortNameHash != m_openHash).Subscribe(b =>
        {
            disposable.Dispose();
            disposable = new SingleAssignmentDisposable();
            disposable.Disposable = this.UpdateAsObservable().Where(c => m_animator.GetCurrentAnimatorStateInfo(0).shortNameHash == m_openHash).Subscribe(d =>
            {
                disposable.Dispose();
                if (callback != null)
                {
                    callback();
                }
            });
        });
    }

    public bool IsOpen()
    {
        return m_animator.GetCurrentAnimatorStateInfo(0).shortNameHash == m_openHash;
    }
}
