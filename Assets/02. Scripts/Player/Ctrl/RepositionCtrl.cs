using UnityEngine;

public class RepositionCtrl : MonoBehaviour
{
    private int m_tile_size = 20;

    private void OnTriggerExit2D(Collider2D col)
    {
        if (!col.CompareTag("GroundCheckArea")) return;

        Vector3 player_pos = GameManager.Player.transform.position;
        Vector3 my_pos = transform.position;
        
        float diff_x = Mathf.Abs(player_pos.x - my_pos.x);
        float diff_y = Mathf.Abs(player_pos.y - my_pos.y);

        Vector3 player_dir = GameManager.Player.joyStick.GetInputVector();
        float dir_x = player_dir.x > 0 ? 1 : -1;
        float dir_y = player_dir.y > 0 ? 1 : -1;

        switch(transform.tag)
        {
            case "Ground":
                if(diff_x>diff_y)
                {
                    transform.Translate(Vector3.right * dir_x * m_tile_size * 2);
                }
                else if (diff_x < diff_y)
                {
                    transform.Translate(Vector3.up * dir_y * m_tile_size * 2);
                }
                break;
        }
    }
}
