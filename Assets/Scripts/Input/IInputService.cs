using Core.Services;
using UnityEngine;

namespace Game.Input
{
    public interface IInputService : IService
    {
        Vector2 Move { get; }
        Vector2 Look { get; }
        bool IsBlockHeld { get; }
        bool IsLookFromMouse { get; }

        event System.Action LightAttackPressed;
        event System.Action HeavyAttackPressed;
        event System.Action DodgeOrStepPressed;
        event System.Action LockOnPressed;
        event System.Action InteractPressed;
        event System.Action PausePressed;

        void Enable();
        void Disable();
    }
}