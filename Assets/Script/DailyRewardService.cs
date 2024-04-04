using System;
using System.Collections;
using UnityEngine;
using YG;

namespace ExampleYGDateTime
{
    public abstract class DailyRewardService
    {
        //DataTime
        private int deltaDateTime;

        //Reward
        private const int TIME_1H = 3600;
        private int timerTime;
        private double localTime;
        private int deltaTime;
        private bool isActiveTimer;
        private bool isActiveTimeData;

        public bool CurrentRequestResult { get; protected set; }
        
        public string DateTimeNow => CalculateDateTime();
        
        public string TimeLeft => CalculateTimeLeft();
        
        public bool IsActiveTimer
        {
            get => isActiveTimer;
            set => isActiveTimer = value;
        }

        public void GetDateTimeServer()
        {
            int serverTimeNow = GetServerTimeNow();
            double localDateTime = Time.realtimeSinceStartupAsDouble;
            deltaDateTime = (int)(serverTimeNow - localDateTime);
        }

        public void InitializeTimerRewardReceived()
        {
            isActiveTimeData = YandexGame.savesData.isActiveTimer;
            if (isActiveTimeData)
            {
                int serverTimeNow = GetServerTimeNow();
                timerTime = YandexGame.savesData.lastReceiveCoinsTime;
                localTime = Time.realtimeSinceStartupAsDouble;
                deltaTime = (int)(serverTimeNow - localTime);
                isActiveTimer = true;
            }
            else
            {
                isActiveTimer = false;
            }
            Debug.Log($"[DailyRewardService] =>  Initialize timer -> {IsActiveTimer}");
        }

        public void StartTimerRewardReceived()
        {
            int serverTimeNow = GetServerTimeNow();
            timerTime = TIME_1H + serverTimeNow;
            localTime = Time.realtimeSinceStartupAsDouble;
            deltaTime = (int)(serverTimeNow - localTime);
            isActiveTimeData = true;
            isActiveTimer = true;

            Debug.Log($"[DailyRewardService] =>  Start timer reward received -> {IsActiveTimer}");
        }


        public bool CheckTimerRewardEnded() => 
            Time.realtimeSinceStartupAsDouble + deltaTime > timerTime;

        public abstract IEnumerator CheckConnection();
        protected abstract int GetServerTimeNow();

        public void SetTimerRewardData()
        {
            YandexGame.savesData.lastReceiveCoinsTime = timerTime;
            YandexGame.savesData.isActiveTimer = isActiveTimer;
            YandexGame.SaveProgress();
            Debug.Log($"[DailyRewardService] => Save timer reward data / isActiveTimer -> {YandexGame.savesData.isActiveTimer}");
        }
        
        private string CalculateDateTime()
        {
            double calculateTime = deltaDateTime + Time.realtimeSinceStartupAsDouble;
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds((int)calculateTime);
            return dateTimeOffset.ToString("dd/MM/yyyy HH:mm:ss");
        }

        private string CalculateTimeLeft()
        {
            double calculateTimeLeft = timerTime - Time.realtimeSinceStartupAsDouble - deltaTime;
            int minutes = Mathf.FloorToInt((float)calculateTimeLeft / 60);
            int seconds = Mathf.FloorToInt((float)calculateTimeLeft % 60);
            return $"{minutes:00}:{seconds:00}";
        }
    }
}