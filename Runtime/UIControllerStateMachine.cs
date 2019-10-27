using UnityEngine;

public class UIControllerStateMachine : StateMachineBehaviour
{

    AnimatorStateInfo lastStateInfo;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
		OnStateUpdateInternal(animator, lastStateInfo, stateInfo);
        lastStateInfo = stateInfo;
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
		OnStateUpdateInternal(animator, lastStateInfo, stateInfo);
        lastStateInfo = stateInfo;
    }

    void OnStateUpdateInternal(Animator animator, AnimatorStateInfo oldInfo, AnimatorStateInfo newInfo)
    {
        if (oldInfo.fullPathHash != newInfo.fullPathHash || oldInfo.normalizedTime < 1 && newInfo.normalizedTime >= 1)
        {
            if (oldInfo.IsName("Show"))
            {
                animator.SendMessage("OnShow");
            }
            else if (oldInfo.IsName("Hide"))
            {
                animator.SendMessage("OnHide");
            }
        }
    }
}
