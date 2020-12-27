using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StageData/Create Data")]
public class StageDataManager : ScriptableObject
{
    [SerializeField]
    private StageData[] m_datas = null;

    public StageData[] Datas => m_datas;
}
