using UnityEngine;

public class Skill4_HolyBow : Skill5_MagicMissile
{
    private float m_bow_cool_time = 0.8f;
    private float m_damage_e_level_ratio = 1f; // 스킬 만랩의 레벨별 공격력 배수

    private float m_bow_damage_ratio = 1.0f; // 스킬의 공격력 계수
    private float m_bow_damage_level_ratio = 1f; // 레벨별 공격력 배수
    private float m_bow_damage_levelup_ratio = 0.15f; //레벨업시 공격력 배수가 증가하는 수치
    private float m_bos_cool_time_decrease = 0.1f;

    new void Awake()
    {
        m_cool_time = m_bow_cool_time;
        m_bullet = SkillBullet.Arrow;
        m_damage_level_ratio = m_damage_e_level_ratio;
        m_skill5_damage_ratio = m_bow_damage_ratio;
        m_damage_level_ratio = m_bow_damage_level_ratio;
        m_damage_levelup_ratio = m_bow_damage_levelup_ratio;
        m_cool_time_decrease = m_bos_cool_time_decrease;
        m_sfx_name = "Holy Bow SFX";
    }

    protected override void ApplyLevelUpEffect(int level)
    {
        m_damage_level_ratio += m_damage_levelup_ratio;

        m_cool_time -= m_cool_time_decrease;
        
    }
}
