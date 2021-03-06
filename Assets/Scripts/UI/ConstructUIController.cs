﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ConstructUIController : MonoBehaviour 
{
    public Text text;
    public float UIHeight;
    public MapManager m_MapManager;
    public TowerManager m_TowerManager;
    public Camera m_Camera;
    public AudioManager m_AudioManager;
    public Image m_Image;
    public Sprite m_SpriteConstruct;
    public Sprite m_SpriteUpgrade;

    private Vector3 m_Scale;

    private Canvas m_Canvas;
    private Animator m_Animator;
    private Vector3 m_Pos;
    private GameObject m_Tower;
    private string state;

    private bool isClick = false;

    [HideInInspector]public Vector2 m_MapPos;
    [HideInInspector]public Ray m_Ray;
    private void Awake()
    {
        m_Scale = transform.localScale;
        m_Canvas=GetComponent<Canvas>();
        m_Animator = GetComponent<Animator>();
        m_Pos=new Vector3();
        m_MapPos=new Vector2();
        m_Ray=new Ray();
    }

    private void Start()
    {
        m_Canvas.enabled=false;
        m_Pos.y=UIHeight;
    }

    private void Update()
    {
        float temp = m_Camera.orthographicSize / 10f;
        m_Scale.Set(temp,temp,temp);
        transform.localScale = m_Scale;
    }
    
    //将UI移动至参数坐标
    public void MoveTo(Vector3 m_PosTemp)
    {
        //不同的情况 UI高度也不同
        switch (state)
        {
            case "Tower":
                m_Pos.y=3.5f;
                break;
            case "Upgrade":
                m_Pos.y= UIHeight;
                break;
            default:
                //Debug.LogError("ConstructUI Error : No this name sprite");
                break;
        }
        m_Pos.x= m_PosTemp.x;
        m_Pos.z= m_PosTemp.z;
        transform.position=m_Pos;
    }

    //改变按钮内容
    public void ChangeButtonImage(string content)
    {
        switch(content)
        {
            case "Tower":
                m_Image.sprite = m_SpriteConstruct;
                break;
            case "Upgrade":
                m_Image.sprite = m_SpriteUpgrade;
                break;
            default:
                //Debug.LogError("ConstructUI Error : No this name sprite");
                break;
        }
    }

    public void ChangeState(string state)
    {
        this.state = state;
    }

    //更新地图坐标，参数来自上一次显示UI时的地图坐标
    public void UpdateMapPos(Vector2 m_MapPos)
    {
        this.m_MapPos.x=m_MapPos.x;
        this.m_MapPos.y=m_MapPos.y;
        //Debug.Log("X:"+m_MapPos.x+" Y:"+m_MapPos.y);
    }

    //更新上一次显示UI时屏幕到鼠标的射线信息
    //由于DestroyStone函数作废，该函数也作废
    //但是该函数仍然在使用中
    public void UpdateCameraRay(Ray ray)
    {
        m_Ray.origin=ray.origin;
        m_Ray.direction=ray.direction;
    }

    //显示UI
    public void Enable()
    {
        //Debug.Log("Invoke ConstructUIController OnEnable");
        m_Canvas.enabled=true;
    }

    //隐藏UI
    public void Disable()
    {
        //Debug.Log("Invoke ConstructUIController OnDisable");
        m_Canvas.enabled=false;
        Hide();
    }

    public void Show()
    {
        m_Animator.SetInteger("State",1);
        isClick = false;
    }

    public void Hide()
    {
        m_Animator.SetInteger("State", 2);
    }

    public void SetTowerGameObject(GameObject m_Tower)
    {
        this.m_Tower = m_Tower;
    }

    public GameObject GetTowerGameObject()
    {
        return m_Tower;
    }

    public void OnClick()
    {
        //点击后，UI消失
        Hide();
        if(isClick)
        {
            return;
        }
        isClick = true;
        switch (state)
        {
            case "Tower":
                {
                    m_AudioManager.PlayPop();
                    m_MapManager.ModifyMap((int)m_MapPos.x, (int)m_MapPos.y, MapType.Tower);
                    m_TowerManager.RandomInstantiateTower(transform.position);
                    m_TowerManager.RetrieveUpdatableTower();
                    UIRemainTowerCount.SubTowerCount();
                    break;
                }
            case "Upgrade":
                {
                    //m_AudioManager.PlayPop();
                    m_TowerManager.RetrieveMergeableTower(m_Tower);
                    break;
                }
        }
    }
}
