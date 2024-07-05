﻿using System.Collections;
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
    public Sprite headUp;
    public Sprite headDown;
    public Sprite headLeft;
    public Sprite headRight;
    public Sprite bodyHorizontal;
    public Sprite bodyVertical;
    public Sprite curveTopRight;
    public Sprite curveTopLeft;
    public Sprite curveBottomRight;
    public Sprite curveBottomLeft;
    public Sprite tailUp;
    public Sprite tailDown;
    public Sprite tailLeft;
    public Sprite tailRight;
    private List<Transform> segments = new List<Transform>();
    private Vector2Int input;
    private float nextUpdate;
    public GameOver gameOver;
    public DebuffController debuffController;
    public Collider2D snakeCollider;
    private SpriteRenderer spriteRenderer;
    public AudioSource eatSound;
    public AudioSource powerUpSound;
    public AudioSource wallPassSound;
    public ScoreManager scoreManager;


    private void Start()
    {
        snakeCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        ResetState();
    }

    private void UpdateSprites()
    {
        // Atualizar sprite da cabeça
        if (direction == Vector2Int.up) {
            spriteRenderer.sprite = headUp;
        } else if (direction == Vector2Int.down) {
            spriteRenderer.sprite = headDown;
        } else if (direction == Vector2Int.left) {
            spriteRenderer.sprite = headLeft;
        } else if (direction == Vector2Int.right) {
            spriteRenderer.sprite = headRight;
        }

        // Atualizar sprites do corpo e cauda
        for (int i = 1; i < segments.Count; i++) {
            SpriteRenderer segmentRenderer = segments[i].GetComponent<SpriteRenderer>();
            Vector2Int prevSegmentDirection = Vector2Int.RoundToInt((Vector2)segments[i - 1].position - (Vector2)segments[i].position);
            Vector2Int nextSegmentDirection = Vector2Int.zero;

            if (i < segments.Count - 1) {
                nextSegmentDirection = Vector2Int.RoundToInt((Vector2)segments[i].position - (Vector2)segments[i + 1].position);
            }

            if (i == segments.Count - 1) {
                // Atualizar sprite da cauda
                if (prevSegmentDirection == Vector2Int.up) {
                    segmentRenderer.sprite = tailDown;
                } else if (prevSegmentDirection == Vector2Int.down) {
                    segmentRenderer.sprite = tailUp;
                } else if (prevSegmentDirection == Vector2Int.left) {
                    segmentRenderer.sprite = tailRight;
                } else if (prevSegmentDirection == Vector2Int.right) {
                    segmentRenderer.sprite = tailLeft;
                }
            } else {
                // Atualizar sprite do corpo
                if (prevSegmentDirection.x != 0 && nextSegmentDirection.x != 0) {
                    segmentRenderer.sprite = bodyHorizontal;
                } else if (prevSegmentDirection.y != 0 && nextSegmentDirection.y != 0) {
                    segmentRenderer.sprite = bodyVertical;
                } else {
                    // Identificar curvas
                    if (prevSegmentDirection == Vector2Int.up && nextSegmentDirection == Vector2Int.left ||
                        prevSegmentDirection == Vector2Int.right && nextSegmentDirection == Vector2Int.down) {
                        segmentRenderer.sprite = curveBottomLeft;
                    } else if (prevSegmentDirection == Vector2Int.up && nextSegmentDirection == Vector2Int.right ||
                            prevSegmentDirection == Vector2Int.left && nextSegmentDirection == Vector2Int.down) {
                        segmentRenderer.sprite = curveBottomRight;
                    } else if (prevSegmentDirection == Vector2Int.down && nextSegmentDirection == Vector2Int.left ||
                            prevSegmentDirection == Vector2Int.right && nextSegmentDirection == Vector2Int.up) {
                        segmentRenderer.sprite = curveTopLeft;
                    } else if (prevSegmentDirection == Vector2Int.down && nextSegmentDirection == Vector2Int.right ||
                            prevSegmentDirection == Vector2Int.left && nextSegmentDirection == Vector2Int.up) {
                        segmentRenderer.sprite = curveTopRight;
                    }
                }
            }
        }
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

        // Atualiza os sprites
        UpdateSprites();
    }


    public void Grow()
    {
        Transform segment = Instantiate(segmentPrefab);
        segment.position = segments[segments.Count - 1].position;
        segments.Add(segment);
        scoreManager.AddScore(1);
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
            eatSound.Play();
            debuffController.SwitchCamerasToDefault();
            debuffController.timeSinceEat = 0f;
            debuffController.cameraDebuffDuration = 0f;
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
                wallPassSound.Play();
            } else {
                Debug.Log("Colisão detectada");
                gameOver.EndGame();
            }
        }
        else if (other.gameObject.CompareTag("PowerUp"))
        {
            powerUpSound.Play();
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