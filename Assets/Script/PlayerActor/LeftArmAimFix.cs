using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftArmAimFix : MonoBehaviour
{
    //左手骨骼姿势修正
    //功能：非防御状态下，手动调整左小臂角度，修复动画姿势不自然的问题

    //动画控制器
    private Animator Anim;

    [Header("手臂旋转的偏移角度")]
    public Vector3 A;
    
    //初始化：获取动画组件
    private void Awake()
    {
        Anim = GetComponent<Animator>();
    }

    //动画IK专用方法：专门用来修改角色骨骼姿势
    void OnAnimatorIK()
    {
        //只用不在防御状态的时候，才修正手臂
        if (Anim.GetBool("defense") == false)
        {
            //获取角色的左手小臂骨骼
            Transform leftLowerArm = Anim.GetBoneTransform(HumanBodyBones.LeftLowerArm);
            //叠加设置好的角度，修正姿势
            leftLowerArm.localEulerAngles += A;
            //将修正的角度应用到骨骼上
            Anim.SetBoneLocalRotation(HumanBodyBones.LeftLowerArm, Quaternion.Euler(leftLowerArm.localEulerAngles));
        }
       
    }

}
