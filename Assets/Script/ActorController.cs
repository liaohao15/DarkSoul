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
        anim.SetFloat("forward",pi.Dup);
    }
}
