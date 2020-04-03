using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInputManager))]
public class PlayerJoinManager : MonoBehaviour
{
    private PlayerInputManager playerInputManager;

    private void Awake() {
        playerInputManager = GetComponent<PlayerInputManager>();
    }
}
