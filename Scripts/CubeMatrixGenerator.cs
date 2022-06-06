using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMatrixGenerator : MonoBehaviour
{
    public Vector2Int matrixSize = Vector2Int.zero;
    public Vector3 cubeSize = Vector3.zero;

    public float scale = 2f;
    public float intensity = 1f;
    public float speed = 1;
    public float detailOffset = 100;

    [Range(1, 10)] public int detailLevels = 4;

    public GameObject cube;
    private GameObject[] cubes;


    public GameObject[] GenerateMatrix(Vector2Int matrixSize, Vector3 cubeSize, GameObject prefab)
    {
        GameObject[] cubes = new GameObject[matrixSize.y * matrixSize.x];

        string parentName = "CubesParent";

        if (!GameObject.Find(parentName))
        {
            new GameObject(parentName);
        }
        
        for(int x = 0; x < matrixSize.x; x++)
        {
            for(int y = 0; y < matrixSize.y; y++)
            {
                GameObject instance = Instantiate(prefab, new Vector3(x * cubeSize.x, 0, y * cubeSize.z), Quaternion.identity, GameObject.Find(parentName).transform);
                instance.transform.localScale = new Vector3(cubeSize.x, cubeSize.y, cubeSize.z);
                cubes[y * matrixSize.x + x] = instance;
            }
        }

        return cubes;
    }

    public void CalculateNoise()
    {
        for (int y = 0; y < matrixSize.y; y++)
        {
            for (int x = 0; x < matrixSize.x; x++)
            {
                float offset = Time.time * speed;
                float sample = 0;

                for (int i = 0; i < detailLevels; i++)
                {
                    float xCoord = detailOffset * i + x / (float)matrixSize.x * (scale / detailLevels) * (i + 1);
                    float yCoord = detailOffset * i + y / (float)matrixSize.y * (scale / detailLevels) * (i + 1);

                    sample += Mathf.PerlinNoise(xCoord + offset, yCoord + offset) + Mathf.PerlinNoise(xCoord - offset, yCoord - offset);
                }

                sample /= detailLevels;

                cubes[y * matrixSize.x + x].GetComponent<MeshRenderer>().material.color = new Color(sample / 4, sample / 2, sample);
                cubes[y * matrixSize.x + x].transform.localScale = new Vector3(cubeSize.x, cubeSize.y * sample * intensity, cubeSize.z);
            }
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        cubes = GenerateMatrix(matrixSize, cubeSize, cube);
    }

    // Update is called once per frame
    void Update()
    {
        CalculateNoise();
    }
}
