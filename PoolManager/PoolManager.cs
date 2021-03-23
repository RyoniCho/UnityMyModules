using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : SingletonBase<PoolManager> {

	
	protected override void Awake()
	{
		
        base.Awake();
        if(Instance!=this)
        {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(this.gameObject);
	}

	public Pool[] pools;
	Dictionary<string,Pool> dicTotalPools = new Dictionary<string, Pool> ();

	public void CreateDefaultPool()
	{

		for (int i = 0; i < pools.Length; i++) 
		{
			pools [i].CreatePool (this.transform);
			dicTotalPools.Add (pools [i].poolName, pools [i]);
		}
	}
	public void RegisterObjectToPool(Pool poolObj)
	{
		poolObj.CreatePool(this.transform);
		dicTotalPools.Add(poolObj.poolName,poolObj);
		UnityEngine.Debug.Log($"Register Object To Pool :{poolObj.poolName}");
	}

	public void RegisterObjectToPool(string objectName,int poolSize,GameObject gameObj)
	{
		Pool pool=new Pool();
		pool.poolName=objectName;
		pool.poolSize=poolSize;
		pool.poolingObj=gameObj;

		pool.CreatePool(this.transform);
		dicTotalPools.Add(pool.poolName,pool);
		UnityEngine.Debug.Log($"Register Object To Pool :{pool.poolName}");

	}

	public void UnregisterObjectFromPool(string objectName)
	{
		Pool pool;
		if(dicTotalPools.TryGetValue(objectName,out pool))
		{
			pool.ClearPool();
			dicTotalPools.Remove(objectName);
		}
		else
		{
			Debug.LogError($"{objectName} is not contained from pool");
		}
	}

	public GameObject SpawnObject(string poolName)
	{

		Pool pool;
		if (dicTotalPools.TryGetValue (poolName, out pool)) 
		{
			return pool.SpawnObject (this.transform);

		} 
		else 
		{
			Debug.LogError ("Error: "+poolName + "is not contain pools");
			return null;
		}

	}

	public GameObject SpawnObject(string poolName,Transform spawnTransformInfo)
	{

		Pool pool;
		if (dicTotalPools.TryGetValue (poolName, out pool)) 
		{
			return pool.SpawnObject (this.transform,Vector3.zero,Quaternion.identity,spawnTransformInfo);

			
		} 
		else 
		{
			Debug.LogError ("Error: "+poolName + "is not contain pools");
			return null;
		}

	}

	public GameObject SpawnObject(string poolName,Vector3 spawnPosition, Quaternion spawnRotation)
	{

		Pool pool;
		if (dicTotalPools.TryGetValue (poolName, out pool)) 
		{
			return pool.SpawnObject (this.transform,spawnPosition,spawnRotation);

		} 
		else 
		{
			Debug.LogError ("Error: "+poolName + "is not contain pools");
			return null;
		}

	}

    public void SpawnObject(string poolName,Transform trans,float despawnDelay)
    {
        Pool pool;
        if (dicTotalPools.TryGetValue(poolName, out pool))
        {
            GameObject obj=pool.SpawnObject(this.transform, Vector3.zero, Quaternion.identity, trans);

            DespawnObject(poolName,obj,despawnDelay);
        }
        else
        {
            Debug.LogError("Error: " + poolName + "is not contain pools");

        }


    }
    public void SpawnObject(string poolName, Vector3 spawnPosition, Quaternion spawnRotation, float despawnDelay)
    {

        Pool pool;
        if (dicTotalPools.TryGetValue(poolName, out pool))
        {
            GameObject obj=pool.SpawnObject(this.transform, spawnPosition, spawnRotation);
            DespawnObject(poolName, obj, despawnDelay);

        }
        else
        {
            Debug.LogError("Error: " + poolName + "is not contain pools");
           
        }

    }

	public void DespawnObject(string poolName,GameObject obj,float delay=0)
	{

		Pool pool;
		if (dicTotalPools.TryGetValue (poolName, out pool)) 
		{
			if (delay > 0)
				StartCoroutine (DelayDespawn (pool, obj, delay));
			else
				pool.DespawnObject (obj,this.transform);

		} 
		else 
		{
			Debug.LogError ("Error: "+poolName + "is not contain pools");

		}

	}
	IEnumerator DelayDespawn(Pool pool,GameObject obj,float delay)
	{
		yield return new WaitForSeconds (delay);

		pool.DespawnObject(obj,this.transform);
	}


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
			
				Destroy (obj.gameObject, 2f);
			}
			listPool.Clear ();

		}

		public void CreatePool(Transform parent)
		{
			for (int i = 0; i < poolSize; i++) 
			{
                Transform obj = Instantiate(poolingObj.transform, Vector3.zero, Quaternion.identity) as Transform;
                obj.gameObject.SetActive(false);
                obj.SetParent (parent);
                listPool.Add (obj.gameObject);
			}
		}

		public GameObject SpawnObject(Transform parent,Vector3 spawnPosition=default(Vector3),Quaternion spawnRotation=default(Quaternion),Transform spawnTransformInfo=null)
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
			obj.SetActive (true);

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
			GameObject obj = Instantiate (poolingObj, Vector3.zero, Quaternion.identity) as GameObject;
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
}
