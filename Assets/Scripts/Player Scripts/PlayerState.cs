using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerState : MonoBehaviour
{
    public Animator animator;

    private StatControler statControler;

    private void Start()
    {
        statControler = GetComponentInParent<StatControler>();
    }

    public void LoadNextLevel() {
        Debug.Log("load");
        Map.MapManager.instance.LoadNextLevel();
    }

    public void JoinNextLevel()
    {
        animator.SetBool("Falling", false);
        Map.MapManager.instance.JoinNextLevel();
        HealthStat h = statControler?.GetStatOfType(StatType.HealthStat) as HealthStat;
        if (h != null)
            h.addValue(-1 * h.BaseValue * .5f);
    }
}
