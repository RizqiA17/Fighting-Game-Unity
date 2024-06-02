using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ENEMY_STATE
{
    AGRESIVE,
    PASSIVE,
    DEAD
}

// Kondisi Enemy
public enum ENEMY_FEAR
{
    BRAVE,
    NORMAL,
    FEAR,
}

public class EnemyController : CharController
{
    [Header("Enemy Move AI")]
    [SerializeField] private ENEMY_STATE enemyState; // Enemy State
    [SerializeField] private ENEMY_FEAR enemyFear; // Kondisi Enemy
    [SerializeField] private CharManager _playerManager; // Player Manager
    [SerializeField] private Transform backCheck; // posisi Back Cheker
    [SerializeField] private float minimumDistance; // Minimum jarak dengan player
    [SerializeField] private float distanceToPlayer; // Jarak dengan player
    [SerializeField] private float cdChangeState; // Cooldown ganti state
    private float currentSpeed; // kecepatan x
    [Range(0, 100)]
    private int fearPoint; // poin untuk kondisi
    [SerializeField] private int state; // state untuk gerakan
    private int minFear, minNormal; // Minimal untuk Kondisi Fear dan normal

    [Header("Enemy Attack")]

    [SerializeField] private LayerMask playerLayer; // Layer Player
    [SerializeField] private float attackRange; // Area untuk menyerang
    [SerializeField] private float attackCooldown; // Cooldown Serangan default
    private float attackCd; // Cooldown saat ini

    // Start is called before the first frame update
    void Awake()
    {
        charController = GetComponent<CharacterController>(); // Set Character Controller
        charHeight = charController.height; // Set Tinggi Default Character Controller
        charCenter = charController.center; // Set Lokasi Default Character Controller
        _charManager = GetComponent<CharManager>(); // Set Character Manager
        fearPoint = Random.Range(0, 100); // Set Agresifitas Start

        // Set Agresifitas in Game
        if (Random.Range(0, 1) == 0)
        {
            enemyState = ENEMY_STATE.PASSIVE;
            minFear = 45;
            minNormal = 60;
        }
        else
        {
            enemyState = ENEMY_STATE.AGRESIVE;
            minFear = 35;
            minNormal = 55;
        }
        CharacterFear();
    }

    // Update is called once per frame
    void Update()
    {
        if (_charManager._charControl.isDead) charController.enabled = false; // set Character Collision Off
        else
        {
            CharacterMovement(); // Panggil movement
            CharacterDirection(); // Panggil pergantian arah
            SetCharStable(); // Panggil set stabil player
            attackCd -= Time.deltaTime; // Hitung mundur serangan
        }
        print(BackChecking());
    }

