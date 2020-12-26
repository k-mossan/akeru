using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct StageData
{
    [SerializeField]
    private int m_notesIndex;

    [SerializeField]
    private PhoneController.eType m_phoneType;

    [SerializeField]
    private int m_phoneMax;

    public int NotesIndex => m_notesIndex;
    public PhoneController.eType PhoneType => m_phoneType;
    public int PhoneMax => m_phoneMax;

    public StageData(int notesIndex, PhoneController.eType phoneType, int phoneMax)
    {
        m_notesIndex = notesIndex;
        m_phoneType = phoneType;
        m_phoneMax = phoneMax;
    }
}
