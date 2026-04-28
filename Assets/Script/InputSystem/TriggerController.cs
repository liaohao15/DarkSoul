using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerController : MonoBehaviour
{
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    public void cleanTrigger(string triggername)
    {
        anim.ResetTrigger(triggername);
    }
}
