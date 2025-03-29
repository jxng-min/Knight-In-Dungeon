using UnityEngine;

public class CameraMoveCtrl : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Player == null) return;

        Vector3 player_pos = GameManager.Player.transform.position;
        transform.position = new Vector3(player_pos.x,player_pos.y,-10f);
    }
}
