# Проект Daily Reward Timer for Yandex Game

## Описание
В проекте реализован пример для работы с ежедневными наградами, использующий серверное время непосредственно со страницы игры.
Это предотвращает возможность манипуляций со временем на устройстве и не требует одобрение хоста в Яндекс.Играх.

## Обработка асинхронных операций
- Unity Coroutine

## Классы
### `DailyRewardService`
- Получение серверного времени и синхронизация с локальным временем устройства;
- Проверка соединения с сервером для получения данных;
- Управление таймером награды, начало и окончание таймер;
- Проверка завершения таймера.
    #### Методы и свойства:
- `CurrentRequestResult` - Результат успешности текущего запроса.
- `TimeLeft` - Время оставшееся до окончания таймера.
- `IsActiveTimer` - Проверка активности таймера.


- `ConnectServer()` - Проверка соединения с сервером.
- `GetDateTimeServer()` - Получение серверного время.
- `InitializeTimerRewardReceived()` - Инициализия таймера на основе данных сохранений игры.
- `StartTimerRewardReceived()` - Запуск таймера.
- `CheckTimerRewardEnded()`- Проверка завершения таймера.
- `SetTimerRewardData()` - Сохранение данных таймера.

### `DailyRewardYandexService` : `DailyRewardService`
- Отправка запроса на сервер с использованием метода `HEAD` (запрос заголовков) к URL, представленному `Application.absoluteURL`. 

## Использование:

### 1. Создание экземпляра класса `DailyRewardService`.
```c#
    private DailyRewardService rewardService;
    private void Awake()
    {
#if UNITY_EDITOR
        rewardService = new DailyRewardUnityService();
#else
        rewardService = new DailyRewardYandexService();
#endif
    }
```
### 2. Вызов методов для инициализации таймера.
```c#
   private void Start()
   {
        StartCoroutine(LoadDateTimeReward());
   }
```
```c#
    private IEnumerator LoadDateTimeReward()
    {
        yield return rewardService.ConnectServer(); //Соединения с сервером, получение времени
        if (rewardService.CurrentRequestResult) //Результат текущего запроса
        {
            rewardService.InitializeTimerRewardReceived(); //Инициализия таймера на основе данных сохранений игры.
            CompletedInitializeReward(); //Вызов метода активации UI таймера
        }
    }
```
```c#
    private void CompletedInitializeReward()
    {
        rewardContainer.SetActive(true); //Отображение контейнера таймера
        if (rewardService.IsActiveTimer)
        {
            addCoinsButton.gameObject.SetActive(false); //Скрытие кнопки получения награды
            timerContent.SetActive(true); //Отображение таймера
        }
        else
        {
            addCoinsButton.gameObject.SetActive(true); //Отображение кнопки получения награды
            timerContent.SetActive(false); //Скрытие таймера
        }
    }
```
### 3. Обновление отображение таймера.
```c#
    private void FixedUpdate()
    {
        if (!rewardService.IsActiveTimer) return;

        if (!timerContent.activeSelf)
        {
            timerContent.SetActive(true);
        }

        timerText.text = rewardService.TimeLeft;

        if (!rewardService.CheckTimerRewardEnded()) return; //Проверка, завершения таймера.

        addCoinsButton.gameObject.SetActive(true);
        timerContent.SetActive(false);
        rewardService.IsActiveTimer = false;
    }
```
### 4. Методы вызываемые при успешном просмотре рекламы.
```c#
    private void OnRewardedViewingAds(int id) 
    {
        StartCoroutine(StartRewardedViewingAds(id));
    }
        
    private IEnumerator StartRewardedViewingAds(int id)
    {
        yield return rewardService.ConnectServer(); //Соединения с сервером, получение времени
        switch (id)
        {
            case 1:
                if (rewardService.CurrentRequestResult) //Результат текущего запроса
                {
                    rewardService.StartTimerRewardReceived(); //Запуск таймера
                    YandexGame.savesData.CoinCount += 100; // Получение награды
                    rewardService.SetTimerRewardData(); //Сохранение данных таймера
                }
                else
                {
                    //Вывод сообщения отсутствия интернета
                }
                break;
        }
    }
```