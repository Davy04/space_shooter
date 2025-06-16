using TMPro;
using UnityEngine;

public class UICountdown : MonoBehaviour
{
    [Header("Configurações")]
    public float countdownTime = 3f;
    public TMP_Text countdownText;
    [SerializeField] private GameObject countdownPopup;
    [SerializeField] private Timer timer;
    
    private float currentTime;
    private PlayerController playerController;
    private static UICountdown instance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        currentTime = countdownTime;
        playerController = FindObjectOfType<PlayerController>();
       
        if (countdownPopup != null)
            countdownPopup.SetActive(true);
            
        if (countdownText != null)
            countdownText.gameObject.SetActive(true);
        
        if (playerController != null)
        {
            playerController.FreezePlayer(true);
            playerController.enabled = false;
        }
        
        if (timer != null)
            timer.PauseTimer(true);
    }

    void Update()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
         
            if (countdownText != null)
                countdownText.text = Mathf.Ceil(currentTime).ToString("0");
        }
        else
        {
            if (countdownPopup != null)
                countdownPopup.SetActive(false);
            
            if (playerController != null)
            {
                playerController.FreezePlayer(false);
                playerController.enabled = true; 
            }
            
            if (timer != null)
                timer.PauseTimer(false);
            
            enabled = false;
        }
    }

    public static bool IsCountdownActive()
    {
        return instance != null && instance.enabled;
    }
}