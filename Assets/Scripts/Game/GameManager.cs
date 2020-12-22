using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    [SerializeField]
    private GameObject m_playerPrefab = null;

    [SerializeField]
    private Transform m_playerRoot = null;

    [SerializeField]
    private GameObject m_stagePrefab = null;

    [SerializeField]
    private Transform m_stageRoot = null;

    private PlayerController m_player = null;
    private StageController m_stage = null;

    public PlayerController Player => m_player;
    public StageController Stage => m_stage;

    // Start is called before the first frame update
    void Start()
    {
        GameObject obj = GameObject.Instantiate(m_playerPrefab, m_playerRoot);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.Euler(Vector3.zero);
        obj.transform.localScale = Vector3.one;
        m_player = obj.GetComponent<PlayerController>();
        obj = GameObject.Instantiate(m_stagePrefab, m_stageRoot);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.Euler(Vector3.zero);
        obj.transform.localScale = Vector3.one;
        m_stage = obj.GetComponent<StageController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
