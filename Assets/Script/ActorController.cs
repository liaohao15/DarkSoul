using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class ActorController : MonoBehaviour
{
    public GameObject model;//抓取要控制的模型
    public PlayerInput pi;//调用PlayerInput脚本
    public  float movingSpeed = 2.0f; //基础速度
    public Vector3 JumpImpulse;//向上跳跃的冲量
    public float JunmpHight = 10.0f;//向上跳跃的高度

    [SerializeField]
    private Animator anim;//获取组件Animator
    [SerializeField]
    private Rigidbody rigid;//获取刚体

    private Vector3 planVc;//角色移动的最终量
    private float RunMultiplier = 2.0f;//当跑步键按下时，乘以这个速度倍率

    private Vector3 CharacterTurn;//为角色转向而设计的变量
    private float RunTurn;//为动画切换而设计的变量

    private bool PlanLock;

    //private void Awake()
    //{
    //    anim = model.GetComponent<Animator>();
    //    pi = GetComponent<PlayerInput>();   
   
    //}



    // Start is called before the first frame update
    void Awake()
    { //                         ============     获取当前物体的组件   ===================
        anim = model.GetComponent<Animator>();
        pi = GetComponent<PlayerInput>();
        rigid = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    { //                             ===========   转向转换缓冲区      ============
        //1.动画转换缓冲
        RunTurn = ((pi.run) ? 2.0f : 1.0f);
        anim.SetFloat("forward", pi.dL * Mathf.Lerp(anim.GetFloat("forward"),RunTurn,0.3f));//Mathf.Lerp(线性插值)让动画参数"forward"在走路和跑步之间平滑过渡的，实际上是由1增加到2

        //2.人物转向缓冲
        if (pi.dL > 0.1f) //添加这个判断，是为了，避免当玩家没有输入时，他的TargetDug和TargetDturn的变为零，导致角色的面朝方向变为0,0
        {

            CharacterTurn = Vector3.Slerp(model.transform.forward, pi.dV, 0.3f);//Vector3.Slerp（ 球面插值）是用来做人物转向缓冲的
            model.transform.forward = CharacterTurn;
        }

        //                  ==================     移动   ==================
        //1.移动，还使用到FixedUpdate方法
        if (PlanLock == false) 
        {
            planVc = pi.dL * model.transform.forward * movingSpeed * ((pi.run) ? RunMultiplier : 1.0f);//角色最终要移动的向量
        }

        //2.跳跃
        if (pi.jump)
        {
            anim.SetTrigger("jump");
        }

    }
    private void FixedUpdate()
    {
        // rigid.position +=  planVc * Time.fixedDeltaTime;//让刚体移动,但这种方法是直接改变刚体的位置的，有可能造成穿越地形的情况
        //所以我们改为
        //rigid.velocity = planVc;//我们修改刚体的速度
        //但是这样也不行，会制覆盖 Y 轴速度，导至角色下落慢一拍
        rigid.velocity = new Vector3(planVc.x, rigid.velocity.y, planVc.z) + JumpImpulse;
        JumpImpulse = Vector3.zero;

    }
                    // ==================      跳跃动作状态的显示     ============================
    public void OnJumpEnter()
    {
        pi.InputEnable = false;
        PlanLock = true;
        JumpImpulse = new Vector3 (0,JunmpHight,0);
        print("YES");
    }

    public void OnJumpExit()
    {
        pi.InputEnable = true;
        PlanLock = false;
    }
}
