using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;


public class ActorController : MonoBehaviour
{
    public GameObject model;//抓取要控制的模型
    //public PlayerInput pi;//调用PlayerInput脚本。修改成手柄输入
    public BaseUserInput pi;

    [SerializeField]
    private Animator anim;//获取组件Animator
    [SerializeField]
    private Rigidbody rigid;//获取刚体

    public float movingSpeed = 1.0f; //基础速度
    private float RunMultiplier = 2.0f;//当跑步键按下时，乘以这个速度倍率

    public Vector3 JumpImpulse;//向上跳跃的冲量
    public float JunmpHight = 5.0f;//向上跳跃的高度
    public float RollHight = 1.5f;//向上翻滚的高度
    //public float JabHight = 2.0f;//后跳的高度      不需要这个了，有curigidur曲线控制就够了

    [Space(10)] 
    [Header("   === Friction ===   ")]
    public PhysicMaterial frictionZero;//摩擦系数为0

    public PhysicMaterial frictionOne;//摩擦系数为1


    private bool isGround = true;//标记是否在地面（由GroundSensor设置）

    private Vector3 CharacterTurn;//为角色转向而设计的变量
    private float RunTurn;//为动画切换而设计的变量

    private Vector3 planVc;//角色移动的最终量

    private bool PlanLock;

    bool canAttack;//攻击进行的第三个条件
    private CapsuleCollider col;//获取胶囊碰撞体

    public float targetValue;

    private Vector3 deltaPos;

    // Start is called before the first frame update
    void Awake()
    { //                         ============     获取当前物体的组件   ===================
        anim = model.GetComponent<Animator>();
        //pi = GetComponent<PlayerInput>();修改成手柄输入
        //这个代码是用来控制输入选择的，但是他只在Awake阶段进行一次
        BaseUserInput[] inputs = GetComponents<BaseUserInput>();//继续修改用，抽象类来分装
        foreach (var input in inputs)
        {
            if (input.enabled == true)
            {
                pi = input;
                break;
            }
        }


        rigid = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        RefreshInput();//一运行，就进行一次输入源的选择
        //                             ===========   转向转换缓冲区      ============
        //1.动画转换缓冲
        RunTurn = ((pi.run) ? 2.0f : 1.0f);
        anim.SetFloat("forward", pi.dL * Mathf.Lerp(anim.GetFloat("forward"), RunTurn, 0.3f));//Mathf.Lerp(线性插值)让动画参数"forward"在走路和跑步之间平滑过渡的，实际上是由1增加到2

        //2.人物转向缓冲
        if (pi.dL > 0.1f) //添加这个判断，是为了，避免当玩家没有输入时，他的TargetDug和TargetDturn的变为零，导致角色的面朝方向变为0,0
        {

            CharacterTurn = Vector3.Slerp(model.transform.forward, pi.dV, 0.5f);//Vector3.Slerp（ 球面插值）是用来做人物转向缓冲的
            model.transform.forward = CharacterTurn;
        }

        //                  ==================     移动   ==================
        //1.移动，还使用到FixedUpdate方法
        if (PlanLock == false)
        {
            planVc = pi.dL * model.transform.forward * movingSpeed * ((pi.run) ? RunMultiplier : 1.0f);//角色最终要移动的向量

        }

        //2.跳跃
        if (pi.jump && isGround)
        {
            anim.SetTrigger("jump");
        }
        /*
         * 这里有一个类与对象的成员访问（pi是PlayerInput里的一个实例；jump是PlayerInput里的字段）
         */

        //3.落地翻滚在Ingroud里面
        //3.下落翻滚
        if (rigid.velocity.magnitude > 1.6f && isGround)
        {
            anim.SetTrigger("roll");
        }

        //4.攻击
        if (pi.attack && CheckState("ground") && isGround && canAttack )
        {
            anim.SetTrigger("attack");
        }


    }
    private void FixedUpdate()
    {
        rigid.position += deltaPos;
        //1.物理移动：把输入的移动向量赋值给刚体速度
        if (pi.attack)
        {
            rigid.velocity = new Vector3(0, 0, 0);
        }
        else 
        {
            rigid.velocity = new Vector3(planVc.x, rigid.velocity.y, planVc.z) + JumpImpulse;
        }
            
        //2.用完冲量后清空，避免持续施加
        JumpImpulse = Vector3.zero;
        deltaPos = Vector3.zero;

    }

    //      ======   用来检测Animator的层级   ======
    public bool CheckState(string stateName, string layerName = "Base Layer")//（传进来的名字，名字是不是Base layer）
    {
        int layerIndex = anim.GetLayerIndex(layerName);
        bool result = anim.GetCurrentAnimatorStateInfo(layerIndex).IsName(stateName);
        return result;
    }

