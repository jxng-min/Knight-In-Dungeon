using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private StageManager m_stage_manager;

    [Header("스테이지 목록")]
    [SerializeField] private StageData[] m_stage_list;


    [Header("스테이지 정보")]
    [SerializeField] private StageData m_current_stage;
    public StageData Stage
    {
        get { return m_current_stage; }
    }

    [Header("경고 UI 프리펩")]
    [SerializeField] private GameObject m_warning_ui_object;

    [Header("타일 재배치 매니저")]
    [SerializeField] private TileRepositionManager m_tile_manager;

    private int m_current_wave_index;
    private int m_spawned_count;
    private float m_wave_timer;
    private float m_spawn_timer;
    private bool m_wave_active;
    private int m_boss_count;
    public int BossCount
    {
        get { return m_boss_count; }
        set { m_boss_count = value; }
    }
    private bool m_boss_spawn;

    private float m_out_radius = 10f;
    private float m_in_radius = 5f;

    private bool m_warning = false;

    private void Awake()
    {
        m_current_stage = m_stage_list[DataManager.Instance.Data.m_current_stage - 1];

        m_stage_manager = GetComponent<StageManager>();

        float total_time = 0f;
        foreach(WaveData wave in Stage.Waves)
        {
            if(wave.Pattern.EndType is WaveEndType.Time)
            {
                total_time += (wave.Pattern as TimeWave).Duration;
            }
            else if(wave.Pattern.EndType is WaveEndType.Boss)
            {
                total_time += (wave.Pattern as BossWave).Duration;
            }
        }

        m_stage_manager.GameTimer = total_time;
        m_stage_manager.OriginTimer = total_time;

        StartStage(Stage);
    }
    
    private void Update()
    {
        if(GameManager.Instance.GameState != GameEventType.Playing || !m_wave_active)
        {
            return;
        }

        UpdateWave();
        Recycle();
    }

    public void StartStage(StageData stage)
    {
        m_current_stage = stage;
        m_current_wave_index = 0;
        StartWave();
    }

    private void StartWave()
    {
        m_wave_active = true;
        m_spawned_count = 0;
        m_wave_timer = 0f;
        m_spawn_timer = 0f;
        m_warning = false;
    }

    private void NextWave()
    {
        m_current_wave_index++;

        if(m_current_wave_index >= Stage.Waves.Length)
        {
            m_wave_active = false;
            
            GameEventBus.Publish(GameEventType.Clear);

            return;
        }

        StartWave();
    }

    private void UpdateWave()
    {
        if(m_current_wave_index >= Stage.Waves.Length)
        {
            m_wave_active = false;
            return;
        }

        WaveData wave = m_current_stage.Waves[m_current_wave_index];
        Wave pattern = wave.Pattern;

        switch(pattern.EndType)
        {
            case WaveEndType.Count:
            {
                CountWave count_pattern = pattern as CountWave;
                if(m_spawned_count < count_pattern.Count)
                {
                    Spawn(wave);
                    m_spawned_count++;
                }
                else
                {
                    NextWave();
                }

                break;
            }

            case WaveEndType.Time:
            {
                TimeWave time_pattern = pattern as TimeWave;

                m_wave_timer += Time.deltaTime;
                m_spawn_timer += Time.deltaTime;

                if(m_spawn_timer >= time_pattern.Interval)
                {
                    Spawn(wave);
                    m_spawn_timer = 0f;
                }

                if(m_wave_timer + 10 >= time_pattern.Duration  && !m_warning)
                {
                    if(m_current_wave_index + 1 < m_current_stage.Waves.Length)
                    {
                        if(m_current_stage.Waves[m_current_wave_index + 1].Pattern.EndType is WaveEndType.Boss)
                        {
                            m_warning = true;
                            Instantiate(m_warning_ui_object);
                        }
                    }
                }

                if(m_wave_timer >= time_pattern.Duration)
                {
                    NextWave();
                }

                break;
            }

            case WaveEndType.Boss:
            {
                BossWave boss_pattern = pattern as BossWave;

                m_wave_timer += Time.deltaTime;
                
                if(m_boss_spawn is false)
                {
                    m_boss_spawn = true;
                    m_boss_count++;
                    Spawn(wave);
                }

                GameObject[] enemies = ObjectManager.Instance.GetActiveObjects(ObjectType.Enemy);
                foreach(GameObject enemy in enemies)
                {
                    var enemy_ctrl = enemy.GetComponent<EnemyCtrl>(); 
                    if(!enemy_ctrl.Script.Boss)
                    {
                        enemy_ctrl.Die();
                    }
                }

                if(m_wave_timer >= boss_pattern.Duration)
                {
                    GameManager.Instance.Player.UpdateHP(-99999999);
                }

                if(m_boss_count == 0)
                {
                    m_boss_spawn = false;
                    m_boss_count = 0;

                    GameManager.Instance.StageManager.GameTimer -= boss_pattern.Duration - m_wave_timer;
                    NextWave();
                }

                break;
            }
        }
    }

    private void Spawn(WaveData wave)
    {
        Enemy enemy_data = SelectEnemy(wave.Enemies);

        GameObject obj = ObjectManager.Instance.GetObject(ObjectType.Enemy);
        EnemyCtrl prev_enemy = obj.GetComponent<EnemyCtrl>();
        if(prev_enemy)
        {
            Destroy(prev_enemy);
        }

        EnemyCtrl enemy = null;
        switch(enemy_data.EnemyType)
        {
            case EnemyType.Melee:
                enemy = obj.AddComponent<MeleeEnemyCtrl>();
                break;
            
            case EnemyType.Ranged:
                enemy = obj.AddComponent<RangedEnemyCtrl>();
                break;
            
            case EnemyType.Suicide:
                enemy = obj.AddComponent<SuicideEnemyCtrl>();
                break;
        }
        
        Vector2 spawn_position = GetValidSpawnPosition(wave.Pattern.Pattern);

        enemy.transform.position = spawn_position;
        enemy.Script = enemy_data;
        enemy.Initialize();
    }

    private Vector2 GetValidSpawnPosition(SpawnPattern spawn_type)
    {
        int attempt = 0;

        while(attempt < 10)
        {
            Vector2 current_spawn_pos = GetSpawnPosition(spawn_type);

            foreach(Transform tile in m_tile_manager.Tiles)
            {
                Debug.Log("들어옴");
                if(tile.GetComponent<BoxCollider2D>().OverlapPoint(current_spawn_pos))
                {
                    Debug.Log("걸림");
                    return current_spawn_pos;
                }
            }
            attempt++;
        }

        return Vector2.zero;
    }

    private Vector2 GetSpawnPosition(SpawnPattern spawn_type)
    {
        Vector2 center = GameManager.Instance.Player.transform.position;

        switch(spawn_type)
        {
            case SpawnPattern.Random_Around_Player:
            {
                float angle = Random.Range(0f, 2f * Mathf.PI);
                float distance = Random.Range(m_in_radius, m_out_radius);

                Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;
                
                return center + offset;
            }

            case SpawnPattern.From_One_Direction:
            {
                return center + Vector2.right * m_out_radius;
            }

            case SpawnPattern.Surround_Player:
            {
                int direction_index = Random.Range(0, 8);
                float angle_step = 360f / 8f;
                float rad = Mathf.Deg2Rad * (angle_step * direction_index);

                return center + new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * m_out_radius;
            }

            case SpawnPattern.Center:
            {
                return center;
            }

            default:
                return center;
        }
    }

    private Enemy SelectEnemy(Enemy[] enemies)
    {
        if(enemies.Length == 1)
        {
            return enemies[0];
        }
        
        return enemies[UnityEngine.Random.Range(0, enemies.Length)];
    }

    private void Recycle()
    {
        GameObject[] enemies = ObjectManager.Instance.GetActiveObjects(ObjectType.Enemy);

        foreach(var enemy in enemies)
        {
            float distance = Vector2.Distance(GameManager.Instance.Player.transform.position, enemy.transform.position);

            if(distance > m_out_radius)
            {
                var enemy_ctrl = enemy.GetComponent<EnemyCtrl>();
                if(enemy_ctrl)
                {
                    ObjectManager.Instance.ReturnObject(enemy, ObjectType.Enemy);
                }
            }
        }
    }
}
