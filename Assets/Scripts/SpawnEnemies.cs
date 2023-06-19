using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemies : MonoBehaviour
{
    [SerializeField] private GameObject enemy;
    [SerializeField] private Transform player;

    private System.Random random;
    private int minEnemies = 10;
    private int maxEnemies = 15;
    private float spawnTime = 60f;
    private float elapsedTime = 0f;

    private void Start()
    {
        random = new System.Random();

        Spawn();
    }

    private void FixedUpdate()
    {
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

        for (int i = 0; i < enemiesQuantity; i++)
        {
            GameObject prefabInstace = Instantiate(
                enemy,
                new Vector3(
                    player.position.x,
                    player.position.y + 2f,
                    player.position.z + 2f
                ),
                Quaternion.identity
            );
        }
    }
}
