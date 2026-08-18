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
        public static async void LoadSceneAsync(SceneReference scene, Action onComplete)
        {
            await SceneManager.LoadSceneAsync(scene);
            await Awaitable.NextFrameAsync();   // Pause d'une frame pour laisser les vues appeler leur Awake
            await Awaitable.MainThreadAsync();

            onComplete?.Invoke();
        }

    }
}