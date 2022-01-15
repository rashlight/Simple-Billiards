using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RestartPanel : MonoBehaviour
{
	public float timeLimit = 3f;
	
	[SerializeField]
	private Text restartHelpText;
	[SerializeField]
	private Text restartActionText;
	private List<GameObject> hiddenObject;
	private float timeCounter = 0f;
 	private bool firstRun = true;

	// Start is called before the first frame update
 	void Start()
	{
		  
 	}

	// Update is called once per frame
	void Update()
  	{
    	if (Input.GetMouseButtonDown(0) && firstRun)
    	{
    	    firstRun = false;
			restartHelpText.gameObject.SetActive(false);
    	}
	
		if (Input.GetKey(KeyCode.R))
		{
			timeCounter += Time.deltaTime;
		}
		else
		{
			timeCounter -= Time.deltaTime;
		}

		timeCounter = Mathf.Clamp(timeCounter, 0f, timeLimit);
		if (timeCounter == timeLimit) 
		{
			restartActionText.text = "RESTARTING...";
			GlobalProc.GetGlobalVar().ReloadScene();
		}

		restartActionText.color = new Color(
			restartActionText.color.r,
			restartActionText.color.g,
			restartActionText.color.b,
			timeCounter / timeLimit
		);
	}
}
