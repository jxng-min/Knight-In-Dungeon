using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ESevering : Severing
{
    private BoxCollider2D[][] m_col_groups = new BoxCollider2D[5][];

    [SerializeField] private BoxCollider2D[] m_combo1_cols;
    [SerializeField] private BoxCollider2D[] m_combo2_cols;
    [SerializeField] private BoxCollider2D[] m_combo3_cols;
    [SerializeField] private BoxCollider2D[] m_combo4_cols;
    [SerializeField] private BoxCollider2D[] m_combo5_cols;

    private int m_current_comb_index = 0; // 현재 진행 중인 콤보 인덱스

    protected override void Awake()
    {
        base.Awake();
        m_col_groups[0] = m_combo1_cols;
        m_col_groups[1] = m_combo2_cols;
        m_col_groups[2] = m_combo3_cols;
        m_col_groups[3] = m_combo4_cols;
        m_col_groups[4] = m_combo5_cols;
    }

    private void OnEnable()
    {
        m_current_comb_index = 0;
        m_col_index = 0;
        PlayCombo(m_current_comb_index);
    }

    private void PlayCombo(int index)
    {
        string ani_name = $"Combo{index+1}";
        m_animator.Play(ani_name);
    }

    public void SoundPlay()
    {
        string sfx_name = "";
        switch(m_current_comb_index)
        {
            case 0:
                sfx_name = "ESevering1";
                break;
            case 1:
                sfx_name = "ESevering2";
                break;
            case 2:
                sfx_name = "ESevering3";
                break;
            case 3:
                sfx_name = "ESevering4";
                break;
            case 4:
                sfx_name = "ESevering5";
                break;
        }
        SoundManager.Instance.PlayEffect(sfx_name);
    }

    public override void ColActive()
    {
        if (m_col_index - 1 < 0)
        {
            m_col_groups[m_current_comb_index][m_col_index].enabled = true;
            m_col_index++;
            return;
        }
        m_col_groups[m_current_comb_index][m_col_index - 1].enabled = false;
        m_col_groups[m_current_comb_index][m_col_index].enabled = true;
        m_col_index++;
    }
    public override void AniEnd()
    {
        m_col_groups[m_current_comb_index][m_col_index - 1].enabled = false;
        m_col_index = 0;
        m_current_comb_index++;
        PlayCombo(m_current_comb_index);
    }

    public void AllAniEnd()
    {
        m_col_groups[m_current_comb_index][m_col_index - 1].enabled = false;
        transform.gameObject.SetActive(false);
    }
    
}
