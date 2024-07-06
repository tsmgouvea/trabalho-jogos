using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    public GameObject GameOverCanvas;
    public Snake snakeController; // Importa a Snake
    public ScoreManager scoreManager;
    public Text scoreTextGameOver;
    public Food food;
    public PowerUpSpawner powerUpSpawner;
    public DebuffController debuffController;
    private bool isGameOver = false;

    // Garante que a tela de Game Over comece inativa
    private void Start()
    {
        GameOverCanvas.SetActive(false);
    }

    // Ativa a tela de game over e pausa o jogo
    public void EndGame()
    {
        if (!isGameOver && snakeController != null)
        {
            Debug.Log("Game Over Triggered");

            // Desabilita o colisor da Snake para evitar colisões fantasmas
            snakeController.snakeCollider.enabled = false;
            Debug.Log("Snake Collider disabled");

            scoreTextGameOver.text = "Score: " + scoreManager.GetScore().ToString();
            scoreManager.ResetScore();

            GameOverCanvas.SetActive(true); // Activate the Game Over Canvas
            Time.timeScale = 0f; // Pause the game
            isGameOver = true;
        }
    }

    // Reinicia o jogo 
    public void Restart()
    {
        Debug.Log("Restarting the game"); 
        Time.timeScale = 1f; // Despausar o jogo 
        GameOverCanvas.SetActive(false); // Desativar a tela de Game Over

        StartCoroutine(RestartCoroutine());
    }

    // Coroutine para reiniciar o jogo
    private IEnumerator RestartCoroutine()
    {
        snakeController.ResetState(); // Reiniciar o estado da Snake 

        food.RandomizePosition(); 
        powerUpSpawner.ResetPowerUp();
        debuffController.ReseteCameras();

        yield return new WaitForSeconds(0.1f); // Esperar para evitar colisões fantasmas 
        snakeController.EnableCollider(); // Reabilitar o colisor da Snake
        isGameOver = false; // Indica que o jogo não foi terminado
        Debug.Log("Game restarted");
    }
}
