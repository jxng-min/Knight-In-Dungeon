using UnityEngine;
using static UnityEditor.PlayerSettings;

public class CameraMoveCtrl : MonoBehaviour
{
    private float m_y_offset = 0.3f;
    private int m_stage;
    private Vector2 min;
    private Vector2 max;

    void Start()
    {
        m_stage = DataManager.Instance.Data.m_current_stage;
        switch (m_stage)
        {
            case 2:
                min = new Vector2(-9999, -7.7f);
                max = new Vector2(9999, 7);
                break;
            case 3:
                min = new Vector2(-18, -16);
                max = new Vector2(18, 18);
                break;
        }
    }

    void Update()
    {
        if (GameManager.Instance.Player == null) return;

        Vector3 player_pos = GameManager.Instance.Player.transform.position;
        Vector3 target_pos;

        if (m_stage == 2 || m_stage == 3)
        {
            float clamped_x = Mathf.Clamp(player_pos.x, min.x, max.x);
            float clamped_y = Mathf.Clamp(player_pos.y , min.y, max.y);
            target_pos = new Vector3(clamped_x, clamped_y, -10f);
        }
        else
        {
            target_pos = new Vector3(player_pos.x, player_pos.y + m_y_offset, -10f);
        }
        transform.position = target_pos;


    }
}
