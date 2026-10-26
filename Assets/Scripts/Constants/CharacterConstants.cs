public static class CharacterConstants
{
    // ---------- Ground check ----------
    public const float FeetRadius = 0.49f;
    public const float GroundCheckDistance = 0.7f;

    // ---------- Movement ----------
    public const float MaxSpeed = 30f;
    public const float ChargeJumpMoveSpeed = 0.2f;
    public const float MoveSpeed = 3.5f;

    // ---------- Drag ---------
    public const float LandDrag = 21.5f;
    public const float AirDrag = 1f; 
    public const float WallDrag = 27.9f;
    //public const float ChargeJumpAirDrag = 1f;
    public const float StartJumpDrag = 100;

    // ---------- Gravity ---------
    public const float LandGravityScale = 1f;
    public const float AirGravityScale = 7.5f;
    public const float WallGravityScale = 0f; //yes its supposed to be 0

    // ---------- Move speed multiplier ----------
    public const float DefaultMoveSpeedMultiplier = 1;
    public const float StartJumpMultiplier = 0;
    public const float AirSpeedMultiplier = 0.15f;

    // ---------- Jump ----------
    public const float NormalJumpForce = 25f;
    public const float WallJumpForce = 32f;
    public const float BufferTime = 0.15f;
    public const int ChargeMultiplier = 15;
    public const float JumpAngle = 0.75f; //lower value for bigger angle, higher value for smaller angle
    public const float AirLandingGrace = 0.1f;

    // ---------- Wall climb ----------
    public const float WallCheckDistance = 2.24f;
    public const float WallCheckRadius = 0.45f;
    public const float WallClearance = 0.05f;
    public const float SnapSharpness = 20f;

    // ---------- Wall attach ----------                 // ADD
    public const float WallAttachSpeed = 15f;
    public const float WallAttachArriveDistance = 0.1f;
    public const float WallAttachTimeout = 0.5f;          // safety exit if something blocks the path
    public const float WallAttachGroundLift = 1f; 
}
