using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ControlRoom
{
	public class PoolManager : SingletonBase<PoolManager> 
	{

		protected override void Awake()
		{
			
			base.Awake();
			if(Instance!=this)
			{
				Destroy(this.gameObject);
				return;
			}

			DontDestroyOnLoad(this.gameObject);

			if(autoCreatePool)
			{
				CreateDefaultPool();
			}

		}
		public bool autoCreatePool=false;
		
		public Pool[] pools;
		private Dictionary<string,Pool> dicTotalPools = new Dictionary<string, Pool> ();
		private bool isAlreadyCreatedPool=false;

		public void CreateDefaultPool()
		{
			if(isAlreadyCreatedPool)
				return;

			for (int i = 0; i < pools.Length; i++) 
			{
				pools [i].CreatePool (this.transform);
				dicTotalPools.Add (pools [i].poolName, pools [i]);
			}

			isAlreadyCreatedPool=true;
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

        public T GetPreSpawnObject<T>(string poolName)
        {
            Pool pool;
            if (dicTotalPools.TryGetValue(poolName, out pool))
            {
                return pool.GetPreSpawnObject<T>(this.transform);

            }
            else
            {
                Debug.LogError("Error: " + poolName + "is not contain pools");
                return default(T);
            }


        }

        public T GetPreSpawnObject<T>(string poolName, Transform spawnTransformInfo)
        {
            Pool pool;
            if (dicTotalPools.TryGetValue(poolName, out pool))
            {
                return pool.GetPreSpawnObject<T>(this.transform, spawnTransformInfo: spawnTransformInfo);

            }
            else
            {
                Debug.LogError("Error: " + poolName + "is not contain pools");
                return default(T);
            }


        }

        public T GetPreSpawnObject<T>(string poolName, Vector3 spawnPosition, Quaternion spawnRotation)
        {
            Pool pool;
            if (dicTotalPools.TryGetValue(poolName, out pool))
            {
                return pool.GetPreSpawnObject<T>(this.transform, spawnPosition: spawnPosition, spawnRotation: spawnRotation);

            }
            else
            {
                Debug.LogError("Error: " + poolName + "is not contain pools");
                return default(T);
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


		

	}
}


