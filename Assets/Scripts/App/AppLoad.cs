using Core.Services;
using Game.Input;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace App
{
    public sealed class AppLoad : MonoBehaviour
    {
        [SerializeField] private string _firstSceneName = "Gameplay_TestArena";

        private IInputService _inputService;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            ServiceLocator.Clear();

            _inputService = new InputService();
            ServiceLocator.Register(_inputService);

            _inputService.Enable();
        }

        private void Start()
        {
            SceneManager.LoadScene(_firstSceneName);
        }

        private void OnDestroy()
        {
            _inputService?.Disable();
        }
    }
}