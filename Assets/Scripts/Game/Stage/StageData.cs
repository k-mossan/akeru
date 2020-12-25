using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageData
{
    private int m_notesIndex = 0;
    private PhoneController.eType m_phoneType = PhoneController.eType.None;
    private int m_phoneMax = 0;

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
