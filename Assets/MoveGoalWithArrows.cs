using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Tilemaps;
using UnityEngine.UI;



public class MoveGoalWithArrows : MonoBehaviour
{
    public Tilemap moveTilemap;
    public Tilemap obstacleTilemap;
    public float moveSpeed = 3f;

    private Vector3 targetPosition;
    private bool isMoving = false;

    private int coinCount = 0;
    public Text coinText;
    private int totalCoins;
    public Image winImage;
    public Image gameOverImage;

    private bool isGameOver = false;


    public AudioSource coinSound;

    void Start()
    {
        targetPosition = transform.position;

        winImage.gameObject.SetActive(false);
        gameOverImage.gameObject.SetActive(false);

        totalCoins = GameObject.FindGameObjectsWithTag("Coin").Length;
        UpdateCoinText();
    }

    void Update()
    {
        if (isMoving || isGameOver)
            return;

        Vector3Int currentCell = moveTilemap.WorldToCell(transform.position);
        Vector3Int newCell = currentCell;

        if (Input.GetKey(KeyCode.UpArrow))
            newCell += Vector3Int.up;
        else if (Input.GetKey(KeyCode.DownArrow))
            newCell += Vector3Int.down;
        else if (Input.GetKey(KeyCode.LeftArrow))
            newCell += Vector3Int.left;
        else if (Input.GetKey(KeyCode.RightArrow))
            newCell += Vector3Int.right;
        else
            return;

        if (newCell != currentCell && moveTilemap.HasTile(newCell) && !obstacleTilemap.HasTile(newCell))
        {
            targetPosition = moveTilemap.CellToWorld(newCell) + new Vector3(0.5f, 0.5f, 0);
            StartCoroutine(SmoothMove());
        }
    }

    IEnumerator SmoothMove()
    {
        isMoving = true;
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPosition;
        isMoving = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Coin"))
        {
            if (coinSound != null)
            {
                coinSound.Play();
            }
            coinCount++;
            UpdateCoinText();
            Destroy(other.gameObject);

            if (coinCount == totalCoins)
            {
                Win();
            }
        }
        else if (other.CompareTag("Enemy"))
        {
            GameOver();
        }
    }

    private void UpdateCoinText()
    {
        coinText.text = "Coins: " + coinCount + "/" + totalCoins;
    }

    private void Win()
    {
        winImage.gameObject.SetActive(true);
        Debug.Log("Vous avez gagné !");
        Time.timeScale = 0;
    }

    private void GameOver()
    {
        isGameOver = true;
        gameOverImage.gameObject.SetActive(true);
        Debug.Log("Game Over !");
        Time.timeScale = 0;
    }
}




















