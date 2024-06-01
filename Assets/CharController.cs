using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharController : MonoBehaviour
{
    [Header("Script")]

    protected CharManager _charManager; // Script Character Manager

    [Header("Character State")]

    public bool isDucking; // Menunduk
    public bool isMove; // Bergerak
    public bool isJump; // Lompat
    public bool isBlocking; // Block Serangan
    public bool isStable; // Posisi Stabil
    public bool isAttacking; // Posisi Menyerang
    public bool isDead; // Mati

    [Header("Player Move")]

    [SerializeField] protected LayerMask enemyLayer; // Layer Musuh
    [SerializeField] protected Transform enemy; // Posisi Musuh
    [SerializeField] protected float speed; // Kecepatan Bergerak
    [SerializeField] protected float jumpForce; // Kekuatan Lompatan
    protected CharacterController charController; // Character Controller
    public Vector3 characterVelocity; // Simpan Kecepatan Character untuk Character Controller
    public GameObject headRigLooking; // Rigging Head
    public GameObject bodyRigLooking; // Rigging Body
    [HideInInspector] public int frontDirection = 1; // Arah Rotasi Player

    [Header("Gravity Physic")]

    [SerializeField] protected Transform groundChecker; // Posisi Ground Checker
    [SerializeField] protected LayerMask groundLayer; // Layer Ground
    [SerializeField] protected float checkRadius; // Radius Grand Checker
    [SerializeField] protected float gravityForce; // Kekuatan Gravitasi

    [Header("Attack")]
    [SerializeField] protected Transform[] attackPoint; // Posisi Serangan
    [SerializeField] protected float attackRadius; // Radius Serangan
    [SerializeField] protected int minDamage, maxDamage; // Set minmax Damage
    public float totalAttack;
    public float rasioAttack;
    public float attackSuccess;
    public int attackChain;
    public float RasioAttack
    {
        get => rasioAttack;
        set
        {
            rasioAttack = value;
        }
    }

    [Header("Change Height")]

    protected float charHeight; // Ketinggian Character Default
    protected Vector3 charCenter; // Lokasi Character Controller Relative dengan Game Object
    [SerializeField] protected float characterControllerHeightOnDucking; // Ketinggian Saat Menunduk
    [SerializeField] protected float characterControllerYOffset; // Lokasi Character Controller Saat Menunduk


    // Start is called before the first frame update
    //void Start()
    //{
    //    charController = GetComponent<CharacterController>(); // Set Character Controller
    //    charHeight = charController.height; // Set Tinggi Default Character Controller
    //    charCenter = charController.center; // Set Lokasi Default Character Controller
    //    _charManager = GetComponent<CharManager>(); // Set Character Manager        
    //}

    // Update is called once per frame
    void Update()
    {
        CharacterDirection();
        SetCharStable();
    }

    // Set Animasi Pergerakan Character
    protected void SetAnimationLocomotor(Vector3 move)
    {
        _charManager._charAnim.anim.SetFloat(_charManager._charAnim.X_PARAM, move.x * frontDirection); // Set Animasi Berjalan
        _charManager._charAnim.anim.SetFloat(_charManager._charAnim.Y_PARAM, move.y); // Set Animsai Lompat dan Jatuh
    }

    // Set Arah Badan Character
    protected void CharacterDirection()
    {
        if (isStable)
        {
            // Set Lokasi Musuh
            Vector3 relativePos = new Vector3(enemy.position.x - transform.position.x, 0, 0);

            // Set Rotasi Player terhadap Musuh
            Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
            transform.rotation = rotation;

            // Set Arah Pergerakan
            if (transform.rotation.y < 0) frontDirection = -1;
            else frontDirection = 1;

            // Set Body Looking
            if (isMove) bodyRigLooking.SetActive(true);
            else bodyRigLooking.SetActive(false);
        }
        else bodyRigLooking.SetActive(false);
    }

    // Set Ketinggian Collider saat Menunduk
    protected void Ducking()
    {
        charController.height = characterControllerHeightOnDucking; // Ubah Ketinggian Character Controller
        charController.center = new Vector3(charController.center.x, characterControllerYOffset, charController.center.z); // Ubah Posisi Character Controller
        isDucking = true; // Set Menunduk
    }

    // Set Ketinggian Collider Menjadi Normal
    public void NormalCharController()
    {
        charController.height = charHeight; // Ubah Ketinggian Character Controller
        charController.center = charCenter; // Ubah Posisi Character Controller
        isDucking = false; // Set Menunduk False
    }

    // Control Pergerakan Player
    public void Move(Vector3 move)
    {
        charController.Move(move * speed * Time.deltaTime);
    }

    // Set Kondisi Stabil
    protected void SetCharStable()
    {
        isStable = IsGrounded() && !isDucking && !isAttacking;
    }

    // Player Attack
    protected virtual void PlayerAttack()
    {
        _charManager._charAnim.anim.CrossFade(_charManager._charAnim.FIST_1, 0.1f); // Set Attack Anim
        characterVelocity.x = 0; // Set Velocity 0
        isAttacking = true; // Set Attacking
        isStable = false; // Set Not Stable
    }

    // Serangan Kaki
    protected virtual void FootAttack()
    {
        _charManager._charAnim.anim.CrossFade(_charManager._charAnim.KICK_1, 0.1f); // Set Attack Anim
        characterVelocity.x = 0; // Set Velocity 0
        isAttacking = true; // Set Attacking
        isStable = false; // Set Not Stable
        bodyRigLooking.SetActive(false);
        headRigLooking.SetActive(false);
    }

    protected void setRasioAttack()
    {
        totalAttack++;
        RasioAttack = (attackSuccess / totalAttack);
    }

    // Block Serangan
    protected void Block()
    {
        isBlocking = true;
        characterVelocity.x = 0;
    }

    // Output Serangan
    public void Attack(int point)
    {
        Collider[] coll = Physics.OverlapSphere(attackPoint[point].position, attackRadius, enemyLayer);
        if (coll.Length > 0)
        {
            int damage = Random.Range(minDamage, maxDamage);
            coll[0].GetComponent<Health>().TakeDamage(damage);
            attackSuccess++;
            attackChain++;
        }
        else
        {
            attackChain= 0;
        }
        setRasioAttack();
    }

    // Selesai Block
    public void UnBlock()
    {
        isBlocking = false;
    }

    // Lompat
    public void Jump()
    {
        characterVelocity.y = jumpForce;
        isJump = true;
    }

    // Check Ground
    public bool IsGrounded()
    {
        return Physics.CheckSphere(groundChecker.position, checkRadius, groundLayer);
    }
}
