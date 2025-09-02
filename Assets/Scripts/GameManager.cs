using System.Collections;
using System.Collections.Generic;
using Unity.FPS.AI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Settings")]
    public float matchTime = 180f;
    public int scorePerEnemy = 100;
    public int scorePerSecond = 10;

    [Header("Scene Settings")]
    public string winScene;
    public string loseScene;

    [Header("UI References - Player 1")]
    public Text timeTextP1;
    public Text scoreTextP1;
    public Text enemiesTextP1;
    public Text healthTextP1;
    public GameObject eliminatedPanelP1;

    [Header("UI References - Player 2")]
    public Text timeTextP2;
    public Text scoreTextP2;
    public Text enemiesTextP2;
    public Text healthTextP2;
    public GameObject eliminatedPanelP2;

    [Header("Global UI")]
    public GameObject gameOverPanel;
    public Text gameOverText;

    [Header("Enemy Spawn Settings")]
    public GameObject enemyPrefab;
    public int maxEnemies = 10;
    public float spawnInterval = 5f;
    public Transform[] spawnPoints;

    private float currentTime;
    private int currentScore;
    private int enemiesDestroyed;
    private int totalEnemies;
    private bool gameEnded = false;
    private List<GameObject> activeEnemies = new List<GameObject>();
    private List<PlayerController> activePlayers = new List<PlayerController>();
    private List<PlayerController> eliminatedPlayers = new List<PlayerController>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        currentTime = matchTime;
        UpdateUI();

        PlayerController.OnPlayerInstantiated += OnPlayerInstantiated;
        StartCoroutine(SpawnEnemiesCoroutine());
        StartCoroutine(ScoreOverTimeCoroutine());
    }

    void OnDestroy()
    {
        PlayerController.OnPlayerInstantiated -= OnPlayerInstantiated;

        foreach (PlayerController player in activePlayers)
        {
            if (player != null)
            {
                player.OnLifeChanged -= OnPlayerLifeChanged;
            }
        }
    }

    void Update()
    {
        if (!gameEnded)
        {
            currentTime -= Time.deltaTime;
            UpdateUI();

            if (currentTime <= 0f)
            {
                currentTime = 0f;
                EndGame(false);
            }

            if (totalEnemies > 0 && enemiesDestroyed >= totalEnemies && activeEnemies.Count == 0)
            {
                EndGame(true);
            }
        }
    }

    void OnPlayerInstantiated(PlayerController player)
    {
        activePlayers.Add(player);
        player.OnLifeChanged += OnPlayerLifeChanged;
        player.useGameManager = true;

        // Asignar número de jugador
        if (activePlayers.Count == 1)
        {
            player.playerNumber = 1;
        }
        else if (activePlayers.Count == 2)
        {
            player.playerNumber = 2;
        }

        UpdateUI();
    }

    void OnPlayerLifeChanged(float newLife)
    {
        UpdateUI();
    }

    public void OnPlayerEliminated(PlayerController player)
    {
        if (!eliminatedPlayers.Contains(player))
        {
            eliminatedPlayers.Add(player);
            ShowEliminatedPanel(player.playerNumber);

            // Verificar si el juego debe terminar
            if (eliminatedPlayers.Count >= activePlayers.Count)
            {
                EndGame(false);
            }
            else
            {
                // Juego continúa con un jugador
                Debug.Log($"Jugador {player.playerNumber} eliminado. El juego continúa.");
            }
        }
    }

    void ShowEliminatedPanel(int playerNumber)
    {
        if (playerNumber == 1 && eliminatedPanelP1 != null)
        {
            eliminatedPanelP1.SetActive(true);
        }
        else if (playerNumber == 2 && eliminatedPanelP2 != null)
        {
            eliminatedPanelP2.SetActive(true);
        }
    }

    void HideAllEliminatedPanels()
    {
        if (eliminatedPanelP1 != null) eliminatedPanelP1.SetActive(false);
        if (eliminatedPanelP2 != null) eliminatedPanelP2.SetActive(false);
    }

    IEnumerator SpawnEnemiesCoroutine()
    {
        while (!gameEnded)
        {
            if (activeEnemies.Count < maxEnemies)
            {
                SpawnEnemy();
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    IEnumerator ScoreOverTimeCoroutine()
    {
        while (!gameEnded)
        {
            yield return new WaitForSeconds(1f);
            AddScore(scorePerSecond);
        }
    }

    void SpawnEnemy()
    {
        if (spawnPoints.Length > 0 && enemyPrefab != null)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            activeEnemies.Add(enemy);
            totalEnemies++;
            UpdateUI();
        }
    }

    public void OnEnemyDestroyed(GameObject enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
            enemiesDestroyed++;
            AddScore(scorePerEnemy);
            UpdateUI();
        }
    }

    void AddScore(int points)
    {
        currentScore += points;
        UpdateUI();
    }

    void UpdateUI()
    {
        // Tiempo (igual para ambos jugadores)
        string timeString = $"Tiempo: {Mathf.FloorToInt(currentTime / 60f):00}:{Mathf.FloorToInt(currentTime % 60f):00}";
        if (timeTextP1 != null) timeTextP1.text = timeString;
        if (timeTextP2 != null) timeTextP2.text = timeString;

        // Puntaje
        if (scoreTextP1 != null) scoreTextP1.text = $"Puntos: {currentScore}";
        if (scoreTextP2 != null) scoreTextP2.text = $"Puntos: {currentScore}";

        // Enemigos
        string enemiesString = $"Enemigos: {enemiesDestroyed}/{totalEnemies}";
        if (enemiesTextP1 != null) enemiesTextP1.text = enemiesString;
        if (enemiesTextP2 != null) enemiesTextP2.text = enemiesString;

        // Salud por jugador
        foreach (PlayerController player in activePlayers)
        {
            if (player != null)
            {
                string healthString = $"Salud: {player.Life}";
                if (player.playerNumber == 1 && healthTextP1 != null)
                {
                    healthTextP1.text = healthString;
                }
                else if (player.playerNumber == 2 && healthTextP2 != null)
                {
                    healthTextP2.text = healthString;
                }
            }
        }
    }

    void EndGame(bool isVictory)
    {
        if (gameEnded) return;

        gameEnded = true;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            gameOverText.text = isVictory ? "¡VICTORIA!" : "DERROTA";
        }

        foreach (GameObject enemy in activeEnemies)
        {
            enemy enemyController = enemy.GetComponent<enemy>();
            if (enemyController != null)
            {
                enemyController.enabled = false;
            }
        }

        StartCoroutine(LoadSceneWithDelay(isVictory ? winScene : loseScene, 3f));
    }

    IEnumerator LoadSceneWithDelay(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
        Destroy(gameObject);
    }

    public void RestartGame()
    {
        HideAllEliminatedPanels();
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Destroy(gameObject);
    }
}