using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    protected CharManager _charManager;
    private float currentHealth;
    public float maxHealth {get; protected set; }
    [SerializeField] protected float maxHealthDefault;
    [SerializeField] protected TextMeshProUGUI tmp;
    [SerializeField] protected Slider slider;
    [SerializeField] protected GameManager _gameManger;
    private bool isDead;
    public float CurrentHealth
    {
        get => currentHealth;
        set
        {
            currentHealth = value;
            OnChangeHealth();
            if (currentHealth <= 0) Dead();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        maxHealth = maxHealthDefault;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeDamage(int damage)
    {
        if (!_charManager._charControl.isBlocking)
        {
            if (CurrentHealth > 0)
            {
                CurrentHealth -= damage;
                _charManager._charControl.attackChain = 0;
                _charManager._charAnim.anim.CrossFade(_charManager._charAnim.GET_DAMAGE, .1f);
                //_charManager._charAnim.anim.SetTrigger(_charManager._charAnim.SET_EMPTY_TRIGGER);
                //_charManager._changeCharacterAttackState.isPlayed = false;
                //_charManager._charControl.isAttacking = false;
                //print("Take Damage");
                //Blocking();
            }
        }
    }

    public void OnChangeHealth()
    {
        slider.value = CurrentHealth / maxHealth;
        //_charManager._changeCharacterAttackState.isPlayed = false;
        //_charManager._charControl.isAttacking = false;
        //print("Take Damage");
        //Blocking();
    }

    public virtual void Dead()
    {
        _gameManger.EndGame();
    }

    public virtual void Blocking()
    {
        _charManager._changeCharacterAttackState.velocity = 0;
        _charManager._changeCharacterAttackState.isPlayed = false;
        _charManager._charControl.isAttacking = false;
        //_changeCharacterAttackState.EndEffect();
        //_changeCharacterAttackState.AttackEnd();
    }

    void OnDestroy()
    {

    }
}
