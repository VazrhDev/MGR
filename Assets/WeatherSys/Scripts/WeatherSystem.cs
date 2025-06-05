using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MGR
{
    [System.Serializable]
    public struct WeatherData
    {
        public string name;
        public ParticleSystem ParticleSystem;
        [HideInInspector]
        public ParticleSystem.EmissionModule emission;

    }

    public enum WeatherState { Default, fog, rain, snow, DesertSandstorm, fogAndRain }

    public class WeatherSystem : MonoBehaviour
    {

        [SerializeField] WeatherState weatherState;
        public WeatherData[] weatherData;


        private void Awake()
        {
            loadWeatherSystem();

        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                weatherState = WeatherState.rain;
            }
            if (Input.GetKeyDown(KeyCode.G))
            {
                weatherState = WeatherState.snow;
            }
            if (Input.GetKeyDown(KeyCode.B))
            {
                weatherState = WeatherState.Default;
            }
            if (Input.GetKeyDown(KeyCode.H))
            {
                weatherState = WeatherState.fog;
            }
            if (Input.GetKeyDown(KeyCode.Y))
            {
                weatherState = WeatherState.DesertSandstorm;
            }
            setWeather(weatherState);
        }

        void loadWeatherSystem()
        {
            for (int i = 0; i < weatherData.Length; i++)
            {
                weatherData[i].emission = weatherData[i].ParticleSystem.emission;
            }
        }


        public void setWeather(WeatherState weatherState)
        {
            resetWeather();
            switch (weatherState)
            {
                case WeatherState.Default:
                    resetWeather();
                    break;
                case WeatherState.rain:
                    ActivateWeatherName("rain");
                    break;
                case WeatherState.snow:
                    ActivateWeatherName("snow");
                    break;
                case WeatherState.fog:
                    ActivateWeatherName("fog");
                    break;
                case WeatherState.DesertSandstorm:
                    ActivateWeatherName("DesertSandstorm");
                    Debug.Log("DesertSnadstorm");
                    break;
                case WeatherState.fogAndRain:
                    ActivateWeatherName("fog");
                    ActivateWeatherName("rain");
                    break;
                default:
                    break;
            }

        }


        void ActivateWeatherName(string name)
        {
            for (int i = 0; i < weatherData.Length; i++)
            {
                if (weatherData[i].name == name)
                {
                    ParticleSystem[] childrenParticles;
                    childrenParticles = weatherData[i].ParticleSystem.gameObject.GetComponentsInChildren<ParticleSystem>();
                    if (childrenParticles != null)
                    {
                        foreach (ParticleSystem system in childrenParticles)
                        {
                            Debug.Log("Child  Particles");
                            ParticleSystem.EmissionModule em = system.emission;
                            em.enabled = true;
                        }
                    }
                    weatherData[i].emission.enabled = true;
                }
            }
        }
        void resetWeather()
        {
            for (int i = 0; i < weatherData.Length; i++)
            {
                if (weatherData[i].emission.enabled != false)
                {
                    ParticleSystem[] childrenParticles;
                    childrenParticles = weatherData[i].ParticleSystem.gameObject.GetComponentsInChildren<ParticleSystem>();
                    if (childrenParticles != null)
                    {
                        foreach (ParticleSystem system in childrenParticles)
                        {
                            ParticleSystem.EmissionModule em = system.emission;
                            em.enabled = false;
                        }
                    }
                    weatherData[i].emission.enabled = false;
                }
            }
        }
    }
}
