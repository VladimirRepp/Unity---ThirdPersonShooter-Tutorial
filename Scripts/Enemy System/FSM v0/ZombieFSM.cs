using System.Collections;
using UnityEngine;

namespace EnemySystem.FSM.v0
{
    [RequireComponent(typeof(BaseDamageable), typeof(SphereCollider))]
    public class ZombieFSM : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private Transform[] patrolPoints;

        [Header("Attack Settings")]
        [SerializeField] private string playerTag = "Player";
        [SerializeField] private float attackRange = 1f;
        [SerializeField] private float attackCooldown = 1f;
        [SerializeField] private int damage = 10;

        [Header("Animation Settings")]
        [SerializeField] private Animator animator;

        [Header("Debug views-only")]
        [SerializeField] private EStateEnemy _currentState = EStateEnemy.Startup;

        private BaseDamageable _myDamageable;
        private SphereCollider _detectionTrigger;

        private bool _isLockChangeState = false;
        private int _currentPatrolIndex = 0;
        private GameObject _currentTarget = null;

        private Coroutine _attackCoroutine = null;
        private Coroutine _waitAndChangeStateCoroutine = null;

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
            _detectionTrigger.radius = attackRange;
        }

        private void OnDisable()
        {
            if (_myDamageable != null)
            {
                _myDamageable.OnHealthChanged -= HandleHealthChanged;
                _myDamageable.OnDeath -= HandleDeath;
            }
        }

        private void Update()
        {
            if (_currentState == EStateEnemy.Patrol)
                Patrolling();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(playerTag))
            {
                _currentTarget = other.gameObject;
                StateChanges(EStateEnemy.Attack);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(playerTag))
            {
                _currentTarget = null;
                StateChanges(EStateEnemy.Patrol);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }

        private void HandleHealthChanged(float currentHealth)
        {
            StateChanges(EStateEnemy.TakeDamage);
        }

        private void HandleDeath()
        {
            StateChanges(EStateEnemy.Dead);
        }

        private void StateChanges(EStateEnemy newState)
        {
            if (_currentState == EStateEnemy.Dead)
                return;

            if (newState == EStateEnemy.Dead)
            {
                _currentState = newState;
                DeadState();
                return;
            }

            // Даже при блокированние измения состояния, получение урона все равно доступно 
            if (_isLockChangeState && newState != EStateEnemy.TakeDamage)
                return;

            // Когда текущее сотсояние равно новому - нет смысла перезапускать его же 
            // кроме получения урона 
            if (_currentState == newState && newState != EStateEnemy.TakeDamage)
                return;

            if (_currentState == EStateEnemy.Attack &&
            newState != EStateEnemy.Attack)
            {
                if (_attackCoroutine != null)
                    StopCoroutine(_attackCoroutine);

                _attackCoroutine = null;
                _currentTarget = null;
            }

            _currentState = newState;

            if (_currentState == EStateEnemy.TakeDamage)
            {
                TakeDamageState();
                return;
            }

            if (_currentState == EStateEnemy.Attack)
            {
                AttackState(_currentTarget); // наглядно передаем, можно не создавать новую ссылку ref 
                return;
            }

            if (_currentState == EStateEnemy.Patrol)
            {
                PatrolState();
                // Patrolling(); - called in Update
                return;
            }

            if (_currentState == EStateEnemy.Dead)
            {
                DeadState();
                return;
            }
        }

        #region === State Methods ===
        private void PatrolState()
        {
            if (animator != null)
                animator.SetFloat("Speed", 1f);
            else
                Debug.Log($"{gameObject.name} - state: {_currentState}");
        }

        private void Patrolling()
        {
            if (patrolPoints.Length == 0)
            {
                Debug.LogError("ZombieFSM.Patrolling: PatrolPoints.Length is Null!");
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

        private void AttackState(GameObject currentTarget)
        {
            if (animator != null)
                animator.SetFloat("Speed", 0f);
            else
                Debug.Log($"{gameObject.name} - state: {_currentState}");

            if (_attackCoroutine != null)
                StopCoroutine(_attackCoroutine);

            _attackCoroutine = StartCoroutine(AttackRoutine(currentTarget));
        }

        private IEnumerator AttackRoutine(GameObject currentTarget)
        {
            while (true)
            {
                if (currentTarget == null)
                {
                    StateChanges(EStateEnemy.Patrol);
                    yield break;
                }

                transform.LookAt(currentTarget.transform);

                if (currentTarget.TryGetComponent<BaseDamageable>(out BaseDamageable damageable))
                    damageable.TakeDamage(damage);

                // Атака с передышкой
                if (animator != null)
                    animator.SetTrigger("Attack");
                else
                    Debug.Log($"{gameObject.name} - state: {_currentState}");

                yield return new WaitForSeconds(attackCooldown);
            }
        }

        private void TakeDamageState()
        {
            if (_waitAndChangeStateCoroutine != null)
            {
                StopCoroutine(_waitAndChangeStateCoroutine);
                _waitAndChangeStateCoroutine = null;
            }

            _waitAndChangeStateCoroutine = StartCoroutine(WaitAndChangeStateRoutine(
                _currentTarget != null ? EStateEnemy.Attack : EStateEnemy.Patrol,
               1f));

            if (animator != null)
            {
                animator.SetFloat("Speed", 0f);
                animator.SetTrigger("TakeDamage");
            }
            else
            {
                Debug.Log($"{gameObject.name} - state: {_currentState}");
            }
        }

        private IEnumerator WaitAndChangeStateRoutine(EStateEnemy newState, float delay)
        {
            _isLockChangeState = true;

            yield return new WaitForSeconds(delay);
            _isLockChangeState = false;
            StateChanges(newState);
        }

        private void DeadState()
        {
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
                Debug.Log($"{gameObject.name} - state: {_currentState}");
            }

            this.enabled = false;
        }
        #endregion
    }
}