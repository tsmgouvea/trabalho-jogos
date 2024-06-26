using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class PowerUpSpawner : MonoBehaviour
{
    public GameObject powerUpPrefab;  // O prefab do power-up.
    public float spawnInterval = 10f; // Intervalo de tempo entre os spawns

    public Collider2D gridArea;

    private Snake snake; // Declara a snake enste arquivo.

    private GameObject currentPowerUp;

    private void Awake()
    {
        snake = FindObjectOfType<Snake>();
        if (snake == null)
        {
            Debug.LogError("Script da Snake não encontrado na cena.");
        }
    }

    private void Start()
    {
        StartCoroutine(SpawnPowerUpAfterInterval());
    }
    
    // Spawna um novo powerup em uma posição aleatória
    private void SpawnPowerUp()
    {
        if (currentPowerUp != null)
        {
            currentPowerUp.SetActive(true);
            currentPowerUp.transform.position = GetRandomPosition();
        }
        else
        {
            currentPowerUp = Instantiate(powerUpPrefab, GetRandomPosition(), Quaternion.identity);
        }
    }

    private IEnumerator DisablePowerUpAfterLifetime(GameObject powerUp)
    {
        // Espera pelo tempo de vida do power-up
        yield return new WaitForSeconds(spawnInterval);
        if (powerUp != null && powerUp.activeSelf)
        {
            powerUp.SetActive(false);
        }
    }

    private Vector2 GetRandomPosition()
    {
        Bounds bounds = gridArea.bounds;

        // Escolhe uma posição aleatória dentro dos limites
        // Arredonda os valores para garantir que eles se alinhem à grade
        int x = Mathf.RoundToInt(Random.Range(bounds.min.x, bounds.max.x));
        int y = Mathf.RoundToInt(Random.Range(bounds.min.y, bounds.max.y));

        // Garante que a posição não está ocupada pela cobra
        while (snake.Occupies(x, y))
        {
            x = Mathf.RoundToInt(Random.Range(bounds.min.x, bounds.max.x));
            y = Mathf.RoundToInt(Random.Range(bounds.min.y, bounds.max.y));
        }

        return new Vector2(x, y);
    }

    public void OnPowerUpConsumed()
    {
        StartCoroutine(SpawnPowerUpAfterInterval());
    }

    private IEnumerator SpawnPowerUpAfterInterval()
    {
        yield return new WaitForSeconds(spawnInterval);
        SpawnPowerUp();
    }
}
