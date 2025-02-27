using Elias.Scripts.Components;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace Noah.Scripts.WhiteFlash
{
    public class WhiteFlash : MonoBehaviour
    {
        [Header("Scenes to load")] 
        [SerializeField] private SceneField _levelScene;
        [SerializeField] private GameObject _canvasToHide;
        
        [SerializeField] private float flashSpeed = 10.0f; // Adjust the speed of the flash
        [SerializeField] private float maxIntensity = 300.0f; // Adjust the maximum intensity of the flash
        [SerializeField] private float maxRadius = 70f; // Adjust the maximum radius of the light
        [SerializeField] private float cooldownDuration = 3f; // Cooldown duration before flash

        [SerializeField] private SpriteRenderer _playerBodySprite;
        [SerializeField] private Color _newPlayerBodyColor;
        
        
        private Light2D _light2D;
        private bool _isFlashing = false;

        private bool _isOnCooldown = false;
        private float _cooldownTimer = 0.0f;

        void Start()
        {
            _light2D = PlayerController.Instance.GetComponent<Light2D>(); // Get the Light2D component from this GameObject
        }
        
        private void HideMenu()
        {
            _canvasToHide.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !_isOnCooldown)
            {
                StartCooldown();
            }
        }

        void Update()
        {
            if (_isFlashing)
            {
                _playerBodySprite.color = _newPlayerBodyColor;
                // Increase the intensity over time until it reaches maxIntensity
                if (_light2D.intensity < maxIntensity)
                {
                    _light2D.intensity += flashSpeed * Time.deltaTime;
                    _light2D.color = Color.white; // Set the initial color to white
                }

                // Increase the radius over time until it reaches maxRadius
                if (_light2D.pointLightOuterRadius < maxRadius)
                {
                    _light2D.pointLightOuterRadius += flashSpeed * Time.deltaTime;
                    if (_light2D.pointLightOuterRadius >= maxRadius)
                    {
                        flashSpeed = flashSpeed * 2;
                    }
                }
                
                if (_light2D.intensity >= maxIntensity && _light2D.pointLightOuterRadius >= maxRadius)
                {
                    SceneManager.LoadSceneAsync(_levelScene);
                    Destroy(PlayerController.Instance.gameObject);
                }
            }

            if (_isOnCooldown || _isFlashing)
            {
                HideMenu();
                PlayerController.Instance._anim.SetBool("IsWalking", false);
                PlayerController.Instance.canMove = false;
                PlayerController.Instance.canJump = false;
                PlayerController.Instance.audioSource.mute = true;

            }

            // Decrease cooldown timer
            if (_cooldownTimer > 0 && _isOnCooldown)
            {
                _cooldownTimer -= Time.deltaTime;
                if (_cooldownTimer <= 0)
                {
                    _cooldownTimer = 0;
                    _isOnCooldown = false;
                    TriggerWhiteFlash();
                }
            }
        }

        void StartCooldown()
        {
            _isOnCooldown = true;
            _cooldownTimer = cooldownDuration;
        }

        // Call this method to trigger the white flash
        public void TriggerWhiteFlash()
        {
            _isFlashing = true;
        }
    }
}
