using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

//角色主控制器
//负责：输入接收、移动、动画、物理、动作逻辑
public class ActorController : MonoBehaviour
{
    [Header("角色引用绑定")]
    public GameObject Model;//抓取要控制的模型
    public BaseUserInput Pi;//输入系统基类
    public GameObject PlayerHandle;//角色手柄对象


    [Header("核心组件")]
    [SerializeField]
    private Animator Anim;//获取组件Animator
    [SerializeField]
    private Rigidbody Rigid;//获取刚体
    private CapsuleCollider Col;//获取胶囊碰撞体

    [Header("角色移动与动作")]
    public float MovingSpeed = 3.0f; //基础速度
    private float RunMultiplier = 2.0f;//跑步速度倍率
    public Vector3 JumpImpulse;//向上跳跃的冲量
    public float JunmpHight = 5.0f;//向上跳跃的高度
    public float RollHight = 1.5f;//向上翻滚的高度
    public float JabVelocity = 10.0f;//后撤步的距离

    [Space(10)] 
    [Header("地面摩擦材质")]
    public PhysicMaterial FrictionZero;//摩擦系数为0
    public PhysicMaterial FrictionOne;//摩擦系数为1

    [Header("内部缓存变量")]
    private Vector3 CharacterTurn;//角色转向缓存向量
    private float RunTurn;//跑步/走路动画切换参数
    private Vector3 planVc;//最终移动向量
    public float TargetValue;//动画层权重目标值
    private Vector3 DeltaPos;//位置修正偏移量
    private Vector3 moveDirection;//移动方向向量

    [Header("角色状态标记")]
    private bool IsGround = true;//标记是否在地面（由GroundSensor设置）
    private bool PlanLock;//移动锁定开关
    private bool isJumping = false;
    private bool IsRolling = false;
    private bool IsJabbing = false;
    private bool canAttack;

    //获取组件、初始化
    void Awake()
    { //获取当前物体的组件
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

        //1.动画逻辑
        //计算走路/跑步切换倍率
        float moveInputMagnitude = Mathf.Clamp01(Mathf.Sqrt(Pi.Dup * Pi.Dup + Pi.Dturn * Pi.Dturn));
        // 推幅>0.8算跑步，否则走路
        RunTurn = (Pi.Run || moveInputMagnitude > 1.0f) ? 2.0f : 1.0f;

        Anim.SetFloat("forward", Pi.DL * Mathf.Lerp(Anim.GetFloat("forward"), RunTurn, 0.3f));
        
        //2.角色转向
       
        if (Pi.DL > 0.1f && moveDirection != Vector3.zero)
        {
            //球面插值平滑转向
            CharacterTurn = Vector3.Slerp(Model.transform.forward,moveDirection, 0.5f);
            Model.transform.forward = CharacterTurn;
        }

        //3.移动逻辑
        if (PlanLock == false)
        {   //角色最终要移动的向量
            //planVc = Pi.DL * Model.transform.forward * MovingSpeed * ((Pi.Run) ? RunMultiplier : 1.0f);

            // 获取主相机的正前方和右方向（忽略 Y 轴，只取水平）
            Transform cam = Camera.main.transform;
            Vector3 camForward = cam.forward;
            Vector3 camRight = cam.right;
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            // 根据输入计算移动方向（相对于相机）
            moveDirection = (camForward * Pi.Dup + camRight * Pi.Dturn).normalized;

            // 最终移动向量（保留原有速度倍率）
            planVc = moveDirection * MovingSpeed * (Pi.Run ? RunMultiplier : 1.0f);

        }

        //4.翻滚触发：地面+按键 或者 高速移动时
        if ( IsGround && Rigid.velocity.magnitude > 1.2f && Pi.Jump)
        {
            Anim.SetTrigger("roll");

        }

        //5.攻击触发：满足所有条件才可以攻击
        if (Pi.Attack && CheckState("ground") && canAttack )
        {
            Anim.SetTrigger("attack");
        }

        //6.防御动画：按键状态同步
        Anim.SetBool("defense", Pi.Defense);
        
        //7.跳跃触发：满足条件才可以跳跃
        if (Pi.Jump && CheckState("ground"))
        {
            Anim.SetTrigger("jump");  
        }

    }

