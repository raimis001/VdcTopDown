using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotor : MonoBehaviour
{
    private void LateUpdate()
    {
        transform.eulerAngles = Vector3.zero;
    }
}
