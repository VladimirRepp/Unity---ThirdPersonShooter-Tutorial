using UnityEngine;

namespace EnemySystem.FSM.v0_1
{
    public abstract class BaseState : MonoBehaviour
    {
        protected ZombieFSM controller;
        protected Animator animator;

        public abstract void EnterState(ZombieFSM controller, Animator animator);

        public abstract void ExitState();
    }
}