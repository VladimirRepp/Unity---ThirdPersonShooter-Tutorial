using System.Collections;
using UnityEngine;

namespace EnemySystem.FSM.v0_1
{
    public class AttackState : BaseState
    {
        [Header("Attack Settings")]
        [SerializeField] private string playerTag = "Player";
        [SerializeField] private float attackRange = 1f;
        [SerializeField] private float attackCooldown = 1f;
        [SerializeField] private int damage = 10;

        private Coroutine _attackCoroutine = null;

        public float AttackRange => attackRange;
        public int Damage => damage;
        public string PlayerTag => playerTag;

        private IEnumerator AttackRoutine()
        {
            while (true)
            {
                if (controller.CurrentTarget == null)
                {
                    yield break;
                }

                transform.LookAt(controller.CurrentTarget.transform);

                if (controller.CurrentTarget.TryGetComponent<BaseDamageable>(out BaseDamageable damageable))
                    damageable.TakeDamage(damage);

                // Атака с передышкой
                if (animator != null)
                    animator.SetTrigger("Attack");
                else
                    Debug.Log($"{gameObject.name} - state: Attack - Animator is Null!");

                yield return new WaitForSeconds(attackCooldown);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }

        public override void EnterState(ZombieFSM controller, Animator animator)
        {
            this.animator = animator;
            this.controller = controller;

            if (animator != null)
                animator.SetFloat("Speed", 0f);
            else
                Debug.Log($"{gameObject.name} - state: Attack - Animator is Null!");

            if (_attackCoroutine != null)
                StopCoroutine(_attackCoroutine);

            _attackCoroutine = StartCoroutine(AttackRoutine());
        }

        public override void ExitState()
        {
            if (_attackCoroutine != null)
            {
                StopCoroutine(_attackCoroutine);
                _attackCoroutine = null;
            }
        }
    }
}
