using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DisableButtonAfterTap : MonoBehaviour
{
    GameManager gameManager;
    private void Awake()
    {
        gameManager = GetComponent<GameManager>();
    }

    void Update()
    {
        if (Touchscreen.current.primaryTouch.press.isPressed)
        {
            gameManager.StartTheGame();
            this.enabled = false;
        }
    }
}
