using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCharacterAttackState : MonoBehaviour
{
    private CharManager _charManager;
    public bool isPlayed;
    public bool isAttacking;
    public float velocity;
    // Start is called before the first frame update
    void Start()
    {
        _charManager = GetComponentInParent<CharManager>();        
    }

    private void Update()
    {
        if (isPlayed) _charManager._charControl.Move(new Vector3(velocity * _charManager._charControl.frontDirection, 0, 0));
        _charManager._charControl.isAttacking = isAttacking;
    }

    void StartEffect(float effectX)
    {
        velocity = effectX;
        isPlayed = true;
    }

    public void EndEffect()
    {
        isPlayed = false;
    }

    void AttackEffect(int point)
    {
        _charManager._charControl.Attack(point);
        isAttacking = true;

    }

    public void AttackEnd()
    {
        isAttacking = false;
    }

    void ActiveRig()
    {
        _charManager._charControl.headRigLooking.SetActive(true);
        _charManager._charControl.bodyRigLooking.SetActive(true) ;
    }

    void SetNormalCharController()
    {
        _charManager._charControl.NormalCharController();
    }

    void SetUnBlock()
    {
        _charManager._charControl.UnBlock();
    }

    void Jump()
    {
        _charManager._charControl.Jump();
    }

    void EndJump()
    {
        _charManager._charControl.IsGrounded();
    }
}
