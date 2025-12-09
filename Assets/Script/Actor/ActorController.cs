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
    public float JunmpHight = 4.5f;//向上跳跃的高度
    public float fallSpeed = 7.0f;//下落速度
    public bool isFall = false;//标记是否下落
    private bool isGround = true;//标记是否在地面
    public float RollHight = 1.5f;//向上翻滚的高度
    public Vector3 JabImpulse;//后跳的冲量
    public float JabHight = 10.0f;//后跳的高度

    [SerializeField]
    private Animator anim;//获取组件Animator
    [SerializeField]
    private Rigidbody rigid;//获取刚体

    private Vector3 planVc;//角色移动的最终量
    private float RunMultiplier = 2.0f;//当跑步键按下时，乘以这个速度倍率

    private Vector3 CharacterTurn;//为角色转向而设计的变量
    private float RunTurn;//为动画切换而设计的变量

    private bool PlanLock;



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

        //3.下落
        if (!isGround && rigid.velocity.y < 0)
        {
            isFall = true;
            if (isFall)
            {
                anim.SetBool("isfall",isFall);
            }
        }

        //4.落地翻滚在Ingroud里面
        //4.下落翻滚
        if (rigid.velocity.magnitude > 4.6f && isGround)
        {
            anim.SetTrigger("roll");
        }


    }
    private void FixedUpdate()
    {
        //1.移动
        rigid.velocity = new Vector3(planVc.x, rigid.velocity.y, planVc.z) + JumpImpulse + JabImpulse;
        //2.跳跃
        JumpImpulse = Vector3.zero;
        JabImpulse = Vector3.zero;

    }
   
    



    //                          ******************************  信息接收区    *************************************
    //                              ==================      跳跃动作状态的显示     ============================
    public void OnJumpEnter()
    {
        pi.InputEnable = false;
        PlanLock = true;
        JumpImpulse = model.transform.forward;
    }

    //可能要删调这个了
    public void OnJumpExit()
    { }


    //                           ==================      人物下落检测区    ============================
    public void Ingroud()
    {
        print("is groud");
        isGround = true;
        anim.SetBool("isgroud", true);

        pi.InputEnable = true;
        PlanLock = false;
        anim.SetBool("isfall", false);

        //3.下落
        if (isFall && rigid.velocity.y > -fallSpeed)
        {
            rigid.velocity += Vector3.down * fallSpeed * Time.fixedDeltaTime;
        }
      
    }

    public void NotIngroud()
    {
        print("not is groud");
        isGround = false;
        anim.SetBool("isgroud", false);
    }
    //                              ==================      翻滚动作状态的显示     ============================
    public void OnRollEnter()
    {
        pi.InputEnable = false;
        PlanLock = true;
        JumpImpulse = new Vector3(0, RollHight, 0);
    }

    //                              ==================      后跳动作状态的显示     ============================
    public void OnJabEnter()
    {
        pi.InputEnable = false;
        PlanLock = true;
        JabImpulse = model.transform.forward * (-1) * JabHight;
    }
}
