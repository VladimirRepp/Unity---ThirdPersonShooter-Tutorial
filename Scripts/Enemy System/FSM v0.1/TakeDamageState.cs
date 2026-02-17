using UnityEngine;

namespace EnemySystem.FSM.v0_1
{
    public class TakeDamageState : BaseState
    {
        private Coroutine _waitAndChangeStateCoroutine;

        public override void EnterState(ZombieFSM controller, Animator animator)
        {
            this.animator = animator;

            if (_waitAndChangeStateCoroutine != null)
            {
                StopCoroutine(_waitAndChangeStateCoroutine);
                _waitAndChangeStateCoroutine = null;
            }

            _waitAndChangeStateCoroutine = StartCoroutine(controller.WaitAndChangeStateRoutine(
                controller.CurrentTarget != null ? EStateEnemy.Attack : EStateEnemy.Patrol,
               1f));

            if (animator != null)
            {
                animator.SetFloat("Speed", 0f);
                animator.SetTrigger("TakeDamage");
            }
            else
            {
                Debug.Log($"{gameObject.name} - state: TakeDamage - Animator is Null!");
            }
        }

        public override void ExitState()
        {
            if (_waitAndChangeStateCoroutine != null)
            {
                StopCoroutine(_waitAndChangeStateCoroutine);
                _waitAndChangeStateCoroutine = null;
            }
        }
    }
}