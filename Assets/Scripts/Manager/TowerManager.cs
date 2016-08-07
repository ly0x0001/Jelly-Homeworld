﻿using UnityEngine;
using System.Collections.Generic;

public class TowerManager : MonoBehaviour
{
    public const int TowerCount = 50;//测试代码 临时修改
    static public int remainTowerCount = TowerCount;
    static public bool isOnUpdate = false;
    public BasicTower m_NewTower;
    public GameObject[] m_TowerPrefabs;
    public GameObject[] m_StonePrefabs;
    public GameObject m_TeleportAPrefab;
    public GameObject m_TeleportBPrefab;
    private GameObject m_SelectedPrefab;
    private GameObject m_Instance;
    private BasicTower m_InstanceBasicTower;
    [SerializeField]
    private List<BasicTower> m_TowerList = new List<BasicTower>();
    [SerializeField]
    private List<GameObject> m_StoneList = new List<GameObject>();
    [SerializeField]
    private List<GameObject> m_TeleportList = new List<GameObject>();

    private RaycastHit m_Hit;
    private int[,] towerLevelRandomMap;

    private void Start()
    {
        towerLevelRandomMap = new int[4, 4] {
            {100,100,100,100 } ,
            {75,100,100,100 } ,
            {50,75,100,100 } ,
            { 25,50,75,100} };
    }

    public void RandomInstantiateTower(Vector3 m_Position)
    {
        if (m_TowerPrefabs.Length != 0)
        {
            m_SelectedPrefab = m_TowerPrefabs[Random.Range(0, m_TowerPrefabs.Length)];
            m_Position.y = 1.3f;
            m_Instance = Instantiate(m_SelectedPrefab, m_Position, Quaternion.identity) as GameObject;
            m_InstanceBasicTower = m_Instance.GetComponent<BasicTower>();
            m_InstanceBasicTower.Init((TowerLevel)GetTowerLevel());//随机确认等级
            m_TowerList.Add(m_InstanceBasicTower);
            m_NewTower = m_InstanceBasicTower;
        }
    }

    #region Judge TowerLevel from GameLevel
    //根据关卡随机确认等级
    //缺点：不依赖TowerLevel类型
    private int GetTowerLevel()
    {
        int index= (UIGameLevel.level - 1) / 5;
        int rd = Random.Range(1, 101);
        for(int i=0;i<4; i++)
        {
            if(rd<=towerLevelRandomMap[index,i])
            {
                return i;
            }
        }

        //逻辑上代码不应该走到这里
        Debug.LogError("Judge TowerLevel from GameLevel Function ERROR");
        return 1;
    }
    #endregion

    public void RandomInstantiateStone(Vector3 m_Position)
    {
        if (m_StonePrefabs.Length != 0)
        {
            m_SelectedPrefab = m_StonePrefabs[Random.Range(0, m_StonePrefabs.Length)];
            m_Position.y = 0.158f;
            m_Instance = Instantiate(m_SelectedPrefab, m_Position, Quaternion.identity) as GameObject;
            m_StoneList.Add(m_Instance);
        }
    }

    public void RandomInstantiateTeleport(MapType mapType, Vector3 m_Position)
    {
        if (mapType == MapType.TeleportA)
            m_SelectedPrefab = m_TeleportAPrefab;
        else
            m_SelectedPrefab = m_TeleportBPrefab;
        m_Position.y = 1f;
        m_Instance = Instantiate(m_SelectedPrefab, m_Position, Quaternion.identity) as GameObject;
        m_TeleportList.Add(m_Instance);
    }

    //删除石头模型实体
    public bool DestroyStone(Ray ray)
    {
        //以ray的方向和原点，发射一条射线，检测射线是否碰撞到LayerMask为Stone的Collider
        if (Physics.Raycast(ray.origin, ray.direction, out m_Hit, Mathf.Infinity, 1 << 9))
        {
            Debug.Log("Casted");
            //将石头从石头列表中移除
            if (m_StoneList.Remove(m_Hit.transform.parent.gameObject))
                Debug.Log("Removed From Stone List");
            //摧毁找到的石头
            Destroy(m_Hit.transform.parent.gameObject);
            return true;
        }
        return false;
    }

    public void ClearStoneList()
    {
        for (int i = 0; i < m_StoneList.Count; i++)
        {
            Destroy(m_StoneList[i]);
        }
        m_StoneList.Clear();
    }

    public void ClearTeleportList()
    {
        for (int i = 0; i < m_TeleportList.Count; i++)
        {
            Destroy(m_TeleportList[i]);
        }
        m_TeleportList.Clear();
    }

    public void RetrieveUpdatableTower()
    {
        DisableHightlight();

        for (int i = 0; i < m_TowerList.Count; i++)
        {
            //如果已经被激活说明已被之前的塔搜索过，跳过该塔
            if (m_TowerList[i].m_Hightlight.activeSelf == true)
                continue;
            //如果塔已经达到最大等级，则不能升级
            if ((int)m_TowerList[i].towerLevel >= (int)TowerLevel.MaxLevel)
                continue;
            for (int j = 0; j < m_TowerList.Count; j++)
            {
                if (i == j)
                    continue;
                if (m_TowerList[i].towerType == m_TowerList[j].towerType &&
                   m_TowerList[i].towerLevel == m_TowerList[j].towerLevel)
                {
                    m_TowerList[i].NoticeEnableUpdate();
                    m_TowerList[i].m_Hightlight.SetActive(true);
                }
            }
        }
    }

    public void RetrieveMergeableTower(GameObject m_GameObject)
    {
        //进入升级塔模式          考虑是否需要创建状态机
        isOnUpdate = true;
        BasicTower m_UpdateTower = m_GameObject.GetComponent<BasicTower>();
        DisableHightlight();

        for (int i = 0; i < m_TowerList.Count; i++)
        {
            //要升级的塔自己不会高亮
            if (m_TowerList[i] != m_UpdateTower)
            {
                if (m_TowerList[i].towerType == m_UpdateTower.towerType &&
                   m_TowerList[i].towerLevel == m_UpdateTower.towerLevel)
                {
                    m_TowerList[i].NoticeEnableMerge();
                    m_TowerList[i].m_Hightlight.SetActive(true);
                }
            }
        }
    }

    public void DisableHightlight()
    {
        for (int i = 0; i < m_TowerList.Count; i++)
        {
            m_TowerList[i].m_Hightlight.SetActive(false);
        }
    }

    public bool DestroyTower(GameObject m_DeleteTower)
    {
        BasicTower m_Temp = m_DeleteTower.GetComponent<BasicTower>();
        //将塔从塔列表中移除
        if (m_TowerList.Remove(m_Temp))
        {
            Destroy(m_DeleteTower);
            return true;
        }
        return false;
    }

    public void UpdateTower(GameObject m_UpdateTower)
    {
        BasicTower m_Temp = m_UpdateTower.GetComponent<BasicTower>();
        m_Temp.UpdateTower();
        //如果是最新的塔马上进行升级，奖励额外的一座塔
        if (m_Temp == m_NewTower)
            UIRemainTowerCount.AddTowerCount();
        //退出升级模式
        isOnUpdate = false;
    }
}
