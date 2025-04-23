using UnityEngine;
using System.Collections.Generic;

public class TileRepositionManager : MonoBehaviour
{
    public GameObject[] m_tile_prefabs;
    private Transform m_player;
    public int m_tile_size = 20;
    private int m_stage2_tile_size = 32;
    private List<Transform> m_tiles = new List<Transform>();
    public List<Transform> Tiles
    {
        get { return m_tiles; }
    }
    
    private Vector2Int m_last_center;
    private int m_now_stage;
    void Start()
    {
        m_now_stage = DataManager.Instance.Data.m_current_stage;
        switch (m_now_stage)
        {
            case 1:
                for (int y = -1; y <= 1; y++)// 초기 3x3 타일 생성
                {
                    for (int x = -1; x <= 1; x++)
                    {
                        Vector3 pos = new Vector3(x * m_tile_size, y * m_tile_size, 0);
                        GameObject tile = Instantiate(m_tile_prefabs[0], pos, Quaternion.identity, transform);
                        m_tiles.Add(tile.transform);
                    }
                }
                break;
            case 2:
                for (int x = -1; x <= 1; x++)
                {
                    Vector3 pos = new Vector3(x * m_stage2_tile_size, 8, 0); 
                    GameObject tile = Instantiate(m_tile_prefabs[1], pos, Quaternion.identity, transform);
                    m_tiles.Add(tile.transform);
                }
                break;
            case 3:
                    GameObject tile_3 = Instantiate(m_tile_prefabs[2], Vector3.zero, Quaternion.identity, transform);
                    m_tiles.Add(tile_3.transform);
                break;
            case 4:
                for (int y = -1; y <= 1; y++)// 스테이지 4 초기 3x3 타일 생성
                {
                    for (int x = -1; x <= 1; x++)
                    {
                        Vector3 pos = new Vector3(x * m_tile_size, y * m_tile_size, 0);
                        GameObject tile = Instantiate(m_tile_prefabs[3], pos, Quaternion.identity, transform);
                        m_tiles.Add(tile.transform);
                    }
                }
                break;
        }


        m_last_center = new Vector2Int(0, 0);
    }

    void Update()
    {
        if (GameManager.Instance.GameState is not GameEventType.Playing) return;

        if(m_player == null) m_player = GameManager.Instance.Player.transform;

        Vector2Int current_center = GetPlayerTileIndex(); // 플레이어의 현재 위치 불러옴


        switch (m_now_stage)
        {
            case 1:
                if (current_center != m_last_center) // 현재 플레이어가 있는 타일 위치와 다르면 타일 재배치
                {
                    RepositionTiles(current_center);
                    m_last_center = current_center;
                }
                break;
            case 2:
                if (current_center.x != m_last_center.x)
                {
                    RepositionTiles_Horizontal(current_center.x);
                    m_last_center = current_center;
                }
                break;
            case 4:
                if (current_center != m_last_center) 
                {
                    RepositionTiles(current_center);
                    m_last_center = current_center;
                }
                break;
        }
    }

    private Vector2Int GetPlayerTileIndex()
    {
        return new Vector2Int(
            Mathf.FloorToInt(m_player.position.x / m_tile_size),
            Mathf.FloorToInt(m_player.position.y / m_tile_size)
        );
    }
    private void RepositionTiles(Vector2Int center)
    {
        int i = 0;
        for (int y = -1; y <= 1; y++)
        {
            for (int x = -1; x <= 1; x++)
            {
                Vector2Int offset = new Vector2Int(x, y);
                Vector3 new_pos = new Vector3((center.x + offset.x) * m_tile_size,(center.y + offset.y) * m_tile_size,0);
                m_tiles[i].position = new_pos;
                i++;
            }
        }
    }

    private void RepositionTiles_Horizontal(int center_x)
    {
        for (int i = 0; i < 3; i++)
        {
            int offset_x = i - 1; // -1, 0, 1
            Vector3 pos = new Vector3((center_x + offset_x) * m_stage2_tile_size, 8, 0);
            m_tiles[i].position = pos;
        }
    }
}
