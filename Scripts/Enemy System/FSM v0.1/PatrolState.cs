using UnityEngine;

namespace EnemySystem.FSM.v0_1
{
    public class PatrolState : BaseState
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private Transform[] patrolPoints;

        private int _currentPatrolIndex = 0;

        private void Update()
        {
            Patrolling();
        }

        private void Patrolling()
        {
            if (patrolPoints.Length == 0)
            {
                Debug.LogError("PatrolPoints.Length is Null!");
                return;
            }

            Transform targetPoint = patrolPoints[_currentPatrolIndex];
            Vector3 direction = (targetPoint.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
            transform.LookAt(targetPoint);

            if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
            {
                // todo: здесь можно задать разные поведения патрулирования 
                int prev_index = _currentPatrolIndex;
                _currentPatrolIndex = Random.Range(0, patrolPoints.Length);

                if (prev_index == _currentPatrolIndex)
                    _currentPatrolIndex = (prev_index + 1) % patrolPoints.Length;
            }
        }

        public override void EnterState(ZombieFSM controller, Animator animator)
        {
            this.animator = animator;
            this.controller = controller;

            if (animator != null)
                animator.SetFloat("Speed", 1f);
            else
                Debug.Log($"{gameObject.name} - state: Patrol - Animator is Null!");
        }

        public override void ExitState()
        {
            if (animator != null)
                animator.SetFloat("Speed", 0f);
        }
    }
}
