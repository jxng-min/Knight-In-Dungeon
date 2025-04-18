using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Severing : BulletBase
{
    public float Damage { get; set; }

    public float Heal { get; set; }

    [SerializeField]
    private BoxCollider2D[] m_collders;

    private void OnEnable()
    {
        StartCoroutine(EnableColliders());
    }

    public void ExpandArea(float ratio)
    {
        transform.localScale *= ratio;
    }

    public void Update()
    {
        GameStateCheck();
    }

    protected IEnumerator EnableColliders()
    {
        HashSet<float> triggered_points = new HashSet<float>(); // 중복 실행 방지

        while (true)
        {
            
            //현재 실행중인 애니메이션의 정보 불러옴
            AnimatorStateInfo info = m_animator.GetCurrentAnimatorStateInfo(0);
            float n_time = info.normalizedTime; // 값을 시간으로 정규

            if (n_time >= 1f)
            {
                m_collders[2].enabled = false;
                transform.gameObject.SetActive(false);
                yield break; // 코루틴 종료
            }

            if (n_time >= 0.3f && !triggered_points.Contains(0.3f))
            {
                m_collders[0].enabled = true;
                triggered_points.Add(0.3f);
            }

            if (n_time >= 0.45f && !triggered_points.Contains(0.45f))
            {
                m_collders[0].enabled = false;
                m_collders[1].enabled = true;
                triggered_points.Add(0.45f);
            }

            if (n_time >= 0.65f && !triggered_points.Contains(0.65f))
            {
                m_collders[1].enabled = false;
                m_collders[2].enabled = true;
                triggered_points.Add(0.65f);
            }

            yield return null; // 다음 프레임까지 대기
        }
    }

    protected void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy"))
        {
            float heal_ratio = GameManager.Instance.Player.OriginStat.HP * (Heal / 100f);
            GameManager.Instance.Player.UpdateHP(heal_ratio);
            col.GetComponent<EnemyCtrl>().UpdateHP(-Damage);

            GameObject damage_indicator = ObjectManager.Instance.GetObject(ObjectType.DamageIndicator);
            
            damage_indicator.GetComponent<DamageIndicator>().Initialize(Damage);
            damage_indicator.transform.position = col.transform.position;
        }
    }
}
