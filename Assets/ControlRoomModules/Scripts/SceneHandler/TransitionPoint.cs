using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ControlRoom
{
    public class TransitionPoint : MonoBehaviour
    {


        float elapsTime = 0;

        public enum TransitionType
        {
            DifferentScene,
            DifferentNonGameScene,
            SameScene,
            LoadSceneFromFile,
        }

        public enum TransitionWhen
        {
            ExternalCall, OnTriggerEnter,
        }

       
        [Tooltip("This is the gameobject that will transition.  For example, the player.")]
        public GameObject transitioningGameObject;
        // [Tooltip("If true, Compare gameobject tag that will transition. not equality of gameobject")]
        // public bool CompareGameObjectTag = true;
        public bool CompareLayerMask = true;
        public LayerMask checkLayer;
        [Tooltip("Whether the transition will be within this scene, to a different zone or a non-gameplay scene.")]
        public TransitionType transitionType;

        public string newSceneName;
        [Tooltip("The tag of the SceneTransitionDestination script in the scene being transitioned to.")]
        public SceneTransitionDestination.DestinationTag transitionDestinationTag;
      
        [Tooltip("What should trigger the transition to start.")]
        public TransitionWhen transitionWhen;
       
        

        public Transform detinationFromLoadFile;

       
        void OnTriggerEnter(Collider other)
        {
           
            var triggerOn = (CompareLayerMask) ? (Utils.CheckLayerMask(checkLayer,other.gameObject.layer)) : (other.gameObject == transitioningGameObject);

            if (triggerOn)
            {
               
                if (ScreenFader.IsFading || SceneController.Transitioning)
                    return;

                if (transitionWhen == TransitionWhen.OnTriggerEnter)
                    TransitionInternal();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
           
            var triggerOn = (CompareLayerMask) ? (Utils.CheckLayerMask(checkLayer,other.gameObject.layer)) : (other.gameObject == transitioningGameObject);
            
            if (triggerOn)
            {
               

                if (ScreenFader.IsFading || SceneController.Transitioning)
                    return;

                if (transitionWhen == TransitionWhen.OnTriggerEnter)
                    TransitionInternal();
            }
        }


        protected void TransitionInternal()
        {
            

            
            if (transitionType == TransitionType.SameScene)
            {
                var destination = SceneController.Instance.GetDestination(transitionDestinationTag);

                if(destination!=null)
                {
                    transitioningGameObject.transform.position = destination.transform.position;
                    transitioningGameObject.transform.rotation = destination.transform.rotation;
                }

            }

            else
            {
                SceneController.TransitionToScene(this);
            }
        }

        public void Transition()
        {
            if (transitionWhen == TransitionWhen.ExternalCall)
                TransitionInternal();
        }
        
        public void TransitionByLoadFile()
        {
            TransitionInternal();
        }

    }
}

