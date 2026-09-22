using UnityEngine;

public class TuningPanel : MonoBehaviour
{
    //[Header("Only active while this GameObject is enabled")]
    //[SerializeField] private float landDrag = 12f;
    //[SerializeField] private float airDrag = 6f;
    //// add more fields here as you need to tune them

    private void Update()
    {
        //CharacterConstants.LandDrag = landDrag;
        //CharacterConstants.AirDrag = airDrag;
    }
}

//How to use this script:

/*
In CharacterConstants.cs, change public const float LandDrag = 12f; to public static float LandDrag = 12f; 
(drop the const keyword) for whichever values you're about to tune this session.
Add/enable TuningPanel in the scene, tune live.
Once satisfied, note the final numbers, delete/disable TuningPanel, and change those same lines in CharacterConstants.cs back to const float ... = <final number>;.

Only the fields you're actively tuning right now need to be static — everything else in the file can stay const the whole time, untouched. It's a per-value switch, not a whole-file one.
*/
