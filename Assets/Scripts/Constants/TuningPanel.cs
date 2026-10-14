using UnityEngine;

public class TuningPanel : MonoBehaviour
{
    //[Header("Only active while this GameObject is enabled")]

    //[Header("Ground check")]
    //[SerializeField] private float feetRadius = 0.49f;
    //[SerializeField] private float groundCheckDistance = 0.7f;

    //[Header("Movement")]
    //[SerializeField] private float maxSpeed = 30f;
    //[SerializeField] private float chargeJumpMoveSpeed = 0.2f;
    //[SerializeField] private float moveSpeed = 3.5f;

    //[Header("Drag")]
    //[SerializeField] private float landDrag = 21.5f;
    //[SerializeField] private float airDrag = 1f;
    //[SerializeField] private float wallDrag = 27.9f;
    //[SerializeField] private float chargeJumpAirDrag = 1f;
    //[SerializeField] private float startJumpDrag = 100f;

    //[Header("Gravity")]
    //[SerializeField] private float landGravityScale = 1f;
    //[SerializeField] private float airGravityScale = 7.5f;
    //[SerializeField] private float wallGravityScale = 0f;

    //[Header("Move speed multiplier")]
    //[SerializeField] private float defaultMoveSpeedMultiplier = 1f;
    //[SerializeField] private float startJumpMultiplier = 0f;
    //[SerializeField] private float airSpeedMultiplier = 0.15f;

    //[Header("Jump")]
    //[SerializeField] private float normalJumpForce = 22f;
    //[SerializeField] private float wallJumpForce = 32f;
    //[SerializeField] private float bufferTime = 0.15f;
    //[SerializeField] private int chargeMultiplier = 15;
    //[SerializeField] private float jumpAngle = 0.5f;

    //[Header("Wall climb")]
    //[SerializeField] private float wallCheckDistance = 2.24f;
    //[SerializeField] private float wallCheckRadius = 0.45f;
    //[SerializeField] private float wallClearance = 0.05f;
    //[SerializeField] private float snapSharpness = 20f;
    private void Update()
    {
        //CharacterConstants.FeetRadius = feetRadius;
        //CharacterConstants.GroundCheckDistance = groundCheckDistance;

        //CharacterConstants.MaxSpeed = maxSpeed;
        //CharacterConstants.ChargeJumpMoveSpeed = chargeJumpMoveSpeed;
        //CharacterConstants.MoveSpeed = moveSpeed;

        //CharacterConstants.LandDrag = landDrag;
        //CharacterConstants.AirDrag = airDrag;
        //CharacterConstants.WallDrag = wallDrag;
        //CharacterConstants.ChargeJumpAirDrag = chargeJumpAirDrag;
        //CharacterConstants.StartJumpDrag = startJumpDrag;
        //CharacterConstants.JumpAngle = jumpAngle;

        //CharacterConstants.LandGravityScale = landGravityScale;
        //CharacterConstants.AirGravityScale = airGravityScale;
        //CharacterConstants.WallGravityScale = wallGravityScale;

        //CharacterConstants.DefaultMoveSpeedMultiplier = defaultMoveSpeedMultiplier;
        //CharacterConstants.StartJumpMultiplier = startJumpMultiplier;
        //CharacterConstants.AirSpeedMultiplier = airSpeedMultiplier;

        //CharacterConstants.NormalJumpForce = normalJumpForce;
        //CharacterConstants.WallJumpForce = wallJumpForce;
        //CharacterConstants.BufferTime = bufferTime;
        //CharacterConstants.ChargeMultiplier = chargeMultiplier;

        //CharacterConstants.WallCheckDistance = wallCheckDistance;
        //CharacterConstants.WallCheckRadius = wallCheckRadius;
        //CharacterConstants.WallClearance = wallClearance;
        //CharacterConstants.SnapSharpness = snapSharpness;
    }
}

//How to use this script:

/*
In CharacterConstants.cs, change public const float LandDrag = 12f; to public static float LandDrag = 12f; 
(switch const for static) for whichever values you're about to tune this session.
Add/enable TuningPanel in the scene, tune live.
Once satisfied, note the final numbers, delete/disable TuningPanel, and change those same lines in CharacterConstants.cs back to const float ... = <final number>;.

Only the fields you're actively tuning right now need to be static — everything else in the file can stay const the whole time, untouched. It's a per-value switch, not a whole-file one.
*/
