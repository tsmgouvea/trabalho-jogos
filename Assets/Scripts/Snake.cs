using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

[RequireComponent(typeof(BoxCollider2D))]
public class Snake : MonoBehaviour
{
    public Transform segmentPrefab;
    public Vector2Int direction = Vector2Int.right;
    public float speed = 20f;
    public float speedMultiplier = 1f;
    public int initialSize = 4;
    public bool moveThroughWalls = false;
    private List<Transform> segments = new List<Transform>();
    private Vector2Int input;
    private float nextUpdate;
    public GameOver gameOver;
    public Collider2D snakeCollider;

    private void Start()
    {
        snakeCollider = GetComponent<Collider2D>();
        ResetState();
    }

    private void Update()
    {
        // Only allow turning up or down while moving in the x-axis
        if (direction.x != 0f)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) {
                input = Vector2Int.up;
            } else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) {
                input = Vector2Int.down;
            }
        }
        // Only allow turning left or right while moving in the y-axis
        else if (direction.y != 0f)
        {
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) {
                input = Vector2Int.right;
            } else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) {
                input = Vector2Int.left;
            }
        }
    }

    private void FixedUpdate()
    {
        // Wait until the next update before proceeding
        if (Time.time < nextUpdate) {
            return;
        }

        // Set the new direction based on the input
        if (input != Vector2Int.zero) {
            direction = input;
        }

        // Atualiza a posição da Snake (Loop em ordem reversa visto que a cabeça é o índice 0)
        for (int i = segments.Count - 1; i > 0; i--) {
            segments[i].position = segments[i - 1].position;
        }

        // Move the snake in the direction it is facing
        // Round the values to ensure it aligns to the grid
        int x = Mathf.RoundToInt(transform.position.x) + direction.x;
        int y = Mathf.RoundToInt(transform.position.y) + direction.y;
        transform.position = new Vector2(x, y);

        // Set the next update time based on the speed
        nextUpdate = Time.time + (1f / (speed * speedMultiplier));
    }

    public void Grow()
    {
        Transform segment = Instantiate(segmentPrefab);
        segment.position = segments[segments.Count - 1].position;
        segments.Add(segment);
    }

    // Desabilita o colisor da Snake 
    public void DisableCollider()
    {
        snakeCollider.enabled = false;
        Debug.Log("Colisor da Snake desabilitado");
    }

    // Habilita o colisor da Snake
    public void EnableCollider()
    {
        snakeCollider.enabled = true;
        Debug.Log("Colisor da Snake habilitado");
    }

    public void ResetState()
    {
        // Inicia a Snake na posição (0,0)
        direction = Vector2Int.right;
        transform.position = Vector3.zero;

        // Destrói os segmentos da Snake
        foreach (Transform segment in segments.Skip(1))
        {
            Destroy(segment.gameObject);
        }
        
        // Reconstrói a Snake
        segments.Clear();
        segments.Add(this.transform);
        
        // Aumenta o tamanho inicial da Snake
        for (int i = 1; i < initialSize; i++) 
        {
            Grow();
        }

        // Habilita o colisor da Snake
        snakeCollider.enabled = true;
        if (snakeCollider != null)
        {
            Debug.Log("Colisor da Snake habilitado");
        }
    }

    public bool Occupies(int x, int y)
    {
        foreach (Transform segment in segments)
        {
            if (Mathf.RoundToInt(segment.position.x) == x &&
                Mathf.RoundToInt(segment.position.y) == y) {
                return true;
            }
        }

        return false;
    }

    // Detecta colisões e toma providências
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Food"))
        {
            Grow();
        }
        else if (other.gameObject.CompareTag("Obstacle"))
        {
            Debug.Log("Colisão detectada");
            gameOver.EndGame();
        }
        else if (other.gameObject.CompareTag("Wall"))
        {
            if (moveThroughWalls) {
                Traverse(other.transform);
            } else {
                Debug.Log("Colisão detectada");
                gameOver.EndGame();
            }
        }
    }

    private void Traverse(Transform wall)
    {
        Vector3 position = transform.position;

        if (direction.x != 0f) {
            position.x = Mathf.RoundToInt(-wall.position.x + direction.x);
        } else if (direction.y != 0f) {
            position.y = Mathf.RoundToInt(-wall.position.y + direction.y);
        }

        transform.position = position;
    }

    public void EnableWallPass(float duration)
    {
        moveThroughWalls = true;
        StartCoroutine(DisableWallPassAfterTime(duration));
    }

    private IEnumerator DisableWallPassAfterTime(float duration)
    {
        yield return new WaitForSeconds(duration);
        moveThroughWalls = false;
    }
}
