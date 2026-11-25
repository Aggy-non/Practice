using System;
using UnityEngine;

public class MovePlayerWithBox : MonoBehaviour
{
    private const string playerTag = "Player";
    [SerializeField] private Transform Box;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            other.gameObject.transform.parent = Box;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            other.gameObject.transform.parent = null;
        }
    }
}
