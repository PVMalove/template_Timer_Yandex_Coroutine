using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace ExampleYGDateTime
{
    public class ExampleReword : MonoBehaviour
    {
        [SerializeField] private GameObject rewardContainer;
        [SerializeField] private Button addCoinsButton;
        [SerializeField] private GameObject timerContent;
        [SerializeField] private TextMeshProUGUI timerText;

        [SerializeField] private TextMeshProUGUI coinCountText_Example;
        [SerializeField] private Button resetSaveButton;

        [SerializeField] private GameObject warningPopup;

        private DailyRewardService rewardService;
        private bool isActiveTimer;

        private void Awake()
        {
#if UNITY_EDITOR
            Debug.Log("UNITY_EDITOR_DAILY_SERVICE/REWORD");
            rewardService = new DailyRewardUnityService();
#else
            Debug.Log("YANDEX_DAILY_SERVICE/REWORD");
            rewardService = new DailyRewardYandexService();
#endif
        }

        private void Start()
        {
            coinCountText_Example.text = YandexGame.savesData.CoinCount.ToString();
            StartCoroutine(LoadDateTimeReward());
        }

        private void OnEnable()
        {
            addCoinsButton.onClick.AddListener(AddCoinsWatchingAdsOnClick);
            resetSaveButton.onClick.AddListener(OnClickResetSaveButton);
            YandexGame.RewardVideoEvent += OnRewardedViewingAds;
            YandexGame.ErrorVideoEvent += OnRewardedViewingAdsError;
        }

        private void FixedUpdate()
        {
            if (!rewardService.IsActiveTimer) return;

            if (!timerContent.activeSelf)
            {
                timerContent.SetActive(true);
            }

            timerText.text = rewardService.TimeLeft;

            if (!rewardService.CheckTimerRewardEnded()) return;

            addCoinsButton.gameObject.SetActive(true);
            timerContent.SetActive(false);
            rewardService.IsActiveTimer = false;
            Debug.Log("[ExampleReward] End timer");
        }

        private void OnDisable()
        {
            addCoinsButton.onClick.RemoveListener(AddCoinsWatchingAdsOnClick);
            resetSaveButton.onClick.RemoveListener(OnClickResetSaveButton);
            YandexGame.RewardVideoEvent -= OnRewardedViewingAds;
            YandexGame.ErrorVideoEvent -= OnRewardedViewingAdsError;
        }

        private IEnumerator LoadDateTimeReward()
        {
            yield return rewardService.CheckConnection();
            if (rewardService.CurrentRequestResult)
            {
                warningPopup.SetActive(false);
                rewardService.InitializeTimerRewardReceived();
                CompletedInitializeReward();
            }
            else
            {
                warningPopup.SetActive(true);
            }
        }

        private void CompletedInitializeReward()
        {
            rewardContainer.SetActive(true);
            if (rewardService.IsActiveTimer)
            {
                addCoinsButton.gameObject.SetActive(false);
                timerContent.SetActive(true);
            }
            else
            {
                addCoinsButton.gameObject.SetActive(true);
                timerContent.SetActive(false);
            }
        }

        private void OnRewardedViewingAdsError()
        {
            if (!rewardService.CurrentRequestResult)
            {
                warningPopup.SetActive(true);
            }
        }

        private void AddCoinsWatchingAdsOnClick()
        {
            addCoinsButton.gameObject.SetActive(false);
            YandexGame.RewVideoShow(1);
        }

        private void OnRewardedViewingAds(int id) => 
            StartCoroutine(StartRewardedViewingAds(id));

        private IEnumerator StartRewardedViewingAds(int id)
        {
            yield return rewardService.CheckConnection();
            switch (id)
            {
                case 1:
                    if (rewardService.CurrentRequestResult)
                    {
                        rewardService.StartTimerRewardReceived();
                        YandexGame.savesData.CoinCount += 100;
                        coinCountText_Example.text = YandexGame.savesData.CoinCount.ToString();
                        rewardService.SetTimerRewardData();
                    }
                    else
                    {
                        warningPopup.SetActive(true);
                    }
                    break;
            }
        }

        private void OnClickResetSaveButton()
        {
            YandexGame.ResetSaveProgress();
            YandexGame.SaveProgress();
            coinCountText_Example.text = YandexGame.savesData.CoinCount.ToString();
            addCoinsButton.gameObject.SetActive(true);
            rewardService.IsActiveTimer = false;
            CompletedInitializeReward();
        }
    }
}