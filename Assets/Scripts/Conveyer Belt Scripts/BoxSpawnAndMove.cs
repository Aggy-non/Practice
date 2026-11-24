using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

public class BoxSpawnAndMove : MonoBehaviour
{
    [SerializeField] private float gapBetweenBoxes;
    [SerializeField] private float movementSpeed; //boxMovementSpeed = 0.057f * conveyorSpeed - 0.34f;
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;
    [SerializeField] private Transform respawnPoint;
    [SerializeField] GameObject box;

    [SerializeField] private List<GameObject> BoxPool = new List<GameObject>();
    private float beltDistance; //entire conveyor belts distance
    private int boxCount;
    private float boxEdgeLength; //assuming box is a cube


    private void Start()
    {
        CalculateAmountOfBoxes();
        CreateBoxPool();
        PlaceBoxes();
    }

    private void Update()
    {
        MoveBoxes();
    }
    private void CalculateAmountOfBoxes()
    {
        beltDistance = Vector3.Distance(startPoint.position, endPoint.position);//find the belt distance
        boxEdgeLength = box.GetComponent<BoxCollider>().size.x;// confirm box size
        boxCount = Mathf.FloorToInt((beltDistance + gapBetweenBoxes) / (boxEdgeLength + gapBetweenBoxes)); //calculate box count
        float neededDistance = (boxCount * boxEdgeLength) + (boxCount * gapBetweenBoxes);
        respawnPoint.position= startPoint.position + startPoint.forward*neededDistance;
    }

    private void CreateBoxPool()
    {
        for (int i = 0; i < boxCount; i++)
        {
            GameObject spawnBox = Instantiate(box);
            spawnBox.SetActive(false);
            BoxPool.Add(spawnBox);
        }
    }
    private void PlaceBoxes()
    {
        for (int i = 0; i < BoxPool.Count; i++)
        {
            Vector3 pos = startPoint.position + transform.forward * i * (boxEdgeLength + gapBetweenBoxes);
            BoxPool[i].transform.position = pos;
            BoxPool[i].SetActive(true);

        }
    }

    private void MoveBoxes()
    {
        for (int i = 0; i < BoxPool.Count; i++)
        {
            if (Vector3.Distance(BoxPool[i].transform.position, endPoint.position) <= 0.1)
            {
                BoxPool[i].SetActive(false);
            }
            if (Vector3.Distance(BoxPool[i].transform.position, respawnPoint.position) >= 0.1)
            {
                BoxPool[i].transform.position = Vector3.MoveTowards(BoxPool[i].transform.position, respawnPoint.position, movementSpeed * Time.deltaTime);
            }
            else
            {
                BoxPool[i].transform.position = startPoint.position;
                BoxPool[i].SetActive(true);
            }
        }
    }
        
    }
