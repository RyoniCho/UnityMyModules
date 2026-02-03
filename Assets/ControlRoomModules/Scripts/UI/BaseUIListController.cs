using System.Collections.Generic;
using UnityEngine;

namespace ControlRoom.UI
{
    /// <summary>
    /// 리스트 형태의 UI를 관리하는 컨트롤러의 기본 클래스입니다.
    /// 스크롤 뷰나 그리드 형태의 아이템 목록을 관리할 때 사용합니다.
    /// </summary>
    /// <typeparam name="TItem">리스트 아이템 컴포넌트 타입</typeparam>
    /// <typeparam name="TData">아이템 데이터 타입</typeparam>
    public abstract class BaseUIListController<TItem, TData> : MonoBehaviour where TItem : BaseUIListItem<TData>
    {
        [Header("List Settings")]
        [SerializeField] protected Transform itemRoot; // 아이템들이 생성될 부모 Transform
        [SerializeField] protected string itemPrefabName; // PoolManager에서 사용할 프리팹 이름

        protected List<TItem> activeItems = new List<TItem>();

        /// <summary>
        /// 데이터 리스트를 받아 UI 리스트를 갱신합니다.
        /// </summary>
        /// <param name="dataList">표시할 데이터 목록</param>
        public virtual void SetListItems(List<TData> dataList)
        {
            // 1. 기존 아이템 정리 (풀로 반환)
            ClearList();

            if (dataList == null) return;

            // 2. 데이터 수만큼 아이템 생성 및 설정
            for (int i = 0; i < dataList.Count; i++)
            {
                var item = SpawnItem();
                if (item != null)
                {
                    item.Setup(dataList[i], i);
                    activeItems.Add(item);
                }
            }
        }

        protected virtual TItem SpawnItem()
        {
            // PoolManager를 사용하여 아이템 생성
            // ControlRoom.PoolManager가 있다고 가정
            var go = ControlRoom.PoolManager.Instance.SpawnObject(itemPrefabName);
            if (go == null)
            {
                Debug.LogError($"Failed to spawn item: {itemPrefabName}");
                return null;
            }

            go.transform.SetParent(itemRoot, false);
            go.transform.localScale = Vector3.one;
            
            var itemComponent = go.GetComponent<TItem>();
            if (itemComponent == null)
            {
                Debug.LogError($"Prefab {itemPrefabName} does not have component {typeof(TItem).Name}");
            }

            return itemComponent;
        }

        protected virtual void ClearList()
        {
            foreach (var item in activeItems)
            {
                if (item != null && item.gameObject != null)
                {
                    ControlRoom.PoolManager.Instance.DespawnObject(itemPrefabName, item.gameObject);
                }
            }
            activeItems.Clear();
        }
    }
}
