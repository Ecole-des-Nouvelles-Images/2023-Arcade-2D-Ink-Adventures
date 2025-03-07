using System;
using UnityEngine;

namespace Common
{
    public static class GameEvents
    {
        // Color Change
        public static Action<Color> OnColorChanged;
        
        // Player Death
        public static Action OnPlayerDeath;
        
        // Player Climbing
        public static Action<Transform> OnPlayerClimb;
        public static Action<Transform> OnPlayerStopClimb;


    }
}