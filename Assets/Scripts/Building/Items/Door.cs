using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Door : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> doors; //the physical doors attached to this object
    [SerializeField]
    private List<NavMeshObstacle> obstacles; //the navmesh obstacles on the doors
    [SerializeField]
    private float rotationAmount = 90.0f; //how far the doors rotate when opened
    [SerializeField]
    private float speed = 1f; //how long it takes for the doors to open 
    private Dictionary<GameObject, Vector3> startingRotations; //the starting positions of the attached doors
    private int creatureCount; //how many creatures are in proximity to the doors
    private bool isOpen; //if the doors are currently open
    private Coroutine animationCoroutine; //the currently active coroutine controlling the doors rotation

    private void Awake()
    {
        startingRotations = new Dictionary<GameObject, Vector3>();
        foreach (GameObject door in doors) startingRotations.Add(door, door.transform.localRotation.eulerAngles);
        creatureCount = 0;
        isOpen = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Creature")) creatureCount++;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Creature")) creatureCount = Mathf.Max(creatureCount - 1, 0); //I worry there's a chance for the door to not properly count the builder when the door is placed, so this is a precaution
        if (creatureCount == 0 && isOpen) Close();
    }

    public void Open(float dot)
    {
        Debug.Log("Door Open Called");
        if (!isOpen)
        {
            Debug.Log("Door opening");
            isOpen = true;
            if (animationCoroutine != null) StopCoroutine(animationCoroutine);
            animationCoroutine = StartCoroutine(OpenRotation(dot));
        }
    }

    private IEnumerator OpenRotation(float dot)
    {
        Dictionary<GameObject, Quaternion> startRotations = new Dictionary<GameObject, Quaternion>();
        Dictionary<GameObject, Quaternion> endRotations = new Dictionary<GameObject, Quaternion>();
        foreach(GameObject door in doors)
        {
            Quaternion temp = door.transform.localRotation;
            startRotations.Add(door, temp);
            if (dot >= 0)
            {
                if (door.transform.localPosition.x > 0) endRotations.Add(door, Quaternion.Euler(temp.x, temp.y - rotationAmount, temp.z));
                else endRotations.Add(door, Quaternion.Euler(temp.x, temp.y + rotationAmount, temp.z));
            }
            else
            {
                if (door.transform.localPosition.x > 0)  endRotations.Add(door, Quaternion.Euler(temp.x, temp.y + rotationAmount, temp.z));
                else endRotations.Add(door, Quaternion.Euler(temp.x, temp.y - rotationAmount, temp.z));
            }
        }
        float time = 0;
        while (time < 1)
        {
            foreach (GameObject door in doors) door.transform.localRotation = Quaternion.Slerp(startRotations[door], endRotations[door], time);
            yield return null;
            time += Time.deltaTime * speed;
        }
        foreach (GameObject door in doors) door.transform.localRotation = Quaternion.Slerp(startRotations[door], endRotations[door], 1);
        foreach (NavMeshObstacle obstacle in obstacles)
        {
            obstacle.carving = true;
            obstacle.enabled = true;
        }
    }

    public void Close()
    {
        Debug.Log("Door Close called");
        isOpen = false;
        if (animationCoroutine != null) StopCoroutine(animationCoroutine);
        animationCoroutine = StartCoroutine(CloseRotation());
    }

    private IEnumerator CloseRotation()
    {
        foreach (NavMeshObstacle obstacle in obstacles)
        {
            obstacle.carving = false;
            obstacle.enabled = false;
        }

        Dictionary<GameObject, Quaternion> startRotations = new Dictionary<GameObject, Quaternion>();
        Dictionary<GameObject, Quaternion> endRotations = new Dictionary<GameObject, Quaternion>();
        foreach (GameObject door in doors)
        {
            Quaternion temp = door.transform.localRotation;
            startRotations.Add(door, temp);
            endRotations.Add(door, Quaternion.Euler(startingRotations[door]));
        }
        float time = 0;
        while (time < 1)
        {
            foreach (GameObject door in doors) door.transform.localRotation = Quaternion.Slerp(startRotations[door], endRotations[door], time);
            yield return null;
            time += Time.deltaTime * speed;
        }
        foreach (GameObject door in doors) door.transform.localRotation = Quaternion.Slerp(startRotations[door], endRotations[door], 1);
    }
}
