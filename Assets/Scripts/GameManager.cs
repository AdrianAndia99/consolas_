using System.Collections;
using System.Collections.Generic;
using Unity.FPS.AI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

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



    // NUEVO MÉTODO: Se llama cuando una escena se carga
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Escena cargada: {scene.name}");
        ResetGameState();
    }

    // NUEVO MÉTODO: Reiniciar el estado del juego
    void ResetGameState()
    {
        Debug.Log("Reiniciando estado del juego");

        // Limpiar todas las listas
        activePlayers.Clear();
        eliminatedPlayers.Clear();
        activeEnemies.Clear();

        // Reiniciar variables de estado
        currentTime = matchTime;
        currentScore = 0;
        enemiesDestroyed = 0;
        totalEnemies = 0;
        gameEnded = false;

        // Ocultar paneles de eliminación
        HideAllEliminatedPanels();

        // Ocultar panel de game over
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        Debug.Log("Estado del juego reiniciado");
        UpdateUI();
    }

    void Start()
    {
        // Buscar jugadores existentes al inicio
        FindExistingPlayers();
        currentTime = matchTime;
        UpdateUI();
        StartCoroutine(SpawnEnemiesCoroutine());
        StartCoroutine(ScoreOverTimeCoroutine());
    }
    void FindExistingPlayers()
    {
        // REEMPLAZA con el nuevo método no obsoleto
        PlayerController[] players = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
        foreach (PlayerController player in players)
        {
            OnPlayerInstantiated(player);
        }
    }
    void OnDestroy()
    {
        Debug.Log("GameManager destruyéndose");
        PlayerController.OnPlayerInstantiated -= OnPlayerInstantiated;
        UnsubscribeAllPlayers();

        // Desuscribir del evento de escena
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void UnsubscribeAllPlayers()
    {
        foreach (PlayerController player in activePlayers)
        {
            if (player != null)
            {
                player.OnLifeChanged -= OnPlayerLifeChanged;
            }
        }
        activePlayers.Clear();
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

    public void OnPlayerInstantiated(PlayerController player)
    {
        Debug.Log($"Jugador {player.playerNumber} instanciado");

        // Limpiar jugadores null o duplicados
        activePlayers.RemoveAll(p => p == null || p == player);

        // Desuscribir primero por si ya estaba suscrito
        player.OnLifeChanged -= OnPlayerLifeChanged;

        // Suscribir al evento de vida
        player.OnLifeChanged += OnPlayerLifeChanged;
        player.useGameManager = true;

        // Asignar número de jugador si no tiene o está duplicado
        if (player.playerNumber == 0 || PlayerNumberExists(player.playerNumber))
        {
            player.playerNumber = GetNextAvailablePlayerNumber();
        }

        activePlayers.Add(player);

        Debug.Log($"Jugador {player.playerNumber} agregado. Total: {activePlayers.Count}");
        UpdateUI();
    }

    // NUEVO MÉTODO: Verificar si un número de jugador ya existe
    bool PlayerNumberExists(int playerNumber)
    {
        foreach (PlayerController p in activePlayers)
        {
            if (p != null && p.playerNumber == playerNumber)
            {
                return true;
            }
        }
        return false;
    }

    // NUEVO MÉTODO: Obtener el siguiente número de jugador disponible
    int GetNextAvailablePlayerNumber()
    {
        for (int i = 1; i <= 2; i++)
        {
            if (!PlayerNumberExists(i))
            {
                return i;
            }
        }
        return 1; // Fallback
    }

    void OnPlayerLifeChanged(float newLife)
    {
        Debug.Log($"Evento OnPlayerLifeChanged recibido: {newLife}");
        UpdateUI();
    }

    public void OnPlayerEliminated(PlayerController player)
    {
        if (!eliminatedPlayers.Contains(player))
        {
            Debug.Log($"Jugador {player.playerNumber} eliminado");
            eliminatedPlayers.Add(player);
            ShowEliminatedPanel(player.playerNumber);

            if (eliminatedPlayers.Count >= activePlayers.Count)
            {
                EndGame(false);
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
        // Limpiar jugadores null antes de actualizar UI
        activePlayers.RemoveAll(p => p == null);

        Debug.Log($"UpdateUI - Jugadores activos: {activePlayers.Count}");

        // Tiempo
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

        // Salud
        foreach (PlayerController player in activePlayers)
        {
            if (player != null)
            {
                string healthString = $"Salud: {Mathf.RoundToInt(player.Life)}";

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

        // Para jugadores que no existen
        if (healthTextP1 != null && !PlayerExists(1)) healthTextP1.text = "Salud: 0";
        if (healthTextP2 != null && !PlayerExists(2)) healthTextP2.text = "Salud: 0";
    }

    bool PlayerExists(int playerNumber)
    {
        foreach (PlayerController player in activePlayers)
        {
            if (player != null && player.playerNumber == playerNumber)
            {
                return true;
            }
        }
        return false;
    }

    void EndGame(bool isVictory)
    {
        if (gameEnded) return;

        gameEnded = true;
        Debug.Log(isVictory ? "VICTORIA" : "DERROTA");

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            gameOverText.text = isVictory ? "ˇVICTORIA!" : "DERROTA";
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
    }

    public void RestartGame()
    {
        HideAllEliminatedPanels();
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}