using UnityEngine;

namespace Player
{
    [CreateAssetMenu(menuName = "Player Movement")]
    public class PlayerMovementStats : ScriptableObject
    {
        [Header("Walk")]
        [Range(1f, 100f)] public float MaxWalkSpeed = 10f;
        [Range(0.25f, 50)] public float GroundAcceleration = 5f;
        [Range(0.25f, 50)] public float GroundDeceleration = 5f;
        [Range(0.25f, 50)] public float AirAcceleration = 5f;
        [Range(0.25f, 50)] public float AirDeceleration = 5f;

        /* IF RUN OPTION IS ADDED
         * [Header("Run")]
         * [Range(1f, 100f)] public float MaxRunSpeed = 20f;
         */

        [Header("Grounded/Collision Checks")]
        public LayerMask GroundLayer;
        public float GroundDetectionRayLength = 0.02f;
        public float HeadDetectionRayLength = 0.02f;
        [Range(0f, 1f)] public float HeadWidth = 0.75f;

        public bool DebugShowIsGroundedBox;

    }
}
