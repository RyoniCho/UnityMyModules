using UnityEngine;

namespace ControlRoom.UI
{
    /// <summary>
    /// 리스트 아이템의 기본 클래스입니다.
    /// 모든 리스트 아이템은 이 클래스를 상속받아 구현해야 합니다.
    /// </summary>
    /// <typeparam name="T">아이템에 설정할 데이터 타입</typeparam>
    public abstract class BaseUIListItem<T> : MonoBehaviour
    {
        protected T data;
        protected int index;

        /// <summary>
        /// 아이템 데이터를 설정하고 UI를 갱신합니다.
        /// </summary>
        /// <param name="data">설정할 데이터</param>
        /// <param name="index">리스트 내 인덱스</param>
        public virtual void Setup(T data, int index)
        {
            this.data = data;
            this.index = index;
            RefreshUI();
        }

        /// <summary>
        /// 데이터에 기반하여 UI 요소를 갱신합니다.
        /// </summary>
        protected abstract void RefreshUI();
    }
}
