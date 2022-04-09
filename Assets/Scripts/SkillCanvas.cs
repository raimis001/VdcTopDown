using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillCanvas : MonoBehaviour
{

    public GameObject panel;

    public Transform skillList;
    public SkillUI skillPrefab;

    public SkillTree skillTree;

    readonly List<SkillUI> skillUIList = new List<SkillUI>();

    void Start()
    {
        panel.SetActive(false);

        foreach (SkillClass skill in skillTree.skills)
        {
            SkillUI skillUI = Instantiate(skillPrefab, skillList);
            skillUI.gameObject.SetActive(true);

            skillUI.SetSkill(skill.name, skill.level);
            skillUIList.Add(skillUI);

        }

    }
    private void OnEnable()
    {
        skillTree.OnSklillChange += SkillCange;
        
    }
    private void OnDisable()
    {
        skillTree.OnSklillChange -= SkillCange;
    }

    void SkillCange(int skillId)
    {
        SkillClass skill = skillTree.skills[skillId];

        skillUIList[skillId].SetSkill(skill.name, skill.level);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            panel.SetActive(!panel.activeInHierarchy);
        if (panel.activeInHierarchy && Input.GetKeyDown(KeyCode.Escape))
            panel.SetActive(false);
    }
}
