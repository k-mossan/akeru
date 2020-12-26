using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageDataManager
{
    private List<StageData> m_stageDataList = new List<StageData>();

    public List<StageData> StageDataList => m_stageDataList;

    public StageDataManager(int stageNo)
    {
        if (stageNo == 0)
        {
            m_stageDataList.Add(new StageData(0, PhoneController.eType.Left, 2));
            m_stageDataList.Add(new StageData(1, PhoneController.eType.Right, 2));
            m_stageDataList.Add(new StageData(0, PhoneController.eType.None, 0));
            //m_stageDataList.Add(new StageData(1, PhoneController.eType.None, 0));
            //m_stageDataList.Add(new StageData(0, PhoneController.eType.None, 0));
            //m_stageDataList.Add(new StageData(1, PhoneController.eType.None, 0));
            //m_stageDataList.Add(new StageData(0, PhoneController.eType.None, 0));
            //m_stageDataList.Add(new StageData(1, PhoneController.eType.None, 0));
        }
    }
}
