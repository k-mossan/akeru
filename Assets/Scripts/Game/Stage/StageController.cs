using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class StageController : MonoBehaviour
{
    [SerializeField]
    private GameObject m_notesManagerPrefab = null;

    [SerializeField]
    private Transform m_notesManagerRoot = null;

    private NotesManager m_notesManager = null;

    private void Awake()
    {
        GameObject obj = GameObject.Instantiate(m_notesManagerPrefab, m_notesManagerRoot);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.Euler(Vector3.zero);
        obj.transform.localScale = Vector3.one;
        m_notesManager = obj.GetComponent<NotesManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        this.UpdateAsObservable().Where(_ => Input.GetKey(KeyCode.A)).Subscribe(_ =>
        {
            m_notesManager.Play(0);
        });
        this.UpdateAsObservable().Where(_ => Input.GetKey(KeyCode.B)).Subscribe(_ =>
        {
            m_notesManager.Play(1);
        });
    }
}
