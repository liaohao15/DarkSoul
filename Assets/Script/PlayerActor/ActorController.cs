using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//角色主控制器
//负责：输入接收、移动、动画、物理、动作逻辑
public class ActorController : MonoBehaviour
{
    public GameObject Model;//抓取要控制的模型
    public BaseUserInput Pi;//输入系统基类
    public GameObject PlayerHandle;//角色手柄对象
    public CameraController Camlock;//相机锁定控制器
    
    [SerializeField]
    private Animator Anim;//获取组件Animator
    [SerializeField]
    private Rigidbody Rigid;//获取刚体
    private CapsuleCollider Col;//获取胶囊碰撞体

    [Header("角色移动与动作")]
    public float MovingSpeed = 1.0f; //基础速度
    private float RunMultiplier = 2.0f;//跑步速度倍率
    public Vector3 JumpImpulse;//向上跳跃的冲量
    public float JunmpHight = 5.0f;//向上跳跃的高度
    public float RollHight = 1.5f;//向上翻滚的高度
    [Space(10)] 
    [Header("   === Friction ===   ")]
    //系数
    public PhysicMaterial FrictionZero;//摩擦系数为0
    public PhysicMaterial FrictionOne;//摩擦系数为1
    //权重
    private Vector3 CharacterTurn;//角色转向缓存向量
    private float RunTurn;//跑步/走路动画切换参数
    private Vector3 planVc;//最终移动向量
    public float TargetValue;//动画层权重目标值
    private Vector3 DeltaPos;//位置修正偏移量

    //角色状态标记
    private bool IsGround = true;//标记是否在地面（由GroundSensor设置）
    private bool PlanLock;//移动锁定开关
    bool CanAttack;//是否可以攻击

    //获取组件
    void Awake()
    { //                         ============     获取当前物体的组件   ===================
        Anim = Model.GetComponent<Animator>();

        RefreshInput();//刷新输入源

        //空引号报错提示
        if (Pi == null)
        {
             Debug.LogError("未找到输入组件");
        }
        //获取自身刚体和碰撞体
        Rigid = GetComponent<Rigidbody>();
        Col = GetComponent<CapsuleCollider>();
    }

    //每帧执行：处理输入、动画、转向、动作触发
    void Update()
    {
        RefreshInput();//刷新输入源
                       //                             ===========   转向转换缓冲区      ============
        //动画平滑过渡：跑步/走路切换
        RunTurn = ((Pi.Run) ? 2.0f : 1.0f);
        Anim.SetFloat("forward", Pi.DL * Mathf.Lerp(Anim.GetFloat("forward"), RunTurn, 0.3f));//Mathf.Lerp(线性插值)让动画参数"forward"在走路和跑步之间平滑过渡的，实际上是由1增加到2
        //角色转向：无锁定目标时，根据移动方向平滑转向
        if (Camlock.LockTarget == null)
        {
            if (Pi.DL > 0.1f) //添加这个判断，是为了，避免当玩家没有输入时，他的TargetDug和TargetDturn的变为零，导致角色的面朝方向变为0,0
            {

                CharacterTurn = Vector3.Slerp(Model.transform.forward, Pi.DV, 0.5f);//Vector3.Slerp（ 球面插值）是用来做人物转向缓冲的
                Model.transform.forward = CharacterTurn;
            }
        }
        //角色移动逻辑：未锁定移动时计算移动方向    
        if (PlanLock == false)
        {
            //无锁定目标：模型朝向移动
            if (Camlock.LockTarget == null)
            {
                planVc = Pi.DL * Model.transform.forward * MovingSpeed * ((Pi.Run) ? RunMultiplier : 1.0f);//角色最终要移动的向量

            }
            //有锁定目标：相机朝向移动
            else
            {
                planVc = PlayerHandle.transform.TransformDirection(Pi.Dturn, 0, Pi.Dup);
                planVc *= MovingSpeed * ((Pi.Run) ? RunMultiplier : 1.0f);
                planVc.y = 0;
                print($"锁敌移动：Dturn={Pi.Dturn}, Dup={Pi.Dup}, planVc={planVc}");
            }
        }

        //防滚触发：地面+按键 或者 高速移动时
        if ((Pi.Roll && IsGround) || Rigid.velocity.magnitude > 7.0f)
        {
            Anim.SetTrigger("roll");
            CanAttack = false;
        }

        //攻击触发：满足所有条件才可以攻击
        if (Pi.Attack && CheckState("ground") && IsGround && CanAttack )
        {
            Anim.SetTrigger("attack");
        }

        //防御动画：按键状态同步
        Anim.SetBool("defense", Pi.Defense);

        //相机锁定/解锁：按键触发
        if (Pi.LockOn)
        {
            Camlock.LockUnlock();
        }
    }

    //用固定帧：处理刚体移动、物理逻辑
    private void FixedUpdate()
    {
        //应用位置修正
        Rigid.position += DeltaPos;

        //攻击时停止移动，否则正常移动
        if (Pi.Attack)
        {
            Rigid.velocity = new Vector3(0, 0, 0);//攻击时停止
        }
        else 
        {
            Rigid.velocity = new Vector3(planVc.x, Rigid.velocity.y, planVc.z) + JumpImpulse;
        }
            
        //用完冲量后清空，避免持续施加
        JumpImpulse = Vector3.zero;
        DeltaPos = Vector3.zero; 
    }

