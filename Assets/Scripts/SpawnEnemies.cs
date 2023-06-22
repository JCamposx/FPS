using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemies : MonoBehaviour
{
    [SerializeField] private GameObject enemy;
    [SerializeField] private Transform player;

    private System.Random random;
    private int minEnemies = 15;
    private int maxEnemies = 20;
    private float spawnRadius = 3f;
    private float spawnTime = 60f;
    private float elapsedTime = 0f;

    private void Start()
    {
        random = new System.Random();

        Spawn();
    }

    private void FixedUpdate()
    {
        if (player.GetComponent<PlayerController>().health <= 0f) return;

        VerifySpawn();
    }

    private void VerifySpawn()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime < spawnTime) return;

        Spawn();

        elapsedTime = 0f;

        if (spawnTime > 10f) spawnTime -= 5f;
    }

    public void Spawn()
    {
        int enemiesQuantity = random.Next(minEnemies, maxEnemies + 1);

        float angleIncrement = 360f / enemiesQuantity;

        for (int i = 0; i < enemiesQuantity; i++)
        {
            float angle = i * angleIncrement;

            float spawnX = player.position.x + spawnRadius * Mathf.Cos(Mathf.Deg2Rad * angle);
            float spawnZ = player.position.z + spawnRadius * Mathf.Sin(Mathf.Deg2Rad * angle);

            GameObject prefabInstace = Instantiate(
                enemy,
                new Vector3(
                    spawnX,
                    player.position.y + 2f,
                    spawnZ
                ),
                Quaternion.identity
            );
        }
    }
}
