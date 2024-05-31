using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerController : CharController
{
    void Awake()
    {
        charController = GetComponent<CharacterController>(); // Set Character Controller
        charHeight = charController.height; // Set Tinggi Default Character Controller
        charCenter = charController.center; // Set Lokasi Default Character Controller
        _charManager = GetComponent<CharManager>(); // Set Character Manager   
        RasioAttack = 0;
    }

    // Update is called once per frame
    void Update()
    {
        CharacterDirection();
        SetCharStable();
        PlayerMove();
    }

    // Pergerakan
    void PlayerMove()
    {
        float y = Input.GetAxisRaw("Vertical");
        bool inputDuckingDown = Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow);
        bool inputDuckingUp = Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.DownArrow);
        bool inputBlockingDown = Input.GetMouseButtonDown(2) || Input.GetKeyDown(KeyCode.J);
        bool inputBlockingUp = Input.GetMouseButtonUp(2) || Input.GetKeyUp(KeyCode.J);

        // Set Anim State
        _charManager._charAnim.anim.SetBool(_charManager._charAnim.ISGROUNDED_PARAM, IsGrounded());
        _charManager._charAnim.anim.SetBool(_charManager._charAnim.ISDUCKING_PARAM, isDucking);
        _charManager._charAnim.anim.SetBool(_charManager._charAnim.BLOCK_PARAM, isBlocking);

        // Cek Kondisi Player
        if (IsGrounded())
        {
            if (isStable && !isAttacking && !isDucking)
            {
                _charManager._changeCharacterAttackState.isPlayed = false;

                // Player Attack
                if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.H)) PlayerAttack();

                if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.K)) FootAttack();

                // Blocking Attack
                if (inputBlockingDown) Block();

                // Player Ducking
                if (inputDuckingDown) Ducking();
            }

            // Set False
            if (inputBlockingUp) UnBlock();
            if (inputDuckingUp) NormalCharController();

            // Set Velociity Y agar Kembali 0
            if (!isJump) characterVelocity.y = 0;

            // Lompat
            if (Input.GetButtonDown("Jump") || y > 0) _charManager._charAnim.anim.CrossFade(_charManager._charAnim.JUMP, .1f);

            // Move Kiri Kanan Player
            if (!isDucking && !isBlocking) characterVelocity.x = Input.GetAxis("Horizontal");
            else characterVelocity.x = 0;

            // Set IsMove
            if (characterVelocity.x != 0) isMove = true;
            else isMove = false;
        }
        else
        {
            // Kecepatan Maksimal saat Terjatuh
            if (characterVelocity.y < -9.8f) characterVelocity.y = -gravityForce;
            // Membuat Effect Smooth saat Lompat atau Terjatuh
            else characterVelocity.y -= Time.deltaTime * gravityForce;

            // Set Animasi Menunduk False
            isDucking = false;
        }

        // Blokir Pergerakan Saat Menyerang
        if (!isAttacking) Move(characterVelocity);

        // Set Animasi Jalan
        SetAnimationLocomotor(characterVelocity);

        // Set Posisi Body;
        if (characterVelocity.x == 0) headRigLooking.SetActive(false);
        else headRigLooking.SetActive(true);
    }
}
