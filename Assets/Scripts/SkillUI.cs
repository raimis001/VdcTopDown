using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillUI : MonoBehaviour
{
    public TMPro.TMP_Text skillName;
    public TMPro.TMP_Text skillLevel;

    public void SetSkill(string name, int level)
    {
        skillName.text = name;
        skillLevel.text = level.ToString();
    }
}
