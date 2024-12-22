using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ElementSpawnerArcade : MonoBehaviour
{
    // Поля из вашего кода
    public GameObject[] elements;
    public GameObject bombPrefab;
    public GameObject scorePopupPrefab;
    public GameObject spawnTarget;
    public float objectSpawnDelay = 0f;
    public TextMeshProUGUI[] _pointsDisplayText;
    public TextMeshProUGUI[] _countdownDisplayText;
    public GameObject winMenu;
    public GameObject loseMenu;
    public float horizontalMargin = 1.0f;
    public float topMargin = 1.0f;
    public float bottomMargin = 1.0f;
    public GameObject _gameSceneObject;
    public TextMeshProUGUI[] _textScoreManagerMenu;
    public float easySpeed = 2f;

    public GameObject[] lifeIcons;
    private int currentLives = 3;

    private int playerScore = 0;
    private float remainingTime = 120f;
    private bool isGameOver = false;
    private Vector2 screenBounds;
    private int _currentLevel;

    // Для бонусов
    private bool isInvincibleToBombs = false;
    private List<GameObject> spawnedElements = new List<GameObject>();  // Список для отслеживания всех элементов на экране

    public Image crosshairImage;
    
    public TextMeshProUGUI bombImmunityCountText;
    public TextMeshProUGUI destroyAllTargetsCountText;

    private const string CurrencyKey = "PlayerCurrency";
    private const string BombImmunityCountKey = "BombImmunityCount";
    private const string DestroyAllTargetsCountKey = "DestroyAllTargetsCount";

    public Button destroyAllTargetsButton;
    public Button bombImmunityButton;
    private int bombImmunityCount;
    private int destroyAllTargetsCount;
    private int playerCurrency;

    
    void Start()
    {
        playerCurrency = PlayerPrefs.GetInt(CurrencyKey, 0);
        bombImmunityCount = PlayerPrefs.GetInt(BombImmunityCountKey, 0);
        destroyAllTargetsCount = PlayerPrefs.GetInt(DestroyAllTargetsCountKey, 0);

        UpdateBonusUI();
        
        if (crosshairImage != null)
        {
            crosshairImage.gameObject.SetActive(false);
        }
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        StartCoroutine(SpawnElements());
        RefreshScoreDisplay();
        UpdateLifeIcons();

        _currentLevel = PlayerPrefs.GetInt(OmniMetrics.ACTIVE_STAGEID, 0);
    }
    void UpdateBonusUI()
    {
        // Обновляем текст количества бонусов
        destroyAllTargetsCountText.text = destroyAllTargetsCount.ToString();
        bombImmunityCountText.text = bombImmunityCount.ToString();

        // Делаем кнопки недоступными, если количество бонусов равно 0
        destroyAllTargetsButton.interactable = destroyAllTargetsCount > 0;
        bombImmunityButton.interactable = bombImmunityCount > 0;
    }

    public void UseBombImmunity()
    {
        if (bombImmunityCount > 0)
        {
            bombImmunityCount--;
            PlayerPrefs.SetInt(BombImmunityCountKey, bombImmunityCount);
            PlayerPrefs.Save();

            ActivateBombImmunity();
            UpdateBonusUI();
        }
    }

    public void UseDestroyAllTargets()
    {
        if (destroyAllTargetsCount > 0)
        {
            destroyAllTargetsCount--;
            PlayerPrefs.SetInt(DestroyAllTargetsCountKey, destroyAllTargetsCount);
            PlayerPrefs.Save();

            DestroyAllTargets();
            UpdateBonusUI();
        }
    }
    void Update()
    {
        if (!isGameOver)
        {
            UpdateTimer();
        }
    }
    
    
    void HideCrosshair()
    {
        if (crosshairImage != null)
        {
            crosshairImage.gameObject.SetActive(false);
        }
    }

    IEnumerator SpawnElements()
    {
        while (!isGameOver)
        {
            SpawnRandomElement();
            yield return new WaitForSeconds(objectSpawnDelay);
        }
    }

    void SpawnRandomElement()
    {
        GameObject elementToSpawn = Random.value > 0.2f ? elements[Random.Range(0, elements.Length)] : bombPrefab;

        Vector2 spawnPosition = new Vector2(
            Random.Range(-screenBounds.x + horizontalMargin, screenBounds.x - horizontalMargin),
            screenBounds.y + topMargin
        );

        GameObject newElement = Instantiate(elementToSpawn, spawnPosition, Quaternion.identity, spawnTarget.transform);
        spawnedElements.Add(newElement);  // Добавляем элемент в список

        float speed = _currentLevel >= 4 ? _currentLevel : easySpeed;
        Debug.Log(speed);
        StartCoroutine(MoveElement(newElement, speed));

        Destroy(newElement, 7.0f);
    }

    IEnumerator MoveElement(GameObject element, float speed)
    {
        float elapsedTime = 0f;
        Vector2 startPosition = element.transform.position;
        float amplitude = Random.Range(0.5f, 2f);
        float frequency = Random.Range(1f, 3f);

        while (element != null && element.transform.position.y > -screenBounds.y - bottomMargin)
        {
            elapsedTime += Time.deltaTime;

            float xOffset = Mathf.Sin(elapsedTime * frequency) * amplitude;
            Vector2 newPosition = new Vector2(startPosition.x + xOffset, element.transform.position.y - speed * Time.deltaTime);

            element.transform.position = newPosition;

            yield return null;
        }
    }

    public void OnElementClick(bool isBomb, bool isLive, int numberScore, Vector3 position)
    {
        if (isGameOver) return;

        // Устанавливаем позицию и активируем прицел
        if (crosshairImage != null)
        {
            crosshairImage.gameObject.SetActive(true);
            crosshairImage.transform.position = position;
            Invoke(nameof(HideCrosshair), 1f); // Скрыть прицел через 2 секунды
        }

        if (isLive)
        {
            ChangeLife(1);
            GameObject popup = Instantiate(scorePopupPrefab, position, Quaternion.identity);
            TextMeshProUGUI popupText = popup.GetComponentInChildren<TextMeshProUGUI>();
            popupText.text = "+1 life";

            Destroy(popup, 0.7f);
            return;
        }

        int playerPoints = isBomb ? -100 : numberScore;
        if (isBomb && !isInvincibleToBombs)  // Если неуязвимость отключена, теряем жизнь
        {
            playerScore += playerPoints;
            ShowScorePopup(playerPoints, position);
            ChangeLife(-1);
            RefreshScoreDisplay();
            Lifes();
            return;
        }

        if (!isInvincibleToBombs || isInvincibleToBombs && !isBomb)
        {
            playerScore += playerPoints;
            ShowScorePopup(playerPoints, position);
            RefreshScoreDisplay();
            Lifes();
        }
    }

    private void Lifes()
    {
        if (currentLives <= 0)
        {
            TerminateGameplay(false);
            remainingTime = 0;
        }
    }

    void ChangeLife(int number)
    {
        if (currentLives < 3 || number < 0)
        {
            currentLives += number;
            UpdateLifeIcons();
            Debug.Log(currentLives);
        }
    }

    void UpdateLifeIcons()
    {
        for (int i = 0; i < lifeIcons.Length; i++)
        {
            lifeIcons[i].SetActive(i < currentLives);
        }
    }

    void ShowScorePopup(int points, Vector3 position)
    {
        GameObject popup = Instantiate(scorePopupPrefab, position, Quaternion.identity);
        TextMeshProUGUI popupText = popup.GetComponentInChildren<TextMeshProUGUI>();
        popupText.text = (points > 0 ? "+" : "") + points.ToString();

        Destroy(popup, 0.7f);
    }

    void RefreshScoreDisplay()
    {
        foreach (var textMeshProArray in _textScoreManagerMenu)
        {
            textMeshProArray.text = playerScore.ToString();
        }

        foreach (var pointsDisplayText in _pointsDisplayText)
        {
            pointsDisplayText.text = $"SCORE:{playerScore}";
        }
    }

    void UpdateTimer()
    {
        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            int elapsedMinutes = Mathf.FloorToInt(remainingTime / 60);
            int elapsedSeconds = Mathf.FloorToInt(remainingTime % 60);
            foreach (var countdownDisplayText in _countdownDisplayText)
            {
                countdownDisplayText.text = $"{elapsedMinutes:D2}:{elapsedSeconds:D2}";
            }
        }
        else
        {
            remainingTime = 0;
            TerminateGameplay(false);
        }
    }

    


    void TerminateGameplay(bool didWin)
    {
        isGameOver = true;
        StopAllCoroutines();
        _gameSceneObject.SetActive(false);

        if (didWin)
        {
            PlayerPrefs.SetInt("MaxScore", playerScore);
            PlayerPrefs.SetInt("MaxScoreLevel", _currentLevel);
            PlayerPrefs.Save();
            // Увеличиваем текущий уровень только если победа
            _currentLevel++;
            PlayerPrefs.SetInt(OmniMetrics.ACTIVE_STAGEID, _currentLevel);  // Сохраняем новый уровень
            PlayerPrefs.Save();


            playerCurrency += playerScore;
            PlayerPrefs.SetInt(CurrencyKey, playerCurrency);
            PlayerPrefs.Save();
            winMenu.SetActive(true);
        }
        else
        {
            loseMenu.SetActive(true);
        }
    }



    // Метод для активации бонуса уничтожения всех целей
    public void DestroyAllTargets()
    {
        foreach (var element in spawnedElements)
        {
            if (element != null)
            {
                Destroy(element);  // Уничтожаем все элементы на экране
                playerScore += 10;
                ShowScorePopup(10, transform.position);
                RefreshScoreDisplay();
                anim = element.GetComponent<Animation>(); // Получаем компонент Animation
                PlayDestroyAnimation(element); // Запускаем анимацию уничтожения
            }
        }
        spawnedElements.Clear();  // Очищаем список
    }
    private Animation anim;
    public string destroyAnimationName = "DestroyAnimation"; // Имя анимации для уничтожения элемента
    public GameObject explosionPrefab; // Префаб взрыва
    void PlayDestroyAnimation(GameObject element)
    {
        // Запускаем анимацию уничтожения объекта
        anim.Play(destroyAnimationName);

        // Создаем префаб взрыва в позиции элемента
        GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        
        // Запускаем анимацию взрыва
        Animation explosionAnim = explosion.GetComponent<Animation>();
        if (explosionAnim != null)
        {
            explosionAnim.Play(); // Проигрываем анимацию взрыва
        }

        // Удаляем объект и взрыв через время анимации
        float destroyTime = anim.GetClip(destroyAnimationName).length;
        Destroy(explosion, destroyTime); // Удаляем взрыв через время анимации
        Destroy(element, destroyTime); // Удаляем сам элемент
    }
    
    // Метод для активации бонуса неуязвимости
    public void ActivateBombImmunity()
    {
        StartCoroutine(BombImmunityCoroutine());
    }

    private IEnumerator BombImmunityCoroutine()
    {
        isInvincibleToBombs = true;
        Debug.Log("Bomb immunity is now active.");
        yield return new WaitForSeconds(10f);  // Неуязвимость длится 10 секунд
        isInvincibleToBombs = false;
        Debug.Log("Bomb immunity has expired.");
    }
}