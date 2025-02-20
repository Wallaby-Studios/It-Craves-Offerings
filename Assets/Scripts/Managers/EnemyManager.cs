using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType {
    Ranged
}

public class EnemyManager : MonoBehaviour
{
    #region Singleton Code
    // A public reference to this script
    public static EnemyManager instance = null;

    // Awake is called even before start 
    // (I think its at the very beginning of runtime)
    private void Awake() {
        // If the reference for this script is null, assign it this script
        if(instance == null)
            instance = this;
        // If the reference is to something else (it already exists)
        // than this is not needed, thus destroy it
        else if(instance != this)
            Destroy(gameObject);
    }
    #endregion

    [SerializeField]
    private Transform enemyParentTransform;
    [SerializeField]
    private GameObject rangedEnemyPrefab;

    private Dictionary<EnemyType, GameObject> enemyMap;

    // Start is called before the first frame update
    void Start()
    {
        enemyMap = new Dictionary<EnemyType, GameObject>();
        enemyMap.Add(EnemyType.Ranged, rangedEnemyPrefab);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject SpawnEnemy(EnemyType enemyType) {
        Vector2 spawnPosition = Vector2.zero;
        return Instantiate(enemyMap[enemyType], spawnPosition, Quaternion.identity, enemyParentTransform);
    }

    public void CheckForRemainingEnemies() {
        // Check if any enemies remain with health
        for(int i = 0; i < enemyParentTransform.childCount; i++) {
            if(enemyParentTransform.GetChild(i).GetComponent<Enemy>().Health > 0) {
                return;
            }
        }
        // If no enemies remain with health, notify the RoomManager that the room is cleared
        RoomManager.instance.CombatRoomCleared();
    }
}