    //      ======   进行输入源的选择方法   ======
    public void RefreshInput()
    {
        BaseUserInput[] inputs = GetComponents<BaseUserInput>();
        foreach (var input in inputs)
        {
            if (input.enabled == true)
            {
                pi = input;
                break;
            }
        }
        
    }


    //                          ******************************  信息接收区    *************************************
    //                              ==================      跳跃动作状态的显示     ============================
    //由动画状态机触发的跳跃逻辑（FSMOnEnter发消息调用）
    public void OnJumpEnter()
    {
        pi.InputEnable = false;
        PlanLock = true;
        JumpImpulse = new Vector3(0, JunmpHight, 0);
        canAttack = false;
    }

    //                              ==================      翻滚动作状态的显示     ============================
    public void OnRollEnter()
    {
        pi.InputEnable = false;
        PlanLock = true;
        JumpImpulse = new Vector3(0, RollHight, 0);
        canAttack = false;
    }

    //                              ==================      后跳动作状态的显示     ============================
    public void OnJabEnter()
    {
        pi.InputEnable = false;
        PlanLock = true;
        canAttack = false;
        //JumpImpulse = model.transform.forward * (-1) * JabHight;//这里不能使用newVector3(0, 0, -JabHight);因为这样不管你是面朝前还是面朝后。她永远只会往你Z轴的负坐标移动
        ////所以我们使用模型的方向
        ///但是不再Enter里而是在Update里是想让他时时刻刻都在更新这个冲量
    }

    //                           ==================      人物下落检测区    ============================
    //地面检测回调（GroundSensor发消息调用）
    public void Ingroud()
    {
        //print("is groud");
        //isGround = true;
        anim.SetBool("isgroud", true);

        //pi.InputEnable = true;
        //PlanLock = false;
        //anim.SetBool("isfall", false);

    }

    public void NotIngroud()
    {
        //print("not is groud");
        //isGround = false;
        anim.SetBool("isgroud", false);
    }

    public void OnGroundEnter()
    {
        pi.InputEnable = true;
        PlanLock = false;
        canAttack = true;
        col.material = frictionOne;
    }

    public void OnGroundExit()
    {
        col.material = frictionZero;
    }

    public void OnFallEnter()
    {
        pi.InputEnable = false;
        PlanLock = true;
    }

    public void OnJabUpdate()//这里是让后撤步的时候后退一点
    {
        JumpImpulse = model.transform.forward * anim.GetFloat("jabVelocity") ;//这里不能使用newVector3(0, 0, -JabHight);因为这样不管你是面朝前还是面朝后。她永远只会往你Z轴的负坐标移动
        //所以我们使用模型的方向
    }

    public void OnAttack1hAEnter()
    {
        pi.InputEnable = false;
        PlanLock = true;
        targetValue = 1.0f;//缓冲调整攻击的目标值
       

    }

    public void OnAttack1hAUpdate()//这里是让攻击的时候前进一点
    {
        JumpImpulse = model.transform.forward * anim.GetFloat("attack1hAVelocity");//所以我们使用模型的方向
        //       ===      接下来我们做缓冲调整attack层的权重      ====      
        float currentWeight = anim.GetLayerWeight(anim.GetLayerIndex("attack"));//获取当前的权重
        currentWeight = Mathf.Lerp(targetValue, currentWeight, 0.5f);//缓冲
        anim.SetLayerWeight(anim.GetLayerIndex("attack"), currentWeight);//更新
        //UnityEngine.Debug.Log("Attack层权重：" + anim.GetLayerWeight(anim.GetLayerIndex("attack")));测试是否转入到攻击层次


    }

    public void OnAttackIdleEnter()
    {
        pi.InputEnable = true;
        PlanLock = false;
       // anim.SetLayerWeight(anim.GetLayerIndex("attack"), 0.0f);
        targetValue = 0.0f;
    }

    public void OnAttackIdleUpdate()
    {

        JumpImpulse = model.transform.forward * anim.GetFloat("attack1hAVelocity");//所以我们使用模型的方向
        //       ===      接下来我们做缓冲调整attack层的权重      ====      
        float currentWeight = anim.GetLayerWeight(anim.GetLayerIndex("attack"));//获取当前的权重
        currentWeight = Mathf.Lerp(targetValue, currentWeight, 0.5f);//缓冲
        anim.SetLayerWeight(anim.GetLayerIndex("attack"), currentWeight);//更新
       

    }


    public void OnUpdateRM(object _deltaPos)
    {
        if (CheckState("attack1hC", "attack") || CheckState("attack1hB", "attack"))
        {
            deltaPos += (deltaPos + (Vector3)_deltaPos)/2;
        }
    }

}




