using System.Collections;
using System.Collections.Generic;
//using UnityEngine.EventSystems;
using UnityEngine;

public class CarHorn : MonoBehaviour
{
	[Header("The Sound Of The Horn")]
	[SerializeField]
	private AudioSource hornSound;

	[Header("Key For Sound")]
	[SerializeField]
	private KeyCode keyCode;

	[Header("The Minimum Duration Of The Sound")]
	[SerializeField]
	private float minHornDuration = 0.05f;

	[Header("The Scale Of The Horn When Being Animated")]
	[SerializeField]
	private Vector2 hornAnimationScale = new Vector2(0.9f, 0.9f);

	[Header("The speed of the horn animation")]
	[SerializeField]
	private float animationSpeed = 10f;

	private Transform horn;

	private Vector3 initialHornScale;
	private Coroutine animateHornCoroutine;


	private void Update()
    {
        if(Input.GetKeyDown(keyCode))
        {
			HornPressed();
        }
		if(Input.GetKeyUp(keyCode))
        {
			HornReleased();
		}
    }


	
    private bool coroutineRunning;

	public void HornPressed()
	{
		if (!hornSound.isPlaying)
		{
			StartHorn();

			StartCoroutine(CheckIfSoundFinished());
		}
	}

	public void HornReleased()
	{
		if (hornSound.isPlaying && !coroutineRunning)
			StartCoroutine(StopHornDelayed());
	}


	#region Start Horn
	private void StartHorn()
	{
		hornSound.Play();

		PlayHornAnimation();
	}
	#endregion

	#region Stop Horn
	private void StopHorn()
	{
		if (hornSound.isPlaying)
			hornSound.Stop();

		StopHornAnimation();
	}

	private IEnumerator StopHornDelayed()
	{
		coroutineRunning = true;

		yield return new WaitForSeconds(minHornDuration);
		StopHorn();

		coroutineRunning = false;
	}
	#endregion

	#region Animations

	private void Start()
	{
		//Get the horn transform
		horn = transform;

		//Get the initial scale
		initialHornScale = horn.localScale;
	}

	private void PlayHornAnimation()
	{
		if (animateHornCoroutine != null)
			StopCoroutine(animateHornCoroutine);

		animateHornCoroutine = StartCoroutine(AnimateHorn(hornAnimationScale));
	}

	private IEnumerator AnimateHorn(Vector3 newScale)
	{
		float percentage = 0f;
		Vector3 oldScale = horn.localScale;
		newScale.z = 1;

		while (horn.localScale != newScale)
		{
			percentage += Time.deltaTime * animationSpeed;

			horn.localScale = Vector3.Lerp(oldScale, newScale, percentage);

			yield return null;
		}
	}

	private void StopHornAnimation()
	{
		if (animateHornCoroutine != null)
			StopCoroutine(animateHornCoroutine);

		animateHornCoroutine = StartCoroutine(AnimateHorn(initialHornScale));
	}
	#endregion

	#region Check If Sound Finished
	private IEnumerator CheckIfSoundFinished()
	{
		yield return new WaitForSeconds(0.1f);

		if (horn.localScale != initialHornScale)
		{
			if (!hornSound.isPlaying)
				StopHornAnimation();

			else
				StartCoroutine(CheckIfSoundFinished());
		}
	}
	#endregion
}