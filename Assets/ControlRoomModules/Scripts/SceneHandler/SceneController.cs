using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ControlRoom
{
    public class SceneController : SingletonBase<SceneController>
    {

        Scene m_CurrentScene;
        SceneTransitionDestination.DestinationTag m_ZoneRestartDestinationTag;
        bool m_Transitioning = false;

        public delegate void ProcessBeforeUnloadingCurrentScene();
        public delegate void ProcessAfterLoadingNextScene();
        public ProcessBeforeUnloadingCurrentScene processBeforeUnloadingCurrentScene;
        public ProcessAfterLoadingNextScene processAfterLoadingNextScene;


        protected override void Awake()
        {
            base.Awake();

            if (Instance != this)
            {
                Destroy(this.gameObject);
                return;
            }

            DontDestroyOnLoad(this.gameObject);
        }
        public static bool Transitioning
        {
            get { return Instance.m_Transitioning; }
        }

        public static void TransitionToScene(TransitionPoint transitionPoint)
        {
            Instance.StartCoroutine(Instance.Transition(transitionPoint.newSceneName, transitionPoint.transitionDestinationTag, transitionPoint.transitionType));
        }

        IEnumerator Transition(string newSceneName, SceneTransitionDestination.DestinationTag destinationTag, TransitionPoint.TransitionType transitionType = TransitionPoint.TransitionType.DifferentScene)
        {
            m_Transitioning = true;

            //Process Before Unload Current Scene :Save Current Data/Input Disable, etc
            processBeforeUnloadingCurrentScene?.Invoke();

            //Scene Fade Out
            yield return StartCoroutine(ScreenFader.FadeSceneOut(ScreenFader.FadeType.Loading));

         
            System.GC.Collect();

            //SceneLoad
            yield return SceneManager.LoadSceneAsync(newSceneName);


            //Process After Loading Next Scene : Load Current Data/Input Enable, etc
            processAfterLoadingNextScene?.Invoke();

            SceneTransitionDestination entrance = GetDestination(destinationTag);

            if (transitionType != TransitionPoint.TransitionType.LoadSceneFromFile)
            {
                SetEnteringGameObjectLocation(entrance);
            }
            else
            {
                //Transform tempTransform = GameManager.Instance.CreateHeroPositionFromFile();
                //SetEnteringGameObjectLocation(entrance, tempTransform);
            }

            SetupNewScene(transitionType, entrance);

            if (entrance != null)
                entrance.OnReachDestination?.Invoke();
            yield return StartCoroutine(ScreenFader.FadeSceneIn());
          

            m_Transitioning = false;
        }


        SceneTransitionDestination GetDestination(SceneTransitionDestination.DestinationTag destinationTag)
        {

            //Find Destination in Scene
            SceneTransitionDestination[] entrances = FindObjectsOfType<SceneTransitionDestination>();
            for (int i = 0; i < entrances.Length; i++)
            {
                if (entrances[i].destinationTag == destinationTag)
                    return entrances[i];
            }
            Debug.LogWarning("No entrance was found with the " + destinationTag + " tag.");
            return null;
        }

        void SetupNewScene(TransitionPoint.TransitionType transitionType, SceneTransitionDestination entrance)
        {
            if (entrance == null)
            {
                Debug.LogWarning("Restart information has not been set.");
                return;
            }

            if (transitionType == TransitionPoint.TransitionType.DifferentScene || transitionType == TransitionPoint.TransitionType.LoadSceneFromFile)
                SetZoneStart(entrance);
        }

        void SetZoneStart(SceneTransitionDestination entrance)
        {
            m_CurrentScene = entrance.gameObject.scene;
            m_ZoneRestartDestinationTag = entrance.destinationTag;
        }

        void SetEnteringGameObjectLocation(SceneTransitionDestination entrance, Transform extraPosition = null)
        {
            if (entrance == null)
            {
                Debug.LogWarning("Entering Transform's location has not been set.");
                return;
            }

            Transform entranceLocation = (extraPosition == null) ? entrance.transform : extraPosition;
            Transform enteringTransform = entrance.transitioningGameObject.transform;
            enteringTransform.position = entranceLocation.position;
            enteringTransform.rotation = entranceLocation.rotation;
        }

        public string CurrentEntranceSceneName
        {
            get
            {
                return m_CurrentScene.name;
            }
        }

        public string CurrentActiveSceneName => SceneManager.GetActiveScene().name;

    }
}


