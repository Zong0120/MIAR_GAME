using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PlayerInputAction;

public class AnimationOnFinish : StateMachineBehaviour
{
    [SerializeField] private string animationName;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) 
    {
        PlayerController.Instance.ChangeAnimation(animationName,0.2f,animatorStateInfo.length);
    }
}
