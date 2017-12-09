using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingMenu : MonoBehaviour {

	public static FloatingMenu Instance;
	public List<GameObject> MenuIcons = new List<GameObject>();
	public List<string> Descriptions = new List<string>();

	private List<GameObject> menuObjects = new List<GameObject>();
	private TextMesh tm;
	private GameObject currentIcon;
	private bool waitToReset;
	private bool activated;

	void Awake()
	{
		Instance = this;
	}

	// Use this for initialization
	void Start () {
		tm = GetComponentInChildren<TextMesh>();
		tm.text = "";
		waitToReset = false;
		activated = false;

		InstantiateObjects();
	}
	
	// Update is called once per frame
	void Update () {
		if (waitToReset)
		{
			if (OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick) == Vector2.zero)
			{
				waitToReset = false;
			}
		}
		else if (activated)
		{
			if (OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).x > 0.5)
			{
				CycleRight();
			}
			else if (OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).x < -0.5)
			{
				CycleLeft();
			}
		}

		if (OVRInput.GetDown(OVRInput.Button.One) && !activated)
		{
			ActivateMenu();
			return;
		}
		if (OVRInput.GetDown(OVRInput.Button.One) && activated)
		{
			DeactivateMenu();
			return;
		}
	}

	public void AddItems(GameObject icon, string desc)
	{
		MenuIcons.Add(icon);
		Descriptions.Add(desc);
		InstantiateObjects(icon);
	}

	private void InstantiateObjects(GameObject _go = null)
	{
		print("instantiated");
		if (_go == null)
		{
			for (int i=0; i<MenuIcons.Count; i++)
			{
				GameObject go = Instantiate(MenuIcons[i], transform.GetChild(0).position, Quaternion.identity, transform);
				go.SetActive(false);
				go.transform.localScale = new Vector3(100 ,100, 100);
				menuObjects.Add(go);
			}
		}
		else
		{
			GameObject go = Instantiate(_go, transform.GetChild(0).position, Quaternion.identity, transform);
			go.SetActive(false);
			go.transform.localScale = new Vector3(100 ,100, 100);
			menuObjects.Add(go);
		}
	}

	private void ActivateMenu()
	{
		activated = true;
		waitToReset = true;

		gameObject.SetActive(true);
		currentIcon = menuObjects[0];
		menuObjects[0].SetActive(true);
		tm.text = Descriptions[0];

		int index = menuObjects.IndexOf(currentIcon);

		StartCoroutine("RotateItem", index);
	}

	private void DeactivateMenu()
	{
		activated = false;
		waitToReset = true;

		int index = menuObjects.IndexOf(currentIcon);

		menuObjects[index].SetActive(false);
		tm.text = "";

		StopCoroutine("RotateItem");
	}

	private void CycleRight()
	{
		print("right");
		waitToReset = true;

		int index = menuObjects.IndexOf(currentIcon);
		menuObjects[index].SetActive(false);

		if (index < menuObjects.Count - 1)
		{
			print("primary");
			currentIcon = menuObjects[index+1];
			menuObjects[index+1].SetActive(true);
			//tm.text = Descriptions[index+1];
		}
		else
		{
			print("secondary");
			currentIcon = menuObjects[0];
			menuObjects[0].SetActive(true);
			tm.text = Descriptions[0];
		}
	}

	private void CycleLeft()
	{
		print("left");
		waitToReset = true;

		int index = menuObjects.IndexOf(currentIcon);
		menuObjects[index].SetActive(false);

		if (index > 0)
		{
			currentIcon = menuObjects[index-1];
			menuObjects[index-1].SetActive(true);
			tm.text = Descriptions[index-1];
		}
		else
		{
			currentIcon = menuObjects[menuObjects.Count-1];
			menuObjects[menuObjects.Count-1].SetActive(true);
			tm.text = Descriptions[Descriptions.Count-1];
		}
	}

	private IEnumerator RotateItem(int index)
	{
		while (true)
		{
			menuObjects[index].transform.Rotate(Vector3.up * Time.deltaTime * 80);
			yield return null;
		}
	}
}
