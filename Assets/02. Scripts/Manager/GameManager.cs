using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static PlayerCtrl Player;

    void Start()
    {
        Player = GameObject.Find("Player").GetComponent<PlayerCtrl>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
