using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Health
{
    // Start is called before the first frame update
    void Start()
    {
        maxHealth = maxHealthDefault;
        _charManager = GetComponent<CharManager>();
        CurrentHealth = maxHealth;
        slider.value = CurrentHealth / maxHealth;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Dead()
    {
        base.Dead();
        _charManager._charAnim.anim.CrossFade(_charManager._charAnim.DEAD_PARAM, .1f);
    }

}
