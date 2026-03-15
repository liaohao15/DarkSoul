using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftArmAimFix : MonoBehaviour
{

    private Animator anim;
    public Vector3 a;
    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    void OnAnimatorIK()
    {
        if (anim.GetBool("defense") == false)
        {
            Transform leftLowerArm = anim.GetBoneTransform(HumanBodyBones.LeftLowerArm);
            leftLowerArm.localEulerAngles += a;
            anim.SetBoneLocalRotation(HumanBodyBones.LeftLowerArm, Quaternion.Euler(leftLowerArm.localEulerAngles));
        }
       
    }

}
