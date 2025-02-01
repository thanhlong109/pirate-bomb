using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombPool : MonoBehaviour
{
    public static BombPool Instance { get; private set; }
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private int poolSize = 10;

    private Queue<GameObject> bombPool = new Queue<GameObject>();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject bomb = Instantiate(bombPrefab);
            bomb.SetActive(false);
            bombPool.Enqueue(bomb);
        }
    }

    public GameObject GetBomb(Vector3 position)
    {
        if (bombPool.Count > 0)
        {
            GameObject bomb = bombPool.Dequeue();
            bomb.transform.position = position;
            bomb.SetActive(true);
            return bomb;
        }
       return null;
    }

    public void ReturnBomb(GameObject bomb)
    {
        bomb.SetActive(false);
        bombPool.Enqueue(bomb);
    }
}
