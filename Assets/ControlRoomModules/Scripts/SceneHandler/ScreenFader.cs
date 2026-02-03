using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ControlRoom
{
    public class ScreenFader : SingletonBase<ScreenFader>
    {
        public enum FadeType
        {
            Black, Loading, GameOver,
        }


        public static bool IsFading
        {
            get { return Instance.m_IsFading; }
        }

        public CanvasGroup faderCanvasGroup;
        public CanvasGroup loadingCanvasGroup;
        public CanvasGroup gameOverCanvasGroup;
        public CanvasGroup roundFaderCanvasGroup;

#if FEED_BACK_USE
        public MoreMountains.Feedbacks.MMF_Player fadeInRound;
        public MoreMountains.Feedbacks.MMF_Player fadeOutRound;
#endif

        public float fadeDuration = 1f;

        protected bool m_IsFading;

        const int k_MaxSortingLayer = 32767;

        protected override void Awake()
        {
            base.Awake();

            if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
        }

        protected IEnumerator Fade(float finalAlpha, CanvasGroup canvasGroup)
        {
            m_IsFading = true;
            canvasGroup.blocksRaycasts = true;
            float fadeSpeed = Mathf.Abs(canvasGroup.alpha - finalAlpha) / fadeDuration;
            while (!Mathf.Approximately(canvasGroup.alpha, finalAlpha))
            {
                canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, finalAlpha,
                    fadeSpeed * Time.deltaTime);
                yield return null;
            }
            canvasGroup.alpha = finalAlpha;
            m_IsFading = false;
            canvasGroup.blocksRaycasts = false;
        }

        public static void SetAlpha(float alpha)
        {
            Instance.faderCanvasGroup.alpha = alpha;
        }

        public static IEnumerator FadeSceneIn()
        {
            CanvasGroup canvasGroup;
            if (Instance.faderCanvasGroup.alpha > 0.1f)
                canvasGroup = Instance.faderCanvasGroup;
            else if (Instance.gameOverCanvasGroup.alpha > 0.1f)
                canvasGroup = Instance.gameOverCanvasGroup;
            else
                canvasGroup = Instance.loadingCanvasGroup;

            yield return Instance.StartCoroutine(Instance.Fade(0f, canvasGroup));

            canvasGroup.gameObject.SetActive(false);
        }

        public static IEnumerator FadeSceneOut(FadeType fadeType = FadeType.Black)
        {
            CanvasGroup canvasGroup;
            switch (fadeType)
            {
                case FadeType.Black:
                    canvasGroup = Instance.faderCanvasGroup;
                    break;
                case FadeType.GameOver:
                    canvasGroup = Instance.gameOverCanvasGroup;
                    break;
                default:
                    canvasGroup = Instance.loadingCanvasGroup;
                    break;
            }

            canvasGroup.gameObject.SetActive(true);

            yield return Instance.StartCoroutine(Instance.Fade(1f, canvasGroup));
        }

     

        public static void FadeRound(bool fadeOut=false)
        {
#if FEED_BACK_USE
            var fade = Instance.fadeInRound;
            if(fadeOut)
            {
                fade = Instance.fadeOutRound;
            }

            if (fade != null)
            {
                var fades = fade.GetFeedbacksOfType<MoreMountains.Feedbacks.MMF_Fade>();

                if (fades.Count > 0)
                {
                    foreach (var f in fades)
                    {

                        f.TargetPosition = GameManager.Instance.ScreenCenterWorldPosition;
                    }
                }

            }

            fade.Events.OnComplete.RemoveAllListeners();
            fade.Events.OnComplete.AddListener(() =>
            {
                if(fadeOut)
                    Instance.roundFaderCanvasGroup.gameObject.SetActive(false);
            });

            Instance.roundFaderCanvasGroup.gameObject.SetActive(true);
            fade?.PlayFeedbacks();
#endif
        }

      
    }
}