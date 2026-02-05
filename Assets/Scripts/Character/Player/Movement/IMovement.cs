using UnityEngine;

public interface IMovement
{
    void Initialize(CharacterConfig data);

    void ApplyMovement(CharacterController characterController, Vector3 velocity, float deltaTime);
    
    bool IsGrounded { get; }
    void HandleGroundSnapping(ref Vector3 velocity, float groundedOffset);
    Vector3 CalculateSlideVector(Vector3 currentVelocity, Vector3 groundNormal, float maxSlopeAngle, float slopeSlideSpeed);
}