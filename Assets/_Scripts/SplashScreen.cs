using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class SplashScreen : MonoBehaviour
{
    [SerializeField] VideoPlayer MGRVideo;
    [SerializeField] VideoPlayer VarzhVideo;

    private void Start()
    {
        StartCoroutine(waitForSplash());
    }

    private void Update()
    {
    }

    IEnumerator waitForSplash()
    {
        Debug.LogError(VarzhVideo.isPrepared);
        while(VarzhVideo.isPrepared == true)
        {
            Debug.LogError("Has loaded");
            yield return new WaitForSeconds(0.01f);
        }
        Debug.LogError("Has loaded");
        VarzhVideo.Play();

        yield return new WaitForSeconds(6f);
        VarzhVideo.Stop();
        MGRVideo.Play();

        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("Introduction");
    }

}
