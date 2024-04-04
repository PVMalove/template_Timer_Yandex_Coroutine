using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace ExampleYGDateTime
{
    public class DailyRewardUnityService : DailyRewardService
    {
        private const string SERVER_URI = "https://worldtimeapi.org/api/timezone/Europe/Moscow";
        private UnityWebRequest webRequest;

        public override IEnumerator CheckConnection()
        {
            webRequest = UnityWebRequest.Get(SERVER_URI);
            yield return webRequest.SendWebRequest();
            Debug.Log($"[DailyRewardUnityService] =>  Server webRequest result -> {webRequest.result}");
            switch (webRequest.result)
            {
                case UnityWebRequest.Result.Success:
                    yield return CurrentRequestResult = true;
                    break;
                default:
                    yield return CurrentRequestResult = false;
                    break;
            }
        }
        
        protected override int GetServerTimeNow()
        {
            int serverTime;
            switch (webRequest.result)
            {
                case UnityWebRequest.Result.Success:
                    string json = webRequest.downloadHandler.text;
                    ServerTimeResponse response = JsonUtility.FromJson<ServerTimeResponse>(json);
                    serverTime = response.unixtime;
                    Debug.Log($"[DailyRewardService] => Server time in second -> {serverTime}");
                    break;
                default:
                    throw new Exception($"[DailyRewardService] => Unknown Error! -> {webRequest.error}");
            }
            return serverTime;
        }
    }
}