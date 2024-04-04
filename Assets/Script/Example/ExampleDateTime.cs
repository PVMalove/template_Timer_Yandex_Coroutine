using System.Collections;
using TMPro;
using UnityEngine;

namespace ExampleYGDateTime
{
    public class ExampleDateTime : MonoBehaviour
    {
        [SerializeField] private GameObject dateTimeContent;
        [SerializeField] private TextMeshProUGUI dateTimeYandexText;

        private DailyRewardService rewardService;
        private bool isActive;
        
        private void Awake()
        {
#if UNITY_EDITOR
            Debug.Log("UNITY_EDITOR_DAILY_SERVICE/DATETIME");
            rewardService = new DailyRewardUnityService();
#else
            Debug.Log("YANDEX_DAILY_SERVICE/DATETIME");
            rewardService = new DailyRewardYandexService();
#endif
        }
        
        private void Start()
        {
            StartCoroutine(LoadDateTime());
        }

        private IEnumerator LoadDateTime()
        {
            yield return rewardService.CheckConnection();
            if (rewardService.CurrentRequestResult)
            {
                rewardService.GetDateTimeServer();
                isActive = true;
                dateTimeContent.SetActive(true);
            }
            else
            {
                dateTimeYandexText.text = "No internet connection...";
                isActive = false;
                dateTimeContent.SetActive(true);
            }
        }

        private void FixedUpdate()
        {
            if (isActive)
                dateTimeYandexText.text = $"UTC: {rewardService.DateTimeNow}";
        }
    }
}