using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharAnimation : MonoBehaviour
{
    public readonly string X_PARAM = "MoveX";
    public readonly string Y_PARAM = "MoveY";
    public readonly string ISGROUNDED_PARAM = "IsGrounded";
    public readonly string ISDUCKING_PARAM = "IsDucking";
    public readonly string FIST_1 = "Fist";
    public readonly string KICK_1 = "Leg Sweep";
    public readonly string BLOCK_PARAM = "IsBlocking";
    public readonly string JUMP = "Lompat Revisi";
    public readonly string DEAD_PARAM = "Dead";
    public readonly string SET_EMPTY_TRIGGER = "SetEmpty";
    [HideInInspector] public Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