    //==用来检测Animator的层级===
    public bool CheckState(string stateName, string layerName = "Base Layer")//（传进来的名字，名字是不是Base layer）
    {   //获取动画索引
        int layerIndex = Anim.GetLayerIndex(layerName);
        //判断是否在指定动画状态
        bool result = Anim.GetCurrentAnimatorStateInfo(layerIndex).IsName(stateName);
        return result;
    }

    //==进行输入源的选择方法==
    public void RefreshInput()
    {   //获取角色身上所有的输入脚本
        BaseUserInput[] inputs = GetComponents<BaseUserInput>();
        foreach (var input in inputs)
        {   //绑定启用的输入源
            if (input.enabled == true)
            {
                Pi = input;
                break;
            }
        }
        
    }


    // ===  动画回调方法 ===
    //==人物动作==
    //跳跃进入：禁止输入、锁定移动、施加跳跃力
    public void OnJumpEnter()
    {
        Pi.InputEnable = false;
        PlanLock = true;
        JumpImpulse = new Vector3(0, JunmpHight, 0);
        CanAttack = false;
    }
    //翻滚进入：禁用输入、锁定移动、施加翻滚力
    public void OnRollEnter()
    {
        Pi.InputEnable = false;
        PlanLock = true;
        JumpImpulse = new Vector3(0, RollHight, 0);
        CanAttack = false;
    }
    
    //后撤进入：禁用输入、所定移动
    public void OnJabEnter()
    {
        Pi.InputEnable = false;
        PlanLock = true;
        CanAttack = false;
    }

    //== 人物下落检测区 ==
    //地面检测回调：处于地面
    public void Ingroud()
    {
        Anim.SetBool("isgroud", true);
    }

    //地面检测回调：离开地面
    public void NotIngroud()
    {
        Anim.SetBool("isgroud", false);
    }

    //进入地面状态：恢复输入、解锁移动、切换高摩擦
    public void OnGroundEnter()
    {
        Pi.InputEnable = true;
        PlanLock = false;
        CanAttack = true;
        Col.material = FrictionOne;
    }

    //离开地面状态：切换零摩擦
    public void OnGroundExit()
    {
        Col.material = FrictionZero;
    }

    //下落进入：禁用输入、锁定移动
    public void OnFallEnter()
    {
        Pi.InputEnable = false;
        PlanLock = true;
    }

    //后撤步跟新：施加后退冲量
    public void OnJabUpdate()//这里是让后撤步的时候后退一点
    {
        JumpImpulse = Model.transform.forward * Anim.GetFloat("jabVelocity") ;//这里不能使用newVector3(0, 0, -JabHight);因为这样不管你是面朝前还是面朝后。她永远只会往你Z轴的负坐标移动
        //所以我们使用模型的方向
    }

    //攻击进入：禁止输入、锁定移动、设置动画层权重
    public void OnAttack1hAEnter()
    {
        Pi.InputEnable = false;
        PlanLock = true;
        TargetValue = 1.0f;//缓冲调整攻击的目标值
    }

    //攻击更新：施加攻击位移、平移动画层权重
    public void OnAttack1hAUpdate()//这里是让攻击的时候前进一点
    {
        JumpImpulse = Model.transform.forward * Anim.GetFloat("attack1hAVelocity");//所以我们使用模型的方向
        //       ===      接下来我们做缓冲调整attack层的权重      ====      
        float currentWeight = Anim.GetLayerWeight(Anim.GetLayerIndex("attack"));//获取当前的权重
        currentWeight = Mathf.Lerp(TargetValue, currentWeight, 0.5f);//缓冲
        Anim.SetLayerWeight(Anim.GetLayerIndex("attack"), currentWeight);//更新
        //UnityEngine.Debug.Log("Attack层权重：" + anim.GetLayerWeight(anim.GetLayerIndex("attack")));测试是否转入到攻击层次
    }

    //攻击待机进入：恢复输入、解锁移动
    public void OnAttackIdleEnter()
    {
        Pi.InputEnable = true;
        PlanLock = false;
        TargetValue = 0.0f;
    }

    //攻击待机更新：平滑动画层权重
    public void OnAttackIdleUpdate()
    {
        JumpImpulse = Model.transform.forward * Anim.GetFloat("attack1hAVelocity");//所以我们使用模型的方向
        //       ===      接下来我们做缓冲调整attack层的权重      ====      
        float currentWeight = Anim.GetLayerWeight(Anim.GetLayerIndex("attack"));//获取当前的权重
        currentWeight = Mathf.Lerp(TargetValue, currentWeight, 0.5f);//缓冲
        Anim.SetLayerWeight(Anim.GetLayerIndex("attack"), currentWeight);//更新
    }

    //攻击位置修正：动画事件传递位移数据
    public void OnUpdateRM(object _deltaPos)
    {
        if (CheckState("attack1hC", "attack") || CheckState("attack1hB", "attack"))
        {
            DeltaPos += (DeltaPos + (Vector3)_deltaPos)/2;
        }
    }
}