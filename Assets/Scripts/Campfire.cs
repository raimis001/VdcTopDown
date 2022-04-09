using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Campfire : MonoBehaviour
{
    public float radius = 3;
    public float healthSpeed = 1;

    private void Update()
    {
        float dist = Vector2.Distance(Viking.instance.transform.position, transform.position );
        if (dist > radius)
            return;

        Viking.instance.Healing(Time.deltaTime * healthSpeed);

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
