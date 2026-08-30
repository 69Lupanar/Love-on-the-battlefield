using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Scenes
{
    //Charge et décharge les scènes au besoin
    public static class SceneLoader
    {
        /// <summary>
        /// Charge la scène demandée en asynchrone
        /// </summary>
        /// <param name="scene">Scène à charger</param>
        /// <param name="loadSceneMode">Mode de chargement</param>
        /// <param name="onComplete">Callback appelé en fin de chargement</param>
        public static async void LoadSceneAsync(SceneReference scene, LoadSceneMode loadSceneMode = LoadSceneMode.Single, Action onComplete = null)
        {
            await SceneManager.LoadSceneAsync(scene, loadSceneMode);
            await Awaitable.NextFrameAsync();   // Pause d'une frame pour laisser les vues appeler leur Awake
            await Awaitable.MainThreadAsync();

            onComplete?.Invoke();
        }

        /// <summary>
        /// Décharge la scène demandée en asynchrone
        /// </summary>
        /// <param name="scene">Scène à charger</param>
        /// <param name="onComplete">Callback appelé en fin de chargement</param>
        public static async void UnloadSceneAsync(SceneReference scene, Action onComplete = null)
        {
            await SceneManager.UnloadSceneAsync(scene, UnloadSceneOptions.None);
            await Awaitable.NextFrameAsync();   // Pause d'une frame pour laisser les vues appeler leur Awake
            await Awaitable.MainThreadAsync();

            onComplete?.Invoke();
        }

    }
}