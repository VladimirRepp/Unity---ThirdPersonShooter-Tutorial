using System;
using System.Collections;
using UnityEngine;

namespace EnemySystem.FSM.v0_1
{
    [RequireComponent(typeof(BaseDamageable), typeof(SphereCollider))]
    public class ZombieFSM : MonoBehaviour
    {
        [Header("State Settings")]
        [SerializeField] private PatrolState patrolState;
        [SerializeField] private AttackState attackState;
        [SerializeField] private TakeDamageState takeDamageState;
        [SerializeField] private DeadState deadState;

        [Header("Animation Settings")]
        [SerializeField] private Animator animator;

        [Header("Debug views-only")]
        [SerializeField] private EStateEnemy _currentState = EStateEnemy.Startup;

        private BaseDamageable _myDamageable;
        private SphereCollider _detectionTrigger;

        private bool _isLockChangeState = false;
        private GameObject _currentTarget = null;

        // private Coroutine _waitAndChangeStateCoroutine = null; // note: может пригодится при запуске корутины и слежки в конктролере

        public GameObject CurrentTarget => _currentTarget;

        private void Awake()
        {
            _myDamageable = GetComponent<BaseDamageable>();
            _detectionTrigger = GetComponent<SphereCollider>();
        }

        private void OnEnable()
        {
            if (_myDamageable != null)
            {
                _myDamageable.OnHealthChanged += HandleHealthChanged;
                _myDamageable.OnDeath += HandleDeath;
            }
        }

        private void Start()
        {
            StateChanges(EStateEnemy.Patrol);
            _detectionTrigger.radius = attackState.AttackRange;
        }

        private void OnDisable()
        {
            if (_myDamageable != null)
            {
                _myDamageable.OnHealthChanged -= HandleHealthChanged;
                _myDamageable.OnDeath -= HandleDeath;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(attackState.PlayerTag))
            {
                _currentTarget = other.gameObject;
                StateChanges(EStateEnemy.Attack);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(attackState.PlayerTag))
            {
                _currentTarget = null;
                StateChanges(EStateEnemy.Patrol);
            }
        }

        private void HandleHealthChanged(float currentHealth)
        {
            StateChanges(EStateEnemy.TakeDamage);
        }

        private void HandleDeath()
        {
            StateChanges(EStateEnemy.Dead);
        }

        private void ResetAllStates()
        {
            if (patrolState.enabled)
            {
                patrolState.ExitState();
                patrolState.enabled = false;
            }

            if (attackState.enabled)
            {
                attackState.ExitState();
                attackState.enabled = false;
            }

            if (takeDamageState.enabled)
            {
                takeDamageState.ExitState();
                takeDamageState.enabled = false;
            }

            if (deadState.enabled)
            {
                deadState.ExitState();
                deadState.enabled = false;
            }
        }

        private void StateChanges(EStateEnemy newState)
        {
            if (_currentState == EStateEnemy.Dead)
                return;

            if (newState == EStateEnemy.Dead)
            {
                _currentState = newState;

                ResetAllStates();

                deadState.enabled = true;
                deadState.EnterState(this, animator);

                return;
            }

            // Даже при блокированние измения состояния, получение урона все равно доступно 
            if (_isLockChangeState && newState != EStateEnemy.TakeDamage)
                return;

            // Когда текущее сотсояние равно новому - нет смысла перезапускать его же 
            // кроме получения урона 
            if (_currentState == newState && newState != EStateEnemy.TakeDamage)
                return;

            _currentState = newState;
            ResetAllStates();

            if (_currentState == EStateEnemy.TakeDamage)
            {
                takeDamageState.enabled = true;
                takeDamageState.EnterState(this, animator);

                return;
            }

            if (_currentState == EStateEnemy.Attack)
            {
                attackState.enabled = true;
                attackState.EnterState(this, animator);

                return;
            }

            if (_currentState == EStateEnemy.Patrol)
            {
                patrolState.enabled = true;
                patrolState.EnterState(this, animator);

                return;
            }

            if (_currentState == EStateEnemy.Dead)
            {
                deadState.enabled = true;
                deadState.EnterState(this, animator);

                return;
            }
        }


        public IEnumerator WaitAndChangeStateRoutine(EStateEnemy newState, float delay)
        {
            _isLockChangeState = true;

            yield return new WaitForSeconds(delay);
            _isLockChangeState = false;
            StateChanges(newState);
        }

        public void MuteMyselft(EAfterDeadState afterDeadState)
        {
            ResetAllStates();

            switch (afterDeadState)
            {
                case EAfterDeadState.Destoy:
                    Destroy(gameObject);
                    break;
                case EAfterDeadState.Mute:
                    this.enabled = false;
                    break;
                case EAfterDeadState.SetActiveFalse:
                    gameObject.SetActive(false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(afterDeadState), afterDeadState, null);
            }
        }
    }
}
