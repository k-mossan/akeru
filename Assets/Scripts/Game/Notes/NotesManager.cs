using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotesManager : MonoBehaviour
{
    [SerializeField]
    private GameObject m_notesPrefab = null;

    [SerializeField]
    private Transform m_notesRoot = null;

    private readonly int m_notesMax = 3;
    private NotesController[] m_notesList = null;

    public NotesController[] NotesList => m_notesList;

    private void Awake()
    {
        m_notesList = new NotesController[m_notesMax];
        for (int i = 0; i < m_notesMax; ++i)
        {
            GameObject obj = GameObject.Instantiate(m_notesPrefab, m_notesRoot);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.Euler(Vector3.zero);
            obj.transform.localScale = Vector3.one;
            m_notesList[i] = obj.GetComponent<NotesController>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Play(int index)
    {
        m_notesList[0].Ready(index);
        m_notesList[0].Play();
    }
}
