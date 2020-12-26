using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StageData/Create Data")]
public class StageDataManager : ScriptableObject
{
    [SerializeField]
    private StageData[] m_datas = null;

    [SerializeField]
    private int m_no = 0;

    public StageData[] Datas => m_datas;
    public int No => m_no;
}
