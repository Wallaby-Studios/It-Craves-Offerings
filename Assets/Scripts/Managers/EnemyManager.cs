using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType {
    Rusher,
    Blaster,
    Strafer,
    Sniper,
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
    private Transform enemyParentTransform, enemySpawnAreaTransform;
    [SerializeField]
    private GameObject rusherPrefab, blasterPrefab, straferPrefab, sniperPrefab, bossPrefab;

    private Dictionary<EnemyType, GameObject> enemyMap;
    private KeyValuePair<Vector2, Vector2> enemySpawnArea;

    // Start is called before the first frame update
    void Start()
    {
        enemyMap = new Dictionary<EnemyType, GameObject>();
        enemyMap.Add(EnemyType.Rusher, rusherPrefab);
        enemyMap.Add(EnemyType.Blaster, blasterPrefab);
        enemyMap.Add(EnemyType.Strafer, straferPrefab);
        enemyMap.Add(EnemyType.Sniper, sniperPrefab);

        Vector2 minPos = new Vector2(
            enemySpawnAreaTransform.position.x - enemySpawnAreaTransform.localScale.x / 2,
            enemySpawnAreaTransform.position.y - enemySpawnAreaTransform.localScale.y / 2);
        Vector2 maxPos = new Vector2(
            enemySpawnAreaTransform.position.x + enemySpawnAreaTransform.localScale.x / 2,
            enemySpawnAreaTransform.position.y + enemySpawnAreaTransform.localScale.y / 2);
        enemySpawnArea = new KeyValuePair<Vector2, Vector2>(minPos, maxPos);
    }

    public GameObject SpawnRandomEnemy() {
        List<EnemyType> types = new List<EnemyType>(enemyMap.Keys);
        EnemyType randomEnemyType = types[Random.Range(0, types.Count)];
        return SpawnEnemy(randomEnemyType);
    }

    public GameObject SpawnEnemy(EnemyType enemyType) {
        Vector2 randomPosition = new Vector2(
            Random.Range(enemySpawnArea.Key.x, enemySpawnArea.Value.x),
            Random.Range(enemySpawnArea.Key.y, enemySpawnArea.Value.y));
        // Spawn the enemy into the scene
        return Instantiate(enemyMap[enemyType], randomPosition, Quaternion.identity, enemyParentTransform);
    }

    public GameObject SpawnBoss() {
        return Instantiate(bossPrefab, enemySpawnAreaTransform.position, Quaternion.identity, enemyParentTransform);
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
