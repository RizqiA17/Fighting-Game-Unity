using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ENEMY_STATE
{
    AGRESIVE,
    PASSIVE,
    DEAD
}

public enum ENEMY_FEAR
{
    BRAVE,
    NORMAL,
    FEAR,
}

public class EnemyController : CharController
{
    [Header("Enemy Move AI")]
    [SerializeField] private ENEMY_STATE enemyState;
    [SerializeField] private ENEMY_FEAR enemyFear;
    [SerializeField] private CharManager _playerManager;
    [SerializeField] private float minimumDistance;
    [SerializeField] private float distanceToPlayer;
    [SerializeField] private float cdChangeState;
    private float currentSpeed;
    [Range(0, 100)]
    private int fearPoint;
    private int state;
    private int minFear, minNormal;

    [Header("Enemy Attack")]

    [SerializeField] private LayerMask playerLayer; // Layer Player
    [SerializeField] private float attackRange; // Area untuk menyerang
    [SerializeField] private float attackCooldown;
    private float attackCd;

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
        if (_charManager._charControl.isDead)
        {
            charController.enabled = false;
        }
        else
        {
            CharacterMovement();
            CharacterDirection();
            SetCharStable();
            attackCd -= Time.deltaTime;
        }
    }

    void CharacterMovement()
    {
        // Set Anim State
        _charManager._charAnim.anim.SetBool(_charManager._charAnim.ISGROUNDED_PARAM, IsGrounded());
        _charManager._charAnim.anim.SetBool(_charManager._charAnim.ISDUCKING_PARAM, isDucking);
        _charManager._charAnim.anim.SetBool(_charManager._charAnim.BLOCK_PARAM, isBlocking);

        if(isStable) _charManager._changeCharacterAttackState.isPlayed = false;

        if (_charManager._charControl.IsGrounded())
        {
            PlayerInDistance();

            //else KeepDistanceToPlayer();

            // Set Velocity Y saat di tanah
            characterVelocity.y = 0;
        }
        else
        {
            // Kecepatan Maksimal saat Terjatuh
            if (characterVelocity.y < -9.8f) characterVelocity.y = -gravityForce;
            // Membuat Effect Smooth saat Lompat atau Terjatuh
            else characterVelocity.y -= Time.deltaTime * gravityForce;
        }

        bool AttackPlayer = Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.H) || Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.K);

        if (Random.Range(0, 1) > 0 && AttackPlayer) Block();

        distanceToPlayer = Vector2.Distance(enemy.position, transform.position);

        characterVelocity.x = currentSpeed * frontDirection;
        SetAnimationLocomotor(characterVelocity);
        Move(characterVelocity);
    }

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

        //if (currentSpeed > 0) currentSpeed -= Time.deltaTime;
        //else if (currentSpeed <= 0) currentSpeed = 0;

        ChangeFearPoint();
        Attackable();
    }

    void EnemyBrave()
    {
        cdChangeState -= Time.deltaTime;
        if (cdChangeState <= 0)
        {
            state = Random.Range(0, 5);
            //print(state);
            cdChangeState = Random.Range(0, 5);
        }
        if (state >= 1)
        {
            CasePlayer();
        }
        else if (state >= 4)
        {
            RunFromPlayer();
        }
        else
        {
            if (Random.Range(0, 1) > 0)
            {
                if (EnemyInDistance()) KeepDistanceToPlayer();
            }
            else SmoothToIdle();
        }
    }

    void EnemyNormal()
    {
        cdChangeState -= Time.deltaTime;
        if (cdChangeState <= 0)
        {
            state = Random.Range(0, 5);
            //print(state);
            cdChangeState = Random.Range(0, 5);
        }
        if (state > 2)
        {
            CasePlayer();
        }
        if (state > 0)
        {
            RunFromPlayer();
        }
        else
        {
            if (Random.Range(0, 1) > 0)
            {
                if (EnemyInDistance()) KeepDistanceToPlayer();
            }
            else SmoothToIdle();
        }

    }

    void EnemyFear()
    {
        cdChangeState -= Time.deltaTime;
        if (cdChangeState <= 0)
        {
            state = Random.Range(0, 5);
            //print(state);
            cdChangeState = Random.Range(0, 5);
        }
        if (state > 3)
        {
            CasePlayer();
        }
        if (state >= 2)
        {
            RunFromPlayer();
        }
        else
        {
            if (Random.Range(0, 2) > 0)
            {
                if (EnemyInDistance()) KeepDistanceToPlayer();
            }
            else SmoothToIdle();
        }
    }

    void CasePlayer()
    {
        if (!Physics.CheckSphere(transform.position, attackRange, playerLayer))
        {
            if (currentSpeed < speed) currentSpeed = Mathf.Lerp(currentSpeed, 1.0f, 0.25f);
        }
        else SmoothToIdle();
    }

    void RunFromPlayer()
    {
        if (!Physics.CheckSphere(transform.position, attackRange, playerLayer))
        {
            if (currentSpeed > speed) currentSpeed = -(Mathf.Lerp(currentSpeed, 1.0f, 0.25f));
        }
        else SmoothToIdle();
    }

    void SmoothToIdle()
    {
        if (currentSpeed > 0) currentSpeed -= Time.deltaTime;
        else currentSpeed = 0;
    }

    void CharacterFear()
    {
        if (fearPoint < minFear) enemyFear = ENEMY_FEAR.FEAR;
        else if (fearPoint < minNormal) enemyFear = ENEMY_FEAR.NORMAL;
        else enemyFear = ENEMY_FEAR.BRAVE;
    }

    void ChangeFearPoint()
    {
        int healthPoint = 0, attackPoint = 0, rasioPoint = 0, chainPoint = 0;

        float enemyAttackSuccess = _playerManager._charControl.attackSuccess;
        float enemyRasioAttack = _playerManager._charControl.rasioAttack;
        float enemyChainAttack = _playerManager._charControl.attackChain;

        if (_playerManager._health.CurrentHealth <= _charManager._health.CurrentHealth) healthPoint = 3;
        else healthPoint = -3;

        if (enemyAttackSuccess <= attackSuccess) attackPoint = 3;
        else attackPoint = -3;

        if (enemyRasioAttack <= rasioAttack) rasioPoint = 3;
        else rasioPoint = -3;

        if (chainPoint <= attackChain) chainPoint = 4;
        else chainPoint = 4;

        fearPoint = (healthPoint + attackPoint + rasioPoint + 10) * 5;

        CharacterFear();
    }

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

    bool EnemyInDistance()
    {
        return Vector2.Distance(enemy.position, transform.position) <= minimumDistance;
    }

    // Menjaga jarak agar tidak terlalu jauh
    void KeepDistanceToPlayer()
    {
        if (_playerManager._charControl.characterVelocity.x > 0) currentSpeed = -(Mathf.Lerp(currentSpeed, 1.0f, 0.25f));
        else if (_playerManager._charControl.characterVelocity.x < 0) currentSpeed = Mathf.Lerp(currentSpeed, 1.0f, 0.25f);

        //if (currentSpeed < speed) currentSpeed = Mathf.Lerp(currentSpeed, 1.0f, 0.25f);
    }
}
