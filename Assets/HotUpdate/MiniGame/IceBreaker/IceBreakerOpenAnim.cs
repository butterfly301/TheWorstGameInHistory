using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate.MiniGame.IceBreaker
{
    public class IceBreakerOpenAnim : StateMachineBehaviour
    {
        private Button button;

        private Animator thisAnimator;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            thisAnimator = animator;
            button = animator.GetComponentInChildren<Button>();
            button.onClick.AddListener(StartGame);
            button.gameObject.SetActive(false);
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    
        //}

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            button.gameObject.SetActive(true);
        }

        private void StartGame()
        {
            IceBreakerManager.Instance.StartGame();
            thisAnimator.gameObject.SetActive(false);
        }

        // OnStateMove is called right after Animator.OnAnimatorMove()
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that processes and affects root motion
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK()
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that sets up animation IK (inverse kinematics)
        //}
    }
}