using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActorController : MonoBehaviour
{
    public GameObject model;
    public PlayerInput pi;

    [SerializeField]
    private Animator anim;

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
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetFloat("forward", pi.dL);//通过向量的模长的方法，将横坐标和竖坐标转换成模长
        model.transform.forward = pi.dV;//将角色面朝的方向，设为横坐标和竖坐标向量和
    }
}
