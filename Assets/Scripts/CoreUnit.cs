using UnityEngine;

public class CoreUnit : MonoBehaviour
{
    private bool _isClick = false;
    public bool isBomb; // Это бомба или обычный элемент?
    public bool isLive;
    public int _scoreNumber;
    public GameObject explosionPrefab; // Префаб взрыва
    public string destroyAnimationName = "DestroyAnimation"; // Имя анимации для уничтожения элемента
    
    [SerializeField] private int _numberScore;
    
    private EntityFabricator spawner;
    private bool _isArcade;
    private ElementSpawnerArcade spawnerr;
    private Animation anim;

    void Start()
    {
        if (FindObjectOfType<EntityFabricator>() != null)
        {
            spawner = FindObjectOfType<EntityFabricator>(); // Ищем объект спавнера на сцене
            _isArcade = false;
        }
        else
        {
            spawnerr = FindObjectOfType<ElementSpawnerArcade>(); // Ищем объект спавнера на сцене
            _isArcade = true;
        }
        anim = GetComponent<Animation>(); // Получаем компонент Animation
    }

    // Метод, который вызывается при клике на элемент
    void OnMouseDown()
    {
        if (!_isClick)
        {
            if (_isArcade)
            {
                spawnerr.OnElementClick(isBomb, isLive, _numberScore, transform.position); // Передаем позицию элемента
            }
            else
            {
                spawner.OnElementClick(isBomb, isLive, _numberScore, transform.position); // Передаем позицию элемента
            }
           
            PlayDestroyAnimation(); // Запускаем анимацию уничтожения
            _isClick = true;
        }
    }

    // Метод для воспроизведения анимации и создания взрыва
    void PlayDestroyAnimation()
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
        Destroy(gameObject, destroyTime); // Удаляем сам элемент
    }
}