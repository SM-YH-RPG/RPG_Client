using UnityEngine;

public class PlayerMovement : IMovement
{
    public bool IsGrounded { get; private set; }

    private const float GroundCheckDistance = 0.1f;

    public void Initialize(CharacterConfig data)
    {

    }

    public void ApplyMovement(CharacterController characterController, Vector3 velocity, float deltaTime)
    {
        characterController.Move(velocity * deltaTime);

        IsGrounded = characterController.isGrounded;
    }

    public void HandleGroundSnapping(ref Vector3 velocity, float groundedOffset)
    {
        if (IsGrounded)
        {
            velocity.y = groundedOffset;
        }
    }

    public Vector3 CalculateSlideVector(Vector3 currentVelocity, Vector3 groundNormal, float maxSlopeAngle, float slopeSlideSpeed)
    {
        float angle = Vector3.Angle(Vector3.up, groundNormal);
        if (angle > maxSlopeAngle)
        {
            Vector3 slideDirection = Vector3.ProjectOnPlane(Vector3.down, groundNormal).normalized;
            return slideDirection * slopeSlideSpeed;
        }
        return currentVelocity;
    }
}
