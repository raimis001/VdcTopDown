using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BushController : MonoBehaviour
{
    public float timeToGrow = 10;
    public string itemID = "Berrie";
    public int itemCount = 3;
    public Transform fruits;

    private int status = 0; //0 - Growing state, 1 - Ready state
    private float currentTime;

    void Start()
    {
        fruits.gameObject.SetActive(false);
        currentTime = timeToGrow;
        status = 0;
    }

    void Update()
    {
        if (status != 0)
            return;

        currentTime -= Time.deltaTime;
        if (currentTime > 0)
            return;

        fruits.gameObject.SetActive(true);
        status = 1;
        currentTime = 0;
    }

    private void OnMouseDown()
    {
        if (status != 1)
            return;

        Vector2 pos = Viking.instance.transform.position;
        float distance = Vector2.Distance(transform.position, pos);
        if (distance > 1)
        {
            Debug.Log(distance);
            return;
        }

        fruits.gameObject.SetActive(false);
        status = 0;
        currentTime = timeToGrow;

        Viking.instance.inventory.Add(itemID, itemCount);

    }

    private void OnMouseOver()
    {
        //Debug.Log("Bush: " + itemID);
    }
}
