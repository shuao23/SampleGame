﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterKeyInput : MonoBehaviour
{
    [SerializeField]
    private Character character;


    private void Awake()
    {
        if (!TryFindCharacter())
        {
            Debug.LogWarning("Character not assigned nor found. Disabling");
            enabled = false;
            return;
        }
    }

    private void FixedUpdate()
    {
        float unitDeltaX = 0;
        if (Input.GetKey(KeyCode.S))
        {
            unitDeltaX += 1;
        }

        if (Input.GetKey(KeyCode.A))
        {
            unitDeltaX -= 1;
        }

        if (unitDeltaX != 0)
        {
            character.Move(unitDeltaX);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            character.Jump();
        }

        if (Input.GetMouseButton(0))
        {
            character.Attack();
        }

        if (Input.GetMouseButton(1))
        {
            character.RocketAttack();
        }
    }

    private void Reset()
    {
        TryFindCharacter();
    }


    private bool TryFindCharacter()
    {
        if (character == null)
        {
            character = GetComponent<Character>();
        }
        return character != null;
    }
}
