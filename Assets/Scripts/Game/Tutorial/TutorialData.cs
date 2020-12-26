using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "TutorialData/Create Data")]
public class TutorialData : ScriptableObject
{
    public enum eType
    {
        None,
        LeftDoor,
        RightDoor,
        LeftPhone,
        RightPhone
    }

    [System.Serializable]
    public struct Data
    {
        public int m_stage;
        public int m_notes;
        public eType m_type;
        public Sprite m_sprite;
    }

    [SerializeField]
    private Data[] m_dataList = null;

    public eType GetType(int stage, int notes)
    {
        if (m_dataList.Any(data => data.m_stage == stage && data.m_notes == notes))
        {
            return m_dataList.First(d => d.m_stage == stage && d.m_notes == notes).m_type;
        }
        return eType.LeftDoor;
    }

    public Sprite GetSprite(int stage, int notes)
    {
        if (m_dataList.Any(data => data.m_stage == stage && data.m_notes == notes))
        {
            return m_dataList.First(d => d.m_stage == stage && d.m_notes == notes).m_sprite;
        }
        return null;
    }
}
