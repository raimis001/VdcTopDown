using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public TrailRenderer trail;

    float strenght;
    float flyTime;
    internal float hitpoints;

    void Update()
    {
        transform.Translate(transform.right * Time.deltaTime * strenght * 18f, Space.World);

        flyTime -=  Time.deltaTime;
        if (flyTime <= 0)
            gameObject.SetActive(false);
    }

    public void Shot(Transform start, float strenght, float hitpoint, float speed)
    {
        trail.Clear();

        transform.position = start.position;
        
        transform.rotation = start.rotation;

        float rnd = 10f * (1.1f - strenght) * Mathf.Lerp(1, 2, Mathf.Clamp(speed, 0, 1));
        
        float z = transform.eulerAngles.z + Random.Range(-rnd, rnd);

        transform.eulerAngles = new Vector3(0, 0, z);


        this.strenght = 1;
        this.hitpoints = hitpoint;

        flyTime = strenght;
        gameObject.SetActive(true);
    }
}
