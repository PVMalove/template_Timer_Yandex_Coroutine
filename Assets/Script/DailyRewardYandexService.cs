using System;
using System.Collections;
using System.Globalization;
using UnityEngine;
using UnityEngine.Networking;

namespace ExampleYGDateTime
{
    public class DailyRewardYandexService : DailyRewardService
    {
        private UnityWebRequest webRequest;

        public override IEnumerator ConnectServer()
        {
            webRequest = UnityWebRequest.Head(Application.absoluteURL);
            webRequest.SetRequestHeader("cache-control", "no-cache");
            yield return webRequest.SendWebRequest();
            Debug.Log($"[DailyRewardYandexService] =>  Server webRequest result -> {webRequest.result}");
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
                    string dateString = webRequest.GetResponseHeader("date");
                    Debug.Log($"[DailyRewardYandexService] => Yandex server time -> {dateString}");
                    DateTimeOffset date = DateTimeOffset.ParseExact(dateString, "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.AssumeUniversal);
                    Debug.Log($"[DailyRewardYandexService] => Server time in date -> {date}");
                    serverTime = (int)date.ToUnixTimeSeconds();
                    Debug.Log($"[DailyRewardYandexService] => Server time in second -> {serverTime}");
                    break;
                default:
                    throw new Exception($"[DailyRewardYandexService] => Unknown Error! -> {webRequest.error}");
            }

            return serverTime;
        }
    }
}