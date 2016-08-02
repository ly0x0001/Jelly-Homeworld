﻿using UnityEngine;
using System.Collections;

public enum MapType
{
    Empty,//该位置为空
    Basic,//该位置有地基
    Tower,//该位置有防御塔
    TeleportA,//该位置有传送门A
    TeleportB//该位置有传送门B 
}

public class MapManager : MonoBehaviour
{
    public TowerManager m_TowerManager;
    public static int mapSize = 2;
    public static int mapRegionX = 7;//实际要减1，外围有围墙
    public static int mapRegionY = 8;//实际要减1，外围有围墙

    //存放地图位置信息
    private MapType[,] map;

    private int[,] mapTemp;

    //寻路实体
    private MonsterPathFinding monsterPathFinding;

    //测试代码
    public bool GenerateStone=false;

    public void FixedUpdate()
    {
        if(GenerateStone)
        {
            GenerateStone=false;
            GenerateStoneByMap();
        }
    }

    //初始化地图信息，默认值为0（Empty），地图边缘有一圈围墙，建筑类型为石头（Basic）
    private void Awake()
    {
        monsterPathFinding = new MonsterPathFinding();
        map = new MapType[mapRegionX, mapRegionY];
        mapTemp=new int[mapRegionX, mapRegionY];
        for (int j = 0; j < mapRegionX; j++)
        {
            for (int k = 0; k < mapRegionY; k++)
            {
                if (j == 0 || j == mapRegionX - 1 || k == 0 || k == mapRegionY - 1)
                {
                    map[j, k] = MapType.Basic;
                    mapTemp[j,k]=(int)MapType.Basic;
                }
            }
        }
        //monsterPathFinding.monsterPathFinding(mapTemp, 5, 3, 5, 5);
        Debug.Log("Path Finding Successed");
    }

    //是非可以建造制定建筑，如果可以则修改地图信息并返回true，不行则为false
    public bool ModifyMap(int posX, int posY, MapType mapType)
    {
        //通过建筑类型执行分支
        switch (map[posX, posY])
        {
            case MapType.Empty:
            {
                //石头只能在空地上建造
                if(mapType==MapType.Basic)
                {
                    map[posX, posY] = mapType;
                    return true;
                }
                break;
            }
            case MapType.Basic:
            {
                //塔只能在石头上建造
                if(mapType==MapType.Tower)
                {
                    map[posX, posY] = mapType;
                    return true;
                }
                break;
            }
        }
        Debug.LogError("Map Error");
        return false;
    }

    public void GenerateStoneByMap()
    {
        Vector3 m_VecTemp=new Vector3();
        for (int j = 0; j < mapRegionX; j++)
        {
            for (int k = 0; k < mapRegionY; k++)
            {
                if(map[j,k]==MapType.Basic)
                {
                    m_VecTemp.x=j*mapSize;
                    m_VecTemp.z=k*mapSize;
                    m_TowerManager.RandomInstantiateStone(m_VecTemp);
                }
            }
        }
    }


    //根据参数地图位置信息，返回该位置的建筑类型
    public MapType GetMap(int posX,int posY)
    {
        return map[posX,posY];
    }

    //返回所有地图信息
    public MapType[,] GetMap()
    {
        return map;
    }
}
