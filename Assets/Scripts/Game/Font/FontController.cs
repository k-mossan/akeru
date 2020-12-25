using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class FontController : MonoBehaviour
{
    [System.Serializable]
    public struct PlayData
    {
        public Vector3 pos;
        public Vector3 scale;
    }
    
    [SerializeField]
    private Animator m_animator = null;

    [SerializeField]
    private PlayData[] m_playDataList = null;

    private readonly int[] m_hashs =
    {
        Animator.StringToHash("000"),
        Animator.StringToHash("001"),
        Animator.StringToHash("002"),
        Animator.StringToHash("003"),
        Animator.StringToHash("004"),
    };
    private readonly int m_hideHash = Animator.StringToHash("Hide");

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play(int index)
    {
        gameObject.SetActive(true);
        transform.localPosition = m_playDataList[index].pos;
        transform.localScale = m_playDataList[index].scale;
        m_animator.SetTrigger(m_hashs[index]);

        var disposable = new SingleAssignmentDisposable();
        disposable.Disposable = this.UpdateAsObservable().Where(a => m_animator.GetCurrentAnimatorStateInfo(0).shortNameHash != m_hideHash).Subscribe(b =>
        {
            disposable.Dispose();
            disposable = new SingleAssignmentDisposable();
            disposable.Disposable = this.UpdateAsObservable().Where(c => m_animator.GetCurrentAnimatorStateInfo(0).shortNameHash == m_hideHash).Subscribe(d =>
            {
                disposable.Dispose();
                gameObject.SetActive(false);
            });
        });
    }
}
