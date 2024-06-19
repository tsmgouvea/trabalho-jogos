using UnityEngine;

public class WallPassPowerUp : MonoBehaviour
{
    public float wallPassDuration = 5f; // Duração da habilidade de atravessar paredes

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Snake"))
        {
            Snake snake = other.GetComponent<Snake>();
            if (snake != null)
            {
                snake.EnableWallPass(wallPassDuration);
            }

            // Encontra o PowerUpSpawner e notifica que o power-up foi consumido
            PowerUpSpawner spawner = FindObjectOfType<PowerUpSpawner>();
            if (spawner != null)
            {
                spawner.OnPowerUpConsumed();
            }

            gameObject.SetActive(false); // Desativa o power-up em vez de destruí-lo
        }
    }
}
