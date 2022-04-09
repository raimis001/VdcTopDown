using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Flags]
public enum SkillKind {
    Swords = 0,
    Bows = 1,
    Shields = 2,
    Running = 3,
    Health = 4,
    Stamina = 5
}

[Serializable]
public class SkillClass
{
    public string name;
    public int level;
    public float current;

    public float currentLevel => Mathf.Sqrt(level + 1);
}


[CreateAssetMenu(fileName = "SkillTree", menuName = "Viking/Skill Tree", order = 1)]
public class SkillTree : ScriptableObject
{
    public List<SkillClass> skills;

    public Action<int> OnSklillChange;

    public void AddSkill(SkillKind skillID)
    {
        SkillClass skill = skills[(int)skillID];

        skill.current += 0.5f / (skill.level + 1);
        if (skill.current >= 1)
        {
            skill.current = 0;
            skill.level++;
        }
        
        OnSklillChange?.Invoke((int)skillID);
    }

    public float GetSkill(SkillKind skillID)
    {
        return skills[(int)skillID].currentLevel;
    }

    public int GetLevel(SkillKind skillID)
    {
        return skills[(int)skillID].level;
    }


    [ContextMenu("Clear values")]
    public void Clear()
    {
        foreach (SkillClass skill in skills)
        {
            skill.level = 0;
            skill.current = 0;
        }
    }
}
