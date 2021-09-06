using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ExitKamazBort : MonoBehaviour {

	public GameObject carCamera;
	public GameObject player;
	public GameObject playerKamaz;
	public Transform getOutPosition;
	public GameObject Addons;
	public GameObject Brewno;
	public GameObject canvascar;
	public GameObject canvasRPG;
	public GameObject playerCamera;
	public float HP = 100;
	public float DT = 500;
	public Text HPKamazNov;
	public Text DTKamazNov;
	public AudioClip DTKamaz;
	public AudioClip BrewnoClip;

	public bool  opened = false;
	private float waitTime = 1f;
	public bool  temp = false;
	public bool addons;
	
	AudioSource sourse;
	
	void Start (){
    
		carCamera = carCamera;/* GameObject.FindObjectOfType<RCCCarCamera>().gameObject; */
		carCamera.GetComponent<Camera>().enabled = false;
		carCamera.GetComponent<AudioListener>().enabled = false;
        canvascar.GetComponent<Canvas>().enabled = false;
		/* carCamera.SetActive(false); */
		playerKamaz.SetActive(false);
		GetComponent<RCCCarControllerV2>().canControl = false;
		sourse = GetComponent<AudioSource>();

		if(!getOutPosition){
			GameObject getOutPos = new GameObject("Get Out Position");
			getOutPos.transform.SetParent(transform);
			getOutPos.transform.localPosition = new Vector3(-3f, 0f, 0f);
			getOutPos.transform.localRotation = Quaternion.identity;
			getOutPosition = getOutPos.transform;
		}

		if(GetComponent<RCCCarCameraConfig>())
			GetComponent<RCCCarCameraConfig>().enabled = false;

	}
	
	
	void Update (){

		if((Input.GetKeyDown(KeyCode.E)) && opened && !temp){
			GetOut();
			opened = false;
			temp = false;
			carCamera.SetActive(false);
		}
		if (opened && !temp)
		{
			DT -=Time.deltaTime;
		}
			if (DT<=0)
			{
				/* GetComponent<RCCCarControllerV2>().enabled = false; */
				carCamera.GetComponent<AudioListener>().enabled = false;
				GetComponent<RCCCarControllerV2>().canControl = false;
				DT = 0;
			}
			HPKamazNov.text="HP : "+HP;
		    DTKamazNov.text="DT : "+DT;
			if((Input.GetKeyDown(KeyCode.P)))
			{
			Addons.SetActive(false);
			addons = true;
			}
			if((Input.GetKeyDown(KeyCode.O)))
			{
			Addons.SetActive(true);
			addons = false;
			}
			
	}
	
	IEnumerator Act (GameObject player){

		player = player;

		if (!opened && !temp){
			GetIn();
			opened = true;
			temp = true;
			yield return new WaitForSeconds(waitTime);
			temp = false;
		}

	}
	
	void OnTriggerEnter (Collider other)
	{ 
		if(other.tag == "BenzinTank")
		    { 
				{
					if(DT<500)
					DT = DT + (500-DT);
				    sourse.PlayOneShot(DTKamaz);
				}
		      
			}
			if(other.tag == "KamazBort")
		    { 
				
				Brewno.SetActive(true);
				sourse.PlayOneShot(BrewnoClip);
				
		      
			}
			if(other.tag == "Finish")
		    { 
				
				Brewno.SetActive(false);
				sourse.PlayOneShot(BrewnoClip);
				
		      
			}
	}
			
	void GetIn (){

		carCamera.transform.GetComponent<RCCCarCamera>().playerCar = transform;
		player.transform.SetParent(transform);
		player.transform.localPosition = Vector3.zero;
		player.transform.localRotation = Quaternion.identity;
		player.SetActive(false);
		playerKamaz.SetActive(true);
		playerCamera.SetActive(false);
		carCamera.SetActive(true);
		canvascar.GetComponent<Canvas>().enabled = true;
		canvasRPG.GetComponent<Canvas>().enabled = false;
		carCamera.GetComponent<Camera>().enabled = true;
		if(GetComponent<RCCCarCameraConfig>())
			GetComponent<RCCCarCameraConfig>().enabled = true;
		GetComponent<RCCCarControllerV2>().canControl = true;
		carCamera.GetComponent<AudioListener>().enabled = true;
		SendMessage("KillOrStartEngine");

	}
	
	void GetOut (){

		player.transform.SetParent(null);
		player.transform.position = getOutPosition.position;
		player.transform.rotation = getOutPosition.rotation;
		player.SetActive(true);
		playerKamaz.SetActive(false);
		playerCamera.SetActive(true);
		carCamera.SetActive(false);
		canvascar.GetComponent<Canvas>().enabled = false;
		canvasRPG.GetComponent<Canvas>().enabled = true;
		carCamera.GetComponent<Camera>().enabled = false;
		if(GetComponent<RCCCarCameraConfig>())
			GetComponent<RCCCarCameraConfig>().enabled = false;
		carCamera.GetComponent<AudioListener>().enabled = false;
		GetComponent<RCCCarControllerV2>().canControl = false;
		SendMessage("KillOrStartEngine");

	}
	
}