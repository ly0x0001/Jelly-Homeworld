﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class BasicEnemy : MonoBehaviour 
{
    public bool GoDie = false;
    private float isStun = 0;
    [SerializeField]
    protected float maxHealth = 100;//最初的血量
    [SerializeField]
    protected float health = 100;//当前血量
    [SerializeField]
    protected float maxArmor = 20;//最初的护甲
    [SerializeField]
    protected float armor = 20;//当前的护甲


    [SerializeField]
    protected float stunTime = 0;//眩晕时间


    [SerializeField]
    protected float restore = 5;//回血


    [SerializeField]
    private bool[] isSlowdown;//是否受到减速
    [SerializeField]
    protected float slowdownRate = 0.0f;//减速百分比
    [SerializeField]
    private float slowdownTime;//减速失效时间


    [SerializeField]
    private float LimitAttackTimes = 0;//限制别被攻击的次数
    private float CountAttackTimes = 0;//记录这段时间内被攻击的次数
    private float intervalAttack = 1;//1秒内受到一定次数的攻击

    [SerializeField]
    private Slider slider;

    private float startTime;//记录时间来判断一秒内攻击的次数

    private bool isDied = false;
    private float damageTemp;
    private Collider m_Collider;
    private MonsterWalk m_MonsterWalk;
    private bool isSigned = false;//用来判断是否被标记为攻击目标

    // Use this for initialization
    void Start() 
    {
        m_Collider = GetComponent<Collider>();
        m_MonsterWalk = GetComponent<MonsterWalk>();
        //isSlowdown = new bool[(int)TowerLevel.MaxLevel];
        //for (int i = 0; i < (int)TowerLevel.MaxLevel; i++)
        //{
        //    isSlowdown[i] = false;
        //}
        startTime = Time.time;
        //slowdownTime = new float[(int)TowerLevel.MaxLevel];
    }

    // Update is called once per frame
    void Update()
    {
        if (GoDie == true)
        {
            Died();
            GoDie = false;
        }
        if (health <= 0 && isDied == false)
        {
            isDied = true;
            Died();
        }

    }

    public void Init(float restore, float maxArmor, float speed, float isStun,float isSlownDown,float LimitAttackTimes)
    {
        //限制受到攻击次数还没写
        m_MonsterWalk = GetComponent<MonsterWalk>();
        this.restore = restore;
        this.maxArmor = maxArmor;
        m_MonsterWalk.changeSpeed(speed);
        m_MonsterWalk.setIsSlownDown(isSlownDown);
        this.isStun = isStun;
        this.LimitAttackTimes = LimitAttackTimes;
    }

    void FixedUpdate()
    {

        if (Time.time - startTime >= intervalAttack)
        {
            CountAttackTimes = 0;
            intervalAttack += 1;
        }

        //怪物回血
        if (health < maxHealth)
        {
            health += restore * Time.deltaTime;
            health = health > maxHealth ? maxHealth : health;
        }
        slider.value = health / maxHealth;
    }


    //敌人死后
    protected void Died()
    {
        //防止在敌人死亡的过程中出现在新的塔的范围内
        m_Collider.enabled = false;

        //GameManager.OnEnemyDied(new GameManager.EnemyDiedEventsArgs(m_Collider));
        Debug.Log("Died");
        //死亡后自我销毁
        Destroy(this.gameObject);
    }

    //受到伤害的时候调用，fireStrikeArmor表示破甲数值
    public void TakeDamage(float fireDamage, float fireStrikeArmor, float fireStunTime, int slowDownLevel)
    {
        stunTime = isStun == 1 ? fireStunTime : 0;
        slowdownRate = TowerElemInfo.slowdownDegree[slowDownLevel];
        slowdownTime = TowerElemInfo.slowdownTime[slowDownLevel];
        //传眩晕时间，减速时间给MonsterWalk
        m_MonsterWalk.setFireStatus(fireStunTime, slowDownLevel);
        armor = maxArmor - fireStrikeArmor;
        if (CountAttackTimes < LimitAttackTimes)
        {
            TakeHealth(fireDamage);
            CountAttackTimes++;
        }
        else
        {
            return;
        }
    }
    
    //public void TakeSlowdown(TowerLevel slowdownLevel)
    //{
    //        slowdownTime[(int)slowdownLevel] = Time.time + TowerInfo.debuffDuringTime[(int)TowerType.SourceBuffSlowdown, (int)slowdownLevel];
    //        if (isSlowdown[(int)slowdownLevel] == false)
    //        {
    //            isSlowdown[(int)slowdownLevel] = true;
    //            slowdownRate += TowerInfo.sourceBuffSlow[(int)TowerType.SourceBuffSlowdown, (int)slowdownLevel];
    //        }
    //}

    //计算扣血量
    protected void TakeHealth(float fireDamage)
    {
        //护甲大于0和小于0时的计算方式不一样
        if(armor>=0)
        {
            damageTemp = fireDamage * (1 - ((armor * 0.06f) / (1 + 0.06f * armor)));
        }    
        else
        {
            damageTemp = fireDamage * (1 + (2 - Mathf.Pow(0.94f, -armor)));
        }
        health -= damageTemp;
        UIScore.Add(damageTemp);
        Debug.Log("Damage:" + damageTemp + " Health:" + health);
    }

    public void TakeStun(TowerLevel towerLevel)
    {

    }


    public float GetHealth()
    {
        return health;
    }

    protected void OnDestroy()
    {
        GameManager.OnEnemyDied(new GameManager.EnemyDiedEventsArgs(m_Collider));
    }
}
