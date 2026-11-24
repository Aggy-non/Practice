using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class ConveyerBeltMove : MonoBehaviour
{
    private List<GameObject> beltList = new List<GameObject>();
    private List<GameObject> nonRigidBodies = new List<GameObject>();
    [SerializeField] private float speed; //boxMovementSpeed = 0.057f * conveyorSpeed - 0.34f;
    private Vector3 direction;

    private void Start()
    {
        direction = transform.forward;
    }
    private void Update()
    {
        for (int i=0; i<=beltList.Count-1; i++)
        {
            beltList[i].GetComponent<Rigidbody>().AddForce((speed * direction *Time.deltaTime), ForceMode.VelocityChange) ;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Rigidbody>(out Rigidbody component))
        {
            beltList.Add(other.gameObject);
        }
        else
        {
            nonRigidBodies.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (nonRigidBodies.Contains(other.gameObject))
        {
            nonRigidBodies.Remove(other.gameObject);
        }
        else
        {
            beltList.Remove(other.gameObject);
        }
    }


}
