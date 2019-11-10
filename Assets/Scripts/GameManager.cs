using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class GameManager : MonoBehaviour
{
    static public GameManager S;
    [SerializeField] private Slider _loadingBar;
    [SerializeField] private Slider _healthBar;
    [SerializeField] private CanvasGroup _redFlashPanel;
    [SerializeField] private TextMeshProUGUI _killsText;
    [SerializeField] private TextMeshProUGUI _gameOverText;
    private int _killAmount;

    private void Awake()
    {
        if (S != null)
            Destroy(gameObject);
        S = this;
        DontDestroyOnLoad(gameObject);
        _loadingBar.value = 0;
        _loadingBar.GetComponent<CanvasGroup>().alpha = 0;
        _gameOverText.DOFade(0, 0);
    }

    private void OnEnable()
    {
        Player.OnPlayerDied += HandlePlayerDied;
        Player.OnPlayerTakeDamage += HandlePlayerTakeDamage;
        Enemy.OnEnemyDestroyed += HandleEnemyDestroyed;
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void OnDisable()
    {
        Player.OnPlayerDied -= HandlePlayerDied;
        Player.OnPlayerTakeDamage -= HandlePlayerTakeDamage;
        Enemy.OnEnemyDestroyed -= HandleEnemyDestroyed;
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    private void HandleSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.buildIndex == 0)
        {
            _killsText.text = "";
            _healthBar.GetComponent<CanvasGroup>().alpha = 0;
        }
        else if (arg0.buildIndex == 1)
        {
            _killsText.text = "Kills: 0";
            _healthBar.value = 1;
            _healthBar.GetComponent<CanvasGroup>().alpha = 1;
        }

    }

    private void HandlePlayerDied()
    {
        _gameOverText.DOFade(1, 0.3f);
        StartCoroutine(PlayerDiedCoroutine());
    }

    private IEnumerator PlayerDiedCoroutine()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(0);
    }

    private void HandlePlayerTakeDamage(float currentHealth)
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(_redFlashPanel.DOFade(1, 0.2f));
        seq.Append(_redFlashPanel.DOFade(0, 0.2f));
        seq.Append(_healthBar.DOValue(currentHealth / Player.MAX_HEALTH, 0.2f));
        seq.Play();
    }

    private void HandleEnemyDestroyed(Enemy e)
    {
        _killAmount++;
        _killsText.text = "Kills: " + _killAmount;
    }

    public void LoadGameScene()
    {
        _loadingBar.GetComponent<CanvasGroup>().DOFade(1, 0.2f);
        StartCoroutine(LoadGameSceneCoroutine());
    }

    private IEnumerator LoadGameSceneCoroutine()
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(1);
        while (!ao.isDone)
        {
            _loadingBar.DOValue(ao.progress, 0.1f);
            yield return null;
        }
    }
}
