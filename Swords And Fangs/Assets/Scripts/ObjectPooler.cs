using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectPooler : MonoBehaviour
{
    GameObject[] poolHolders;

    [SerializeField] bool recycle = true;
    [SerializeField] List<EnemyMovement> enemyPrefabs;
    public Dictionary<int, Queue<EnemyMovement>> enemyPoolDictionary;
    [SerializeField] List<Projectile> projectilePrefabs;
    public Dictionary<int, Queue<Projectile>> projectilePoolDictionary;
    [SerializeField] List<CombatText> combatTextPrefabs;
    public Dictionary<int, Queue<CombatText>> combatTextPoolDictionary;
    [SerializeField] ParticleEffect enemyImpactPrefab;
    Queue<ParticleEffect> enemyImpactPool;
    [SerializeField] AudioPlayer audioPlayerPrefab;
    Queue<AudioPlayer> audioPlayerPool;

    public static ObjectPooler Instance;

    void Awake()
    {
        if (!Instance)
            Instance = this;
        CreatePools();
    }   

    void CreatePools()
    {
        poolHolders = new GameObject[5];

        enemyPoolDictionary = new Dictionary<int, Queue<EnemyMovement>>();
        poolHolders[0] = new GameObject("Enemy Pool");
        foreach (EnemyMovement prefab in enemyPrefabs)
        {
            Queue<EnemyMovement> enemyPool = new Queue<EnemyMovement>();
            enemyPoolDictionary.Add(prefab.id, enemyPool);
        }

        projectilePoolDictionary = new Dictionary<int, Queue<Projectile>>();
        poolHolders[1] = new GameObject("Projectile Pool");
        foreach (Projectile prefab in projectilePrefabs)
        {
            Queue<Projectile> projectilePool = new Queue<Projectile>();
            projectilePoolDictionary.Add(prefab.id, projectilePool);
        }

        combatTextPoolDictionary = new Dictionary<int, Queue<CombatText>>();
        poolHolders[2] = new GameObject("Combat Text Pool");
        foreach (CombatText prefab in combatTextPrefabs)
        {
            Queue<CombatText> combatTextPool = new Queue<CombatText>();
            combatTextPoolDictionary.Add(prefab.id, combatTextPool);
        }

        poolHolders[3] = new GameObject("Enemy Impact Pool");
        enemyImpactPool = new Queue<ParticleEffect>();

        poolHolders[4] = new GameObject("Audio Player Pool");
        audioPlayerPool = new Queue<AudioPlayer>();

        for (int i = 0; i < poolHolders.Length; i++)
            poolHolders[i].transform.SetParent(transform);
    }

    public void RecycleEnemy(EnemyMovement enemy)
    {
        Recycle(enemy, enemyPoolDictionary[enemy.id], 0);
    }

    public void RecycleProjectile(Projectile projectile)
    {
        Recycle(projectile, projectilePoolDictionary[projectile.id], 1);
    }

    public void RecycleCombatText(CombatText combatText)
    {
        Recycle(combatText, combatTextPoolDictionary[combatText.id], 2);
    }

    public void RecycleEnemyImpact(ParticleEffect enemyImpact)
    {
        Recycle(enemyImpact, enemyImpactPool, 3);
    }

    public void RecycleAudioPlayer(AudioPlayer audioPlayer)
    {
        Recycle(audioPlayer, audioPlayerPool, 4);
    }

    void Recycle<T>(T objectToRecycle, Queue<T> pool, int poolHolderIndex) where T : MonoBehaviour
    {
        if (!recycle)
        {
            Destroy(objectToRecycle);
            return;
        }
        objectToRecycle.gameObject.SetActive(false);
        pool.Enqueue(objectToRecycle);
        objectToRecycle.transform.SetParent(poolHolders[poolHolderIndex].transform);
    }

    public EnemyMovement GetEnemy(int enemeyId, Vector3 pos, Quaternion rot,
                                  ObjectData enemyData)
    {
        return Get(enemyPrefabs[enemeyId], enemyPoolDictionary, enemeyId, pos, rot,
                   enemyData);
    }

    public Projectile GetProjectile(int projectileId, Vector3 pos, Quaternion rot,
                                    ObjectData projectileData)
    {
        return Get(projectilePrefabs[projectileId], projectilePoolDictionary, projectileId, pos, rot,
                   projectileData);
    }

    public CombatText GetCombatText(int combatTextId, Vector3 pos, Quaternion rot,
                                    ObjectData combatTextData)
    {
        return Get(combatTextPrefabs[combatTextId], combatTextPoolDictionary, combatTextId, pos, rot,
                   combatTextData);
    }

    public ParticleEffect GetEnemyImpact(Vector3 pos, Quaternion rot,
                                      ObjectData enemyImpactData)
    {
        return Get(enemyImpactPrefab, enemyImpactPool, pos, rot,
                   enemyImpactData);
    }

    public AudioPlayer GetAudioPlayer(Vector3 pos, Quaternion rot,
                                      ObjectData enemySpawnData)
    {
        return Get(audioPlayerPrefab, audioPlayerPool, pos, rot,
                   enemySpawnData);
    }

    T Get<T>(T prefab, Dictionary<int, Queue<T>> poolDictionary, int id, Vector3 pos, Quaternion rot,
             ObjectData objectData) where T : MonoBehaviour, IPooledObject
    {
        if (!poolDictionary.ContainsKey(id))
        {
            Debug.LogWarning("There is no existing " + prefab.name + " pool with id " + id);
            return null;
        }
        Queue<T> pool = poolDictionary[id];
        return Get(prefab, pool, pos, rot, objectData);
    }

    T Get<T>(T prefab, Queue<T> pool, Vector3 pos, Quaternion rot,
         ObjectData objectData) where T : MonoBehaviour, IPooledObject
    {
        T objectToGet = (pool.Count == 0 || !recycle)
                      ? Instantiate(prefab, pos, rot)
                      : pool.Dequeue();
        objectToGet.transform.rotation = rot;
        objectToGet.gameObject.SetActive(true);
        objectToGet.transform.position = pos;

        objectToGet.Initialise(objectData);

        return objectToGet;
    }
}