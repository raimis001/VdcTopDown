using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

[Flags]
public enum BuildKind
{
    None = 0,
    Campfire = 1,
}

public class BuildUI : MonoBehaviour
{
    public static bool buildMode;

    public GameObject buildPanel;

    public List<GameObject> buildList;

    GameObject selected;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            SwitchBuildPanel();
        }


        if (Input.GetMouseButtonDown(1) && selected)
        {
            Destroy(selected);
            selected = null;
            return;
        }

        if (!buildMode)
            return;

        if (!selected)
            return;

        if (EventSystem.current.IsPointerOverGameObject())
            return;

        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0;

        selected.transform.position = pos;

        if (Input.GetMouseButtonDown(0))
        {
            selected = null;
        }

        World.Rebuild();

    }

    public void SwitchBuildPanel()
    {
        buildPanel.SetActive(!buildPanel.activeInHierarchy);
        buildMode = buildPanel.activeInHierarchy;
    }

    public void Build(int kind)
    {
        if (selected)
        {
            Destroy(selected);
            selected = null;
        }

        selected = Instantiate(buildList[kind]);

        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0;

        selected.transform.position = pos;

        
    }

    
}
