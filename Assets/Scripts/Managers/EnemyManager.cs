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
    private Transform enemyParentTransform, enemySpawnTransform;
    [SerializeField]
    private List<GameObject> enemyPrefabs;
    [SerializeField]
    private GameObject bossPrefab;

    private Dictionary<EnemyType, GameObject> enemyMap;

    // Start is called before the first frame update
    void Start()
    {
        enemyMap = new Dictionary<EnemyType, GameObject>();
    }

    public List<GameObject> SpawnEnemy(EnemyType enemyType, int count) {
        // Spawn in a specified number (count) of enemies and return them in a list
        List<GameObject> enemies = new List<GameObject>();
        for(int i = 0; i < count; i++) {
            enemies.Add(SpawnRandomEnemy());
        }
        return enemies;
    }

    public GameObject SpawnRandomEnemy() {
        GameObject randomEnemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
        // Spawn the enemy into the scene
        return Instantiate(randomEnemyPrefab, enemySpawnTransform.position, Quaternion.identity, enemyParentTransform);
    }

    public GameObject SpawnBoss() {
        return Instantiate(bossPrefab, enemySpawnTransform.position, Quaternion.identity, enemyParentTransform);
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

    public void ClearEnemies() {
        // Destroy all enemies in the room
        for(int i = enemyParentTransform.childCount - 1; i >= 0; i--) {
            Destroy(enemyParentTransform.GetChild(i).gameObject);
        }
    }
}
