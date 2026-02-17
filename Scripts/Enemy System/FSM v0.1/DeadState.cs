using System.Collections;
using UnityEngine;

namespace EnemySystem.FSM.v0_1
{
    public class DeadState : BaseState
    {
        [Header("Death Settings")]
        [SerializeField] private EAfterDeadState afterDeadState = EAfterDeadState.Destoy;
        [SerializeField] private float destroyDelay = 3f;

        private Coroutine _delayCoroutine = null;

        private IEnumerator DelayRoutine()
        {
            yield return new WaitForSeconds(destroyDelay);
            controller.MuteMyselft(afterDeadState);
        }

        public override void EnterState(ZombieFSM controller, Animator animator)
        {
            if (_delayCoroutine != null)
            {
                StopCoroutine(_delayCoroutine);
            }

            _delayCoroutine = StartCoroutine(DelayRoutine());

            this.controller = controller;
            this.animator = animator;

            if (animator != null)
            {
                // Сбросить все параметры перед смертью
                animator.ResetTrigger("TakeDamage");
                animator.SetFloat("Speed", 0f);

                // Запустить смерть через триггер
                animator.SetTrigger("Die");
            }
            else
            {
                Debug.Log($"{gameObject.name} - state: Dead - Animator is Null!");
            }
        }

        public override void ExitState()
        {
            if (_delayCoroutine != null)
            {
                StopCoroutine(_delayCoroutine);
                _delayCoroutine = null;
            }
        }
    }

    public enum EAfterDeadState
    {
        Destoy,
        Mute,
        SetActiveFalse
    }
}
