using System.Collections.Generic;
using UnityEngine;

namespace FSM
{
    enum PlayerStates
    {
        grounded,
        idle,
        walk,
        air,
        jump,
        fall,
        test
    }
    public class PlayerStateFactory
    {
        private PlayerStateMachine _context;
        Dictionary<PlayerStates, PlayerBaseState> _states = new Dictionary<PlayerStates, PlayerBaseState>();
        public PlayerStateFactory(PlayerStateMachine currentContext)
        {
            _context = currentContext;
           _states[PlayerStates.grounded] = new PlayerGroundedState(_context, this);
           _states[PlayerStates.idle] = new PlayerIdleState(_context, this);
           _states[PlayerStates.walk] = new PlayerWalkState(_context, this);
           _states[PlayerStates.air] = new PlayerInAirState(_context, this);
           _states[PlayerStates.jump] = new PlayerJumpState(_context, this);
           _states[PlayerStates.fall] = new PlayerFastFallState(_context, this);
           _states[PlayerStates.test] = new PlayerTestState(_context, this);

        }

        public PlayerBaseState Grounded()
        {
            return _states[PlayerStates.grounded];
        }

        public PlayerBaseState Idle()
        {
            return _states[PlayerStates.idle];
        }

        public PlayerBaseState Walk()
        {
            return _states[PlayerStates.walk];
        }

        public PlayerBaseState InAir()
        {
            return _states[PlayerStates.air];
        }

        public PlayerBaseState Jump()
        {
            return _states[PlayerStates.jump];
        }

        public PlayerBaseState Fall()
        {
            return _states[PlayerStates.fall];
        }

        public PlayerBaseState Test()
        {
            return _states[PlayerStates.test];
        }
    }
}
