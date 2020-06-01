using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace Invector.vCharacterController
{
    public class vHUDController : MonoBehaviour
    {
        #region General Variables

        #region Health/Stamina Variables
        [Header("Health/Stamina")]
        public Slider healthSlider;
        public Slider staminaSlider;
        
        public static int count = 0;
        public TextMeshProUGUI score;
        public TextMeshProUGUI finalScore;
        [Header("DamageHUD")]
        public Image damageImage;
        public float flashSpeed = 5f;
        public Color flashColour = new Color(1f, 0f, 0f, 0.1f);
        [HideInInspector] public bool damaged;
        #endregion

        #region Display Controls Variables
        [Header("Controls Display")]
        [HideInInspector]
        public bool controllerInput;
        public Image displayControls;
        public Sprite joystickControls;
        public Sprite keyboardControls;
        #endregion

        #region Debug Info Variables
        [Header("Debug Window")]
        public GameObject debugPanel;
        [HideInInspector]
        public Text debugText;
        #endregion

        #region Change Input Text Variables
        [Header("Text with FadeIn/Out")]
        public TextMeshProUGUI fadeText;
        public TextMeshProUGUI dropText;
        private float textDuration, fadeDuration, durationTimer, timer;
        private Color startColor, endColor;
        private bool fade;
        public Animator fadeScreen;
        public GameObject menuCanvas;
        public GameObject GameOverCanvas;
        public GameObject equipmentMenu;
        public AudioSource bgmAudioSource;
        public AudioSource deathSound;
        
        


        #endregion

        #endregion


        private static vHUDController _instance;
        public static vHUDController instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<vHUDController>();
                    //Tell unity not to destroy this object when loading a new scene
                    //DontDestroyOnLoad(_instance.gameObject);
                }
                return _instance;
            }
        }

        void Start()
        {
            
            count = 0;
            InitFadeText();
            fadeScreen.SetTrigger("FadeIn");
            menuCanvas.SetActive(true);
            equipmentMenu.SetActive(true);
            GameOverCanvas.SetActive(true);
            

            if (debugPanel != null)
                debugText = debugPanel.GetComponentInChildren<Text>();
        }

        public void Init(vThirdPersonController cc)
        {
            cc.onDead.AddListener(OnDead);
            cc.onReceiveDamage.AddListener(EnableDamageSprite);
            damageImage.color = new Color(0f, 0f, 0f, 0f);
            if (cc.maxHealth != healthSlider.maxValue)
            {
                healthSlider.maxValue = cc.maxHealth;
                healthSlider.onValueChanged.Invoke(healthSlider.value);
            }
            healthSlider.value = cc.currentHealth;
            if (cc.maxStamina != staminaSlider.maxValue)
            {
                staminaSlider.maxValue = cc.maxStamina;
                staminaSlider.onValueChanged.Invoke(staminaSlider.value);
            }
            staminaSlider.value = cc.currentStamina;
        }



        public void PlusScore()
        {
            
            count = count + 1;
            score.text = count.ToString();
        }

        private void OnDead(GameObject arg0)
        {
            
            ShowText("You are Dead!!!");
            fadeScreen.SetTrigger("FadeOut");
            menuCanvas.SetActive(false);
            equipmentMenu.SetActive(false);
            finalScore.text = "Your Score:    " + count.ToString();

            if (count > highscore.scores[0])
            {
                highscore.scores[0] = count;
                Array.Sort(highscore.scores);
                highscore.SaveScore();
                Debug.Log("Save!   " + highscore.scores[0] + " / " + highscore.scores[1] + " / " + highscore.scores[2]);
            }
            
            bgmAudioSource.Stop();
            deathSound.PlayDelayed(0.5f);


        }

        public virtual void UpdateHUD(vThirdPersonController cc)
        {
            UpdateDebugWindow(cc);
            UpdateSliders(cc);
            ChangeInputDisplay();
            ShowDamageSprite();
            FadeEffect();
            
            if (!cc.isDead && !Pause_Menu.GameIsPaused && !Pause_Menu.InControlMenu)
            {
                InvokeRepeating("PlusScore", 0f, 20f);
            }

            else
            {
                CancelInvoke("PlusScore");
                score.text = count.ToString();
            }
        }

        public void ShowText(string message, float textTime = 2f, float fadeTime = 0.5f)
        {
            if (fadeText != null && !fade)
            {
                fadeText.text = message;
                textDuration = textTime;
                fadeDuration = fadeTime;
                durationTimer = 0f;
                timer = 0f;
                fade = true;
            }
            else if (fadeText != null)
            {
                fadeText.text += "\n" + message;
                textDuration = textTime;
                fadeDuration = fadeTime;
                durationTimer = 0f;
                timer = 0f;
            }
        }

        public void ShowText(string message)
        {
            if (fadeText != null && !fade)
            {
                fadeText.text = message;
                textDuration = 2f;
                fadeDuration = 0.5f;
                durationTimer = 0f;
                timer = 0f;
                fade = true;
            }
            else if (fadeText != null)
            {
                fadeText.text += "\n" + message;
                textDuration = 2f;
                fadeDuration = 0.5f;
                durationTimer = 0f;
                timer = 0f;
            }
        }

        void UpdateSliders(vThirdPersonController cc)
        {
            if (cc.maxHealth != healthSlider.maxValue)
            {
                healthSlider.maxValue = Mathf.Lerp(healthSlider.maxValue, cc.maxHealth, 2f * Time.fixedDeltaTime);
                healthSlider.onValueChanged.Invoke(healthSlider.value);
            }
            healthSlider.value = Mathf.Lerp(healthSlider.value, cc.currentHealth, 2f * Time.fixedDeltaTime);
            if (cc.maxStamina != staminaSlider.maxValue)
            {
                staminaSlider.maxValue = Mathf.Lerp(staminaSlider.maxValue, cc.maxStamina, 2f * Time.fixedDeltaTime);
                staminaSlider.onValueChanged.Invoke(staminaSlider.value);
            }
            staminaSlider.value = cc.currentStamina;
        }

        public void ShowDamageSprite()
        {
            if (damaged)
            {
                damaged = false;
                if (damageImage != null)
                    damageImage.color = flashColour;
            }
            else if (damageImage != null)
                damageImage.color = Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
        }

        public void EnableDamageSprite(vDamage damage)
        {
            if (damageImage != null)
                damageImage.enabled = true;
            damaged = true;
        }

        void UpdateDebugWindow(vThirdPersonController cc)
        {
            if (cc.debugWindow)
            {
                if (debugPanel != null && !debugPanel.activeSelf)
                    debugPanel.SetActive(true);
                if (debugText)
                    debugText.text = cc.DebugInfo();
            }
            else
            {
                if (debugPanel != null && debugPanel.activeSelf)
                    debugPanel.SetActive(false);
            }
        }

        void ChangeInputDisplay()
        {
#if MOBILE_INPUT
		displayControls.enabled = false;
#else
            if (controllerInput)
                displayControls.sprite = joystickControls;
            else
                displayControls.sprite = keyboardControls;
#endif
        }

        void InitFadeText()
        {
            if (fadeText != null)
            {
                startColor = fadeText.color;
                endColor.a = 0f;
                fadeText.color = endColor;
            }
            else
                Debug.Log("Please assign a Text object on the field Fade Text");
        }

        void FadeEffect()
        {
            if (fadeText != null)
            {
                if (fade)
                {
                    fadeText.color = Color.Lerp(endColor, startColor, timer);

                    if (timer < 1)
                        timer += Time.deltaTime / fadeDuration;

                    if (fadeText.color.a >= 1)
                    {
                        fade = false;
                        timer = 0f;
                    }
                }
                else
                {
                    if (fadeText.color.a >= 1)
                        durationTimer += Time.deltaTime;

                    if (durationTimer >= textDuration)
                    {
                        fadeText.color = Color.Lerp(startColor, endColor, timer);
                        if (timer < 1)
                            timer += Time.deltaTime / fadeDuration;
                    }
                }
            }
        }

        

    }
}