// Copyright 2017 Google Inc. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using Firebase;
////using Firebase.Unity.Editor;
using UnityEngine.SocialPlatforms;
using System.Threading.Tasks;
using Firebase.RemoteConfig;

namespace Hamster
{

    public class MainGame : MonoBehaviour
    {

        [HideInInspector]
        public States.StateManager stateManager = new States.StateManager();
        private float currentFrameTime, lastFrameTime;

        private const string PlayerPrefabID = "Player";

        // The active player object in the game.
        [HideInInspector]
        public GameObject player;

        // The player responsible for game music.
        private AudioSource musicPlayer;

        // The PlayerController component on the active player object.
        public PlayerController PlayerController
        {
            get
            {
                return player != null ? player.GetComponent<PlayerController>() : null;
            }
        }

        public UnityEvent PlayerSpawnedEvent = new UnityEvent();

        // Volume is treated as an int for the UI display.
        public const int MaxVolumeValue = 6;
        private int musicVolume = 0;
        public int MusicVolume
        {
            get
            {
                return musicVolume;
            }
            set
            {
                musicVolume = value;
                PlayerPrefs.SetInt(StringConstants.MusicVolume, musicVolume);
                // Music volume is controlled on the music source, which is set to
                // ignore the volume settings of the listener.
                CommonData.mainCamera.GetComponentInChildren<AudioSource>().volume =
                  (float)musicVolume / MaxVolumeValue;
            }
        }
        private int soundFxVolume = 0;
        public int SoundFxVolume
        {
            get
            {
                return soundFxVolume;
            }
            set
            {
                soundFxVolume = value;
                PlayerPrefs.SetInt(StringConstants.SoundFxVolume, soundFxVolume);
                // Sound effect volumes are controlled by setting the listeners volume,
                // instead of each effect individually.
                AudioListener.volume = (float)soundFxVolume / MaxVolumeValue;
            }
        }

        private bool firebaseInitialized;

        IEnumerator Start()
        {
            Screen.SetResolution(Screen.width / 2, Screen.height / 2, true);
            GooglePlayServicesSignIn.InitializeGooglePlayGames();
            InitializeFirebaseAndStart();
            while (!firebaseInitialized)
            {
                yield return null;
            }
            StartGame();
        }

        void Update()
        {
            lastFrameTime = currentFrameTime;
            currentFrameTime = Time.realtimeSinceStartup;
            stateManager.Update();
        }

        void FixedUpdate()
        {
            stateManager.FixedUpdate();
        }

        // Play an audio clip as music.  If that clip is already playing,
        // we ignore it, and keep playing without restarting.
        public void PlayMusic(AudioClip music, bool shouldLoop)
        {
            if (musicPlayer.clip != music || !musicPlayer.isPlaying)
            {
                musicPlayer.Stop();
                musicPlayer.clip = music;
                musicPlayer.Play();
                musicPlayer.loop = shouldLoop;
            }
        }

        // Utility function for picking a random track to play from a selection.
        public void SelectAndPlayMusic(AudioClip[] musicArray, bool shouldLoop)
        {
            PlayMusic(musicArray[Random.Range(0, musicArray.Length)], shouldLoop);
        }

        // Utility function to check the time since the last update.
        // Needed, since we can't use Time.deltaTime, as we are adjusting the
        // simulation timestep.  (Setting it to 0 to pause the world.)
        public float TimeSinceLastUpdate
        {
            get { return currentFrameTime - lastFrameTime; }
        }

        // Utility function to check if the game is currently running.  (i.e.
        // not in edit mode.)
        public bool isGameRunning()
        {
            States.BaseState state = stateManager.CurrentState();
            return (state is States.Gameplay ||
              // While with LevelFinished the game is not technically running, we want
              // to mimic the traditional behavior in the background.
              state is States.LevelFinished);
        }

        // Utility function for spawning the player.
        public GameObject SpawnPlayer()
        {
            if (player == null)
            {
                player = (GameObject)Instantiate(CommonData.prefabs.lookup[PlayerPrefabID].prefab);
                PlayerSpawnedEvent.Invoke();
            }
            return player;
        }

        // Utility function for despawning the player.
        public void DestroyPlayer()
        {
            if (player != null)
            {
                Destroy(player);
                player = null;
            }
        }

        // Pass through to allow states to have their own GUI.
        void OnGUI()
        {
            stateManager.OnGUI();
        }

        // When the app starts, check to make sure that we have
        // the required dependencies to use Firebase, and if not,
        // add them if possible.
        void InitializeFirebaseAndStart()
        {
            DependencyStatus dependencyStatus = FirebaseApp.CheckDependenciesAsync().Result;

            if (dependencyStatus != Firebase.DependencyStatus.Available)
            {
                Firebase.FirebaseApp.FixDependenciesAsync().ContinueWith(task =>
                {
                    dependencyStatus = Firebase.FirebaseApp.CheckDependenciesAsync().Result;
                    if (dependencyStatus == Firebase.DependencyStatus.Available)
                    {
                        InitializeFirebaseComponents();
                    }
                    else
                    {
                        Debug.LogError(
                            "Could not resolve all Firebase dependencies: " + dependencyStatus);
                        Application.Quit();
                    }
                });
            }
            else
            {
                InitializeFirebaseComponents();
            }
        }

        void InitializeFirebaseComponents()
        {
            Task.WhenAll(InitializeRemoteConfig()).ContinueWith(task => { firebaseInitialized = true; });
            Debug.Log("Firebase is now initialized");
            //firebaseInitialized = true;
        }

        Task InitializeRemoteConfig()
        {
            Dictionary<string, object> dictionaryDefaults = new Dictionary<string, object>();
            dictionaryDefaults.Add("gravity", -10.0);
            dictionaryDefaults.Add("force", 5.0);
            dictionaryDefaults.Add("radius", 3.0);
            FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(dictionaryDefaults);
            ConfigSettings configSettings = new ConfigSettings();
            configSettings.FetchTimeoutInMilliseconds = 0;
            FirebaseRemoteConfig.DefaultInstance.SetConfigSettingsAsync(configSettings);
            return FirebaseRemoteConfig.DefaultInstance.FetchAndActivateAsync();
        }


        // Actually start the game, once we've verified that everything
        // is working and we have the firebase prerequisites ready to go.
        void StartGame()
        {
            FirebaseApp app = FirebaseApp.DefaultInstance;

            CommonData.prefabs = FindObjectOfType<PrefabList>();
            CommonData.mainCamera = FindObjectOfType<CameraController>();
            CommonData.mainGame = this;
            AppOptions ops = new AppOptions();
            //Update the URL based on your project
            ops.DatabaseUrl = new System.Uri("https://gbt-firebase-default-rtdb.firebaseio.com");
            CommonData.app = FirebaseApp.Create(ops);
            Screen.orientation = ScreenOrientation.Landscape;

            musicPlayer = CommonData.mainCamera.GetComponentInChildren<AudioSource>();

            CommonData.gameWorld = FindObjectOfType<GameWorld>();

            // Set up volume settings.
            MusicVolume = PlayerPrefs.GetInt(StringConstants.MusicVolume, MaxVolumeValue);
            // Set the music to ignore the listeners volume, which is used for sound effects.
            CommonData.mainCamera.GetComponentInChildren<AudioSource>().ignoreListenerVolume = true;
            SoundFxVolume = PlayerPrefs.GetInt(StringConstants.SoundFxVolume, MaxVolumeValue);

            stateManager.PushState(new States.Startup());
        }
    }
}
