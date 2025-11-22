using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class ActorController : MonoBehaviour
{
    public GameObject model;
    public PlayerInput pi;
    public  float movingSpeed = 2.0f; 

    [SerializeField]
    private Animator anim;
    [SerializeField]
    private Rigidbody rigid;
    private Vector3 movingVt;
    private float RunMultiplier = 2.0f;

    //private void Awake()
    //{
    //    anim = model.GetComponent<Animator>();
    //    pi = GetComponent<PlayerInput>();   
   
    //}



    // Start is called before the first frame update
    void Awake()
    {
        anim = model.GetComponent<Animator>();
        pi = GetComponent<PlayerInput>();
        rigid = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetFloat("forward", pi.dL *((pi.run) ? 2.0f: 1.0f));//通过向量的模长的方法，将横坐标和竖坐标转换成模长
        if (pi.dL > 0.1f) //添加这个判断，是为了，避免当玩家没有输入时，他的TargetDug和TargetDturn的变为零，导致角色的面朝方向变为0,0
        {
            model.transform.forward = pi.dV;//将角色面朝的方向，设为横坐标和竖坐标向量和
        }
        movingVt = pi.dL * model.transform.forward * movingSpeed * ((pi.run)?RunMultiplier:1.0f);//角色最终要移动的向量


    }
    private void FixedUpdate()
    {
        // rigid.position +=  movingVt * Time.fixedDeltaTime;//让刚体移动,但这种方法是直接改变刚体的位置的，有可能造成穿越地形的情况
        //所以我们改为
        //rigid.velocity = movingVt;//我们修改刚体的速度
        //但是这样也不行，会制覆盖 Y 轴速度，导至角色下落慢一拍
        rigid.velocity = new Vector3(movingVt.x, rigid.velocity.y, movingVt.z);





    }



}