    //用固定帧：处理刚体移动、物理逻辑
    private void FixedUpdate()
    {
        //应用位置修正
        Rigid.position += DeltaPos;
        if (Pi.Attack)
            Rigid.velocity = new Vector3(0, 0, 0);
        else
            Rigid.velocity = new Vector3(planVc.x, Rigid.velocity.y, planVc.z) + JumpImpulse;

        
        JumpImpulse = Vector3.zero;
        DeltaPos = Vector3.zero; 
    }

    //==用来检测Animator的层级===
    public bool CheckState(string stateName, string layerName = "Base Layer")//（传进来的名字，名字是不是Base layer）
    {   //获取动画索引
        //判断是否在指定动画状态
        return Anim.GetCurrentAnimatorStateInfo(Anim.GetLayerIndex(layerName)).IsName(stateName);
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
        if (IsGround && !isJumping)
        { 
            Rigid.AddForce(new Vector3(0, JunmpHight, 0), ForceMode.Impulse);
        }
        isJumping = true;
        Pi.InputEnable = false;
        PlanLock = true;
        canAttack = false;
    }
    //翻滚进入：禁用输入、锁定移动、施加翻滚力
    public void OnRollEnter()
    {
        if (IsGround && !IsRolling)
        {
            Rigid.AddForce(new Vector3(0, RollHight, 0), ForceMode.Impulse);
        }
        IsRolling = true;
        Pi.InputEnable = false;
        PlanLock = true;
        canAttack = false;
    }
    
    //后撤进入：禁用输入、所定移动
    public void OnJabEnter()
    {
       
        Pi.InputEnable = false;
        PlanLock = true;
        canAttack = false;
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
        IsRolling = false;
        isJumping = false;
        IsJabbing = false;
        Pi.InputEnable = true;
        PlanLock = false;
        canAttack = true;
        Col.material = FrictionOne;
        Anim.ResetTrigger("roll");

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
        if (IsGround)//后撤步位移只在地面上施加，空中后撤不施加位移  
        {
            if (IsGround && !IsJabbing)
            {
                Rigid.AddForce(Model.transform.forward * Anim.GetFloat("jabVelocity") * JabVelocity, ForceMode.Impulse);
            }
            IsJabbing = true;
        }

    }

    //攻击进入：禁止输入、锁定移动、设置动画层权重
    public void OnAttack1hAEnter()
    {
        Anim.SetLayerWeight(Anim.GetLayerIndex("attack"), 1.0f);//直接切入攻击层.
        Pi.InputEnable = false;
        //PlanLock = true;
        TargetValue = 1.0f;//缓冲调整攻击的目标值
    }

    //攻击更新：施加攻击位移、平移动画层权重
    public void OnAttack1hAUpdate()//这里是让攻击的时候前进一点
    {
        if (IsGround)//攻击位移只在地面上施加，空中攻击不施加位移
        {
            JumpImpulse = Model.transform.forward * Anim.GetFloat("attack1hAVelocity");
        }
        
        //       ===      接下来我们做缓冲调整attack层的权重      ====      
        float currentWeight = Anim.GetLayerWeight(Anim.GetLayerIndex("attack"));//获取当前的权重
        currentWeight = Mathf.Lerp(TargetValue, currentWeight, 0.5f);//缓冲
        Anim.SetLayerWeight(Anim.GetLayerIndex("attack"), currentWeight);//更新
        //UnityEngine.Debug.Log("Attack层权重：" + anim.GetLayerWeight(anim.GetLayerIndex("attack")));测试是否转入到攻击层次
    }

    //攻击待机进入：恢复输入、解锁移动
    public void OnAttackIdleEnter()
    {
        Anim.SetLayerWeight(Anim.GetLayerIndex("attack"), 0.0f);//直接切入待机层.
        Pi.InputEnable = true;
        //PlanLock = false;
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

    public void cleanTrigger()
    {
        // 动画事件要求存在，但不需要做任何事
    }
}