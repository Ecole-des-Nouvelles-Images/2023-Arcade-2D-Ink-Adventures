using UnityEngine;

namespace FSM
{
    public class PlayerTestState : PlayerBaseState, IRootState
    {
        public PlayerTestState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
        {
            IsRootState = true;
        }

        public override void EnterState()
        {
            Debug.Log("Enter Test State");
        }

        public override void UpdateState()
        {
            Debug.Log("Update Test State");
        }

        public override void FixedUpdateState()
        {
        }

        public override void ExitState()
        {
        }

        public override void CheckSwitchStates()
        {
        }

        public override void InitializeSubState()
        {
        }

        public override void OnTriggerEnter2D(Collider2D other)
        {
        }

        public override void OnTriggerStay2D(Collider2D other)
        {
        }

        public override void OnTriggerExit2D(Collider2D other)
        {
        }

        public void HandleGravity()
        {
        }
    }
}
