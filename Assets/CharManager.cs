using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharManager : MonoBehaviour
{
    [HideInInspector] public CharAnimation _charAnim;
    [HideInInspector] public CharController _charControl;
    [HideInInspector] public Health _health;
    [HideInInspector] public ChangeCharacterAttackState _changeCharacterAttackState;
    // Start is called before the first frame update
    void Start()
    {
        _health = GetComponent<Health>();
        _charAnim = GetComponent<CharAnimation>();
        _charControl = GetComponent<CharController>();
        _changeCharacterAttackState = GetComponentInChildren<ChangeCharacterAttackState>();
    }
}