    void CharacterMovement()
    {
        // Set Anim State
        _charManager._charAnim.anim.SetBool(_charManager._charAnim.ISGROUNDED_PARAM, IsGrounded());
        _charManager._charAnim.anim.SetBool(_charManager._charAnim.ISDUCKING_PARAM, isDucking);
        _charManager._charAnim.anim.SetBool(_charManager._charAnim.BLOCK_PARAM, isBlocking);

        // set agar Change character Controller tidak menggangu pergerakan
        if (isStable) _charManager._changeCharacterAttackState.isPlayed = false;

        if (_charManager._charControl.IsGrounded())
        {
            PlayerInDistance(); // Move Player Randomer
            characterVelocity.y = 0; // Set Velocity Y saat di tanah
        }
        else
        {
            if (characterVelocity.y < -9.8f) characterVelocity.y = -gravityForce; // Kecepatan Maksimal saat Terjatuh
            else characterVelocity.y -= Time.deltaTime * gravityForce; // Membuat Effect Smooth saat Lompat atau Terjatuh
        }

        // Serangan Player
        bool AttackPlayer = Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.H) || Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.K);

        if (Random.Range(0, 1) > 0 && AttackPlayer) Block(); // Random Blocking

        distanceToPlayer = Vector2.Distance(enemy.position, transform.position); // Set  Jarak dengan player

        characterVelocity.x = currentSpeed * frontDirection; // Set Pergerakan Karakter
        SetAnimationLocomotor(characterVelocity); // Set Animasi Gerakan
        Move(characterVelocity); // Gerakan Karakter
    }

    // Pengkondisisan pergerakan karakter
    void PlayerInDistance()
    {
        switch (enemyFear)
        {
            case ENEMY_FEAR.BRAVE:
                EnemyBrave();
                break;
            case ENEMY_FEAR.NORMAL:
                EnemyNormal();
                break;
            case ENEMY_FEAR.FEAR:
                EnemyFear();
                break;
        }

        ChangeFearPoint(); // Ubah Fear Point
        Attackable(); // Cek Apakah Bisa menyerang Player
    }

    // Randomer ketika brave
    void EnemyBrave()
    {
        cdChangeState -= Time.deltaTime;
        if (cdChangeState <= 0)
        {
            state = Random.Range(0, 5);
            cdChangeState = Random.Range(0, 5);
        }

        if (state > 4) RunFromPlayer();
        else if (state >= 1) CasePlayer();
        else
        {
            if (Random.Range(0, 1) > 0)
            {
                if (EnemyInDistance()) KeepDistanceToPlayer();
            }
            else SmoothToIdle();
        }
    }

    // Randomer ketika Normal
    void EnemyNormal()
    {
        cdChangeState -= Time.deltaTime;
        if (cdChangeState <= 0)
        {
            state = Random.Range(0, 5);
            cdChangeState = Random.Range(0, 5);
        }

        if (state > 3) RunFromPlayer();
        else if (state > 1) CasePlayer();
        else
        {
            if (Random.Range(0, 1) > 0)
            {
                if (EnemyInDistance()) KeepDistanceToPlayer();
            }
            else SmoothToIdle();
        }

    }

    // Randomer ketika Fear
    void EnemyFear()
    {
        cdChangeState -= Time.deltaTime;
        if (cdChangeState <= 0)
        {
            state = Random.Range(0, 5);
            cdChangeState = Random.Range(0, 5);
        }

        if (state > 2) RunFromPlayer();
        else if (state > 1) CasePlayer();
        else
        {
            if (Random.Range(0, 2) > 0)
            {
                if (EnemyInDistance()) KeepDistanceToPlayer();
            }
            else SmoothToIdle();
        }
    }

    // Mendekati Player
    void CasePlayer()
    {
        if (!Physics.CheckSphere(transform.position, attackRange, playerLayer))
        {
            if (currentSpeed < speed) currentSpeed = Mathf.Lerp(currentSpeed, 1.0f, 0.25f);
        }
        else SmoothToIdle(); // Set ketika dalam range serangan, berhenti
    }

    // Menjauh dari player
    void RunFromPlayer()
    {
        if (!BackChecking())
        {
            if (currentSpeed > -speed)
            {
                currentSpeed = Mathf.Lerp(currentSpeed, -1.0f, 0.25f);
            }
        }
        else currentSpeed = Mathf.Lerp(currentSpeed, 0f, 25f);

        if (Random.Range(0, 1) == 1) _charManager._charAnim.anim.CrossFade(_charManager._charAnim.JUMP, .1f);
    }

    // Kurangi kecepatan secara smooth;
    void SmoothToIdle()
    {
        if (currentSpeed != 0) currentSpeed = Mathf.Lerp(currentSpeed, 0f, 0.25f);
    }

    // Set Enemy Fear sesuai fear point
    void CharacterFear()
    {
        if (fearPoint < minFear) enemyFear = ENEMY_FEAR.FEAR;
        else if (fearPoint < minNormal) enemyFear = ENEMY_FEAR.NORMAL;
        else enemyFear = ENEMY_FEAR.BRAVE;
    }

    // Set fear point
    void ChangeFearPoint()
    {
        int healthIntervalPoint = 0, attackPoint = 0, rasioPoint = 0, chainPoint = 0;

        float enemyAttackSuccess = _playerManager._charControl.attackSuccess; // Attack success player
        float enemyRasioAttack = _playerManager._charControl.rasioAttack; // Rasio attack player
        float enemyChainAttack = _playerManager._charControl.attackChain; // Chain attack player
        float enemyHealth = _playerManager._health.CurrentHealth; // Sisa HP player

        // Point dari jumlah perbedaan nyawa
        //float interval = enemyHealth - _charManager._health.CurrentHealth;
        float interval = enemyHealth - _charManager._health.CurrentHealth > 0 ? (enemyHealth - _charManager._health.CurrentHealth) / _charManager._health.maxHealth : (_charManager._health.CurrentHealth - enemyHealth) / _charManager._health.maxHealth;
        if (interval <= 0.5) healthIntervalPoint = (int)(interval * 40);
        else healthIntervalPoint = 20;

        // Point dari attack success
        if (enemyAttackSuccess <= attackSuccess) attackPoint = 10;
        else attackPoint = -10;

        // Point dari rasio attack
        if (enemyRasioAttack <= rasioAttack) rasioPoint = 5;
        else rasioPoint = -5;

        // Point dari chain attack
        if (enemyChainAttack <= attackChain) chainPoint = 15;
        else chainPoint = 15;

        fearPoint = (healthIntervalPoint + attackPoint + rasioPoint + 50); // Set fear point 100
        CharacterFear();
    }

    // Cek Posisi Player Untuk di serang
    void Attackable()
    {
        if (Physics.CheckSphere(transform.position, attackRange, playerLayer))
        {
            if (attackCd <= 0)
            {
                int attackState = Random.Range(0, 2);
                switch (attackState)
                {
                    case 0:
                        attackCd = attackCooldown;
                        break;
                    case 1:
                        PlayerAttack();
                        break;
                    case 2:
                        FootAttack();
                        break;
                }
            }
        }
    }

    protected override void PlayerAttack()
    {
        base.PlayerAttack();
        attackCd = attackCooldown;
    }

    protected override void FootAttack()
    {
        base.FootAttack();
        attackCd = attackCooldown;
    }

    // Cek jarak dengan Player
    bool EnemyInDistance()
    {
        return Vector2.Distance(enemy.position, transform.position) <= minimumDistance;
    }

    // Cek Bagian belakang kosong
    bool BackChecking()
    {
        return Physics.CheckSphere(backCheck.position, .5f, groundLayer);
    }

    // Menjaga jarak agar tidak terlalu jauh
    void KeepDistanceToPlayer()
    {
        if (_playerManager._charControl.characterVelocity.x > 0) currentSpeed = (Mathf.Lerp(currentSpeed, 1.0f, 0.25f));
    }
}
