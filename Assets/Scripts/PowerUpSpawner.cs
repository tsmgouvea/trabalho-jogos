using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class PowerUpSpawner : MonoBehaviour
{
    public GameObject powerUpPrefab;  // O prefab do power-up
    public float spawnInterval = 10f; // Intervalo de tempo entre os spawns

    public Collider2D gridArea;

    private Snake snake;

    private GameObject currentPowerUp;

    private void Awake()
    {
        snake = FindObjectOfType<Snake>();
    }

    private void Start()
    {
        StartCoroutine(SpawnPowerUpAfterInterval());
    }

    private IEnumerator SpawnPowerUpAfterInterval()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnPowerUp();
        }
    }

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

    private Vector2 GetRandomPosition()
    {
        Bounds bounds = gridArea.bounds;

        // Pick a random position inside the bounds
        // Round the values to ensure it aligns with the grid
        int x = Mathf.RoundToInt(Random.Range(bounds.min.x, bounds.max.x));
        int y = Mathf.RoundToInt(Random.Range(bounds.min.y, bounds.max.y));

        while (snake.Occupies(x, y))
        {
            x++;

            if (x > bounds.max.x)
            {
                x = Mathf.RoundToInt(bounds.min.x);
                y++;

                if (y > bounds.max.y) {
                    y = Mathf.RoundToInt(bounds.min.y);
                }
            }
        }
        
        return new Vector2(x, y);
    }
}
