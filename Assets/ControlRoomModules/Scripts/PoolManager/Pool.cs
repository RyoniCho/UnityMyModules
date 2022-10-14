using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pool
{
    public string poolName;
    public GameObject poolingObj;
    public int poolSize;
    private int currentPoolIndex;

    List<GameObject> listPool=new List<GameObject>();
    Dictionary<int,int> dicSpawnedList =new Dictionary<int, int>();


    public void ClearPool()
    {
        foreach (var obj in listPool) {
        
            UnityEngine.GameObject.Destroy (obj.gameObject, 2f);
        }
        listPool.Clear ();

    }

    public void CreatePool(Transform parent)
    {
        for (int i = 0; i < poolSize; i++) 
        {
            Transform obj = UnityEngine.GameObject.Instantiate(poolingObj.transform, Vector3.zero, Quaternion.identity) as Transform;
            obj.gameObject.SetActive(false);
            obj.SetParent (parent);
            listPool.Add (obj.gameObject);
        }
    }
    public T GetPreSpawnObject<T>(Transform parent, Vector3 spawnPosition = default(Vector3), Quaternion spawnRotation = default(Quaternion), Transform spawnTransformInfo = null)
    {
        var gameObject = this.SpawnObject(parent, spawnPosition: spawnPosition, spawnRotation: spawnRotation, spawnTransformInfo: spawnTransformInfo, preSpawn: true);

        T component = gameObject.GetComponent<T>();

        return component;
    }

    public GameObject SpawnObject(Transform parent,Vector3 spawnPosition=default(Vector3),Quaternion spawnRotation=default(Quaternion),Transform spawnTransformInfo=null, bool preSpawn = false)
    {
        //현재 풀링인덱스 저장 
        int saveIndex = currentPoolIndex;

        while (true) 
        {
            //이미 스폰되어 있다면 풀링인덱스를 더해준다.
            if (dicSpawnedList.ContainsValue (currentPoolIndex))
                currentPoolIndex++;
            //스폰 안된 인덱스라면 break
            else
                break;
            //if current index is over poolsize, make it zero 
            if (currentPoolIndex >= poolSize)
                currentPoolIndex = 0;
            //if current index equals saveCurrentIndex, it means that there is no object to pool so that it has to extend pool
            if (currentPoolIndex == saveIndex) 
            {
                ExtendPool (parent);
                currentPoolIndex = listPool.Count - 1;
                break;
            }
        }

        GameObject obj=listPool [currentPoolIndex];
        obj.transform.SetParent (null);

        if (spawnTransformInfo != null)
            obj.transform.SetPositionAndRotation (spawnTransformInfo.position, spawnTransformInfo.rotation);
        else 
        {
            
            obj.transform.position = spawnPosition;
            obj.transform.rotation =  spawnRotation;
        }

        if (preSpawn == false)
            obj.SetActive(true);

        dicSpawnedList.Add (obj.GetHashCode (), currentPoolIndex);

        currentPoolIndex++;
        if (currentPoolIndex >= poolSize)
            currentPoolIndex = 0;
        
        return obj;
                
    }

    public void DespawnObject(GameObject obj,Transform parent)
    {
        dicSpawnedList.Remove (obj.GetHashCode ());
        
        obj.transform.SetParent(parent);
        obj.SetActive(false);

    }

    void AddPool(Transform parent)
    {
        GameObject obj = UnityEngine.GameObject.Instantiate (poolingObj, Vector3.zero, Quaternion.identity) as GameObject;
        obj.transform.SetParent (parent);
        obj.SetActive (false);
        listPool.Add (obj);
    }

    void ExtendPool(Transform parent)
    {
        AddPool (parent);
        poolSize++;

        Debug.LogWarning(poolName+ ":PoolSize Extend");
    }
}
