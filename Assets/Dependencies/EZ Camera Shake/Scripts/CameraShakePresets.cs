using UnityEngine;

namespace EZCameraShake
{

    public static class CameraShakePresets
    {
        /// <summary>
        /// [One-Shot] A high magnitude, short, yet smooth shake.
        /// </summary>
        public static CameraShakeInstance Bump
        {
            get => new(2.5f, 4, 0.1f, 0.75f)
            {
                PositionInfluence = Vector3.one * 0.15f,
                RotationInfluence = Vector3.one
            };
        }

        /// <summary>
        /// [One-Shot] An intense and rough shake.
        /// </summary>
        public static CameraShakeInstance Explosion
        {
            get => new(5f, 10, 0, 1.5f)
            {
                PositionInfluence = Vector3.one * 0.25f,
                RotationInfluence = new Vector3(4, 1, 1)
            };
        }

        /// <summary>
        /// [Sustained] A continuous, rough shake.
        /// </summary>
        public static CameraShakeInstance Earthquake
        {
            get => new(0.6f, 3.5f, 2f, 10f)
            {
                PositionInfluence = Vector3.one * 0.25f,
                RotationInfluence = new Vector3(1, 1, 4)
            };
        }

        /// <summary>
        /// [Sustained] A bizarre shake with a very high magnitude and low roughness.
        /// </summary>
        public static CameraShakeInstance BadTrip
        {
            get => new(10f, 0.15f, 5f, 10f)
            {
                PositionInfluence = new Vector3(0, 0, 0.15f),
                RotationInfluence = new Vector3(2, 1, 4)
            };
        }

        /// <summary>
        /// [Sustained] A subtle, slow shake. 
        /// </summary>
        public static CameraShakeInstance HandheldCamera
        {
            get => new(1f, 0.25f, 5f, 10f)
            {
                PositionInfluence = Vector3.zero,
                RotationInfluence = new Vector3(1, 0.5f, 0.5f)
            };
        }

        /// <summary>
        /// [Sustained] A very rough, yet low magnitude shake.
        /// </summary>
        public static CameraShakeInstance Vibration
        {
            get => new(0.4f, 20f, 2f, 2f)
            {
                PositionInfluence = new Vector3(0, 0.15f, 0),
                RotationInfluence = new Vector3(1.25f, 0, 4)
            };
        }

        /// <summary>
        /// [Sustained] A slightly rough, medium magnitude shake.
        /// </summary>
        public static CameraShakeInstance RoughDriving
        {
            get => new(1, 2f, 1f, 1f)
            {
                PositionInfluence = Vector3.zero,
                RotationInfluence = Vector3.one
            };
        }
    }
}