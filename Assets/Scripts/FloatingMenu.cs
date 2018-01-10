using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingMenu : MonoBehaviour {

	public static FloatingMenu Instance;

	private List<string> Descriptions = new List<string>();				// Holds all descriptions
	private List<GameObject> menuObjects = new List<GameObject>();		// Holds gameobjects to display
	private TextMesh tm;												// Reference to text mesh
	private GameObject currentIcon;										// Current displayed menu object
	private bool waitToReset;											// Limit one cycle per thumbstick movement
	private bool activated;												// Is the menu active

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
	}
	
	// Update is called once per frame
	void Update () {
		// Wait for the user to reset the thumbstick position to prevent flying through the menu
		if (waitToReset)
		{
			if (OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick) == Vector2.zero && OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick) == Vector2.zero)
			{
				waitToReset = false;
			}
		}
		else if (activated)
		{
			if (OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).x > 0.5f || OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x > 0.5f)
			{
				// Cycle menu right
				CycleRight();
			}
			else if (OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).x < -0.5f || OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x < -0.5f)
			{
				// Cycle menu left
				CycleLeft();
			}
		}

		// Activate menu
		if ((OVRInput.GetDown(OVRInput.Button.One) || OVRInput.GetDown(OVRInput.Button.Two)) && !activated)
		{
			ActivateMenu();
			ControllerTransition.Instance.Activate(false);	// Since we need the thumbstick to navigate the menu
			return;
		}
		// Deactive menu
		if ((OVRInput.GetDown(OVRInput.Button.One) || OVRInput.GetDown(OVRInput.Button.Two))  && activated)
		{
			DeactivateMenu();
			ControllerTransition.Instance.Activate(true);	// Thumbstick for this script is no longer needed
			return;
		}
	}

	/// <summary>
	/// Add new item to the list and instantiate it
	/// </summary>
	/// <param name="_go">Go.</param>
	/// <param name="_desc">Desc.</param>
	/// <param name="_scale">Scale.</param>
	/// <param name="_distance">Distance.</param>
	public void AddItems(GameObject _go, string _desc, Vector3 _scale, float _distance = -1.0f)
	{
		Descriptions.Add(_desc);

		// Deactive all scripts attached to this object
		MonoBehaviour[] scripts = _go.GetComponents<MonoBehaviour>();
		for (int i=0; i<scripts.Length; i++)
		{
			scripts[i].enabled = false;
		}

		InstantiateObjects(_go, _scale, _distance);
	}

	/// <summary>
	/// Instantiate the object and add it to the list
	/// </summary>
	/// <param name="_go">Go.</param>
	/// <param name="_scale">Scale.</param>
	/// <param name="_distance">Distance.</param>
	private void InstantiateObjects(GameObject _go, Vector3 _scale, float _distance = -1.0f)
	{
		// Ignore duplicates
		foreach (GameObject g in menuObjects)
		{
			if (g.tag == _go.tag)
			{
				return;
			}
		}

		// If it is a ring, create a container to center it
		GameObject go;
		if (_distance != -1.0f)
		{
			go = CreateContianer(_go, _go.tag, _distance);
		}
		else
		{
			go = Instantiate(_go, transform.GetChild(0).position, Quaternion.identity, transform);
		}

		go.SetActive(false);
		go.transform.localScale = _scale;

		// Set child rotations to zero
		for (int i=0; i<go.transform.childCount; i++)
		{
			go.transform.GetChild(i).localRotation = Quaternion.identity;
		}
		menuObjects.Add(go);
	}

	/// <summary>
	/// Activates the menu.
	/// </summary>
	private void ActivateMenu()
	{
		activated = true;

		gameObject.SetActive(true);
		currentIcon = menuObjects[0];
		menuObjects[0].SetActive(true);
		tm.text = Descriptions[0];

		int index = menuObjects.IndexOf(currentIcon);

		string currentTag = currentIcon.tag;
		UpdateMaterials(currentTag);

		StartCoroutine("RotateItem", index);

		// Update the messages to be displayed in the help menu
		GameObject[] helpMenus = GameObject.FindGameObjectsWithTag("Help Menu");
		foreach(GameObject go in helpMenus)
		{
			go.GetComponent<HelpMenuManager>().UpdateMessages(true);
		}
	}

	/// <summary>
	/// Deactivates the menu.
	/// </summary>
	private void DeactivateMenu()
	{
		activated = false;

		int index = menuObjects.IndexOf(currentIcon);

		string currentTag = currentIcon.tag;
		UpdateMaterials(currentTag);
		menuObjects[index].SetActive(false);
		tm.text = "";

		StopCoroutine("RotateItem");

		// Update the messages to be displayed in the help menu
		GameObject[] helpMenus = GameObject.FindGameObjectsWithTag("Help Menu");
		foreach(GameObject go in helpMenus)
		{
			go.GetComponent<HelpMenuManager>().UpdateMessages(false);
		}
	}

	/// <summary>
	/// Cycle meu objects by moving the thumbstick right
	/// </summary>
	private void CycleRight()
	{
		waitToReset = true;

		string prevTag, currentTag;
		prevTag = currentIcon.tag;

		int index = menuObjects.IndexOf(currentIcon);
		menuObjects[index].SetActive(false);

		if (index < menuObjects.Count - 1)
		{
			currentIcon = menuObjects[index+1];
			menuObjects[index+1].SetActive(true);
			tm.text = Descriptions[index+1];
		}
		else
		{
			currentIcon = menuObjects[0];
			menuObjects[0].SetActive(true);
			tm.text = Descriptions[0];
		}

		currentTag =  currentIcon.tag;
		UpdateMaterials(currentTag, prevTag);

		index = menuObjects.IndexOf(currentIcon);
		StopCoroutine("RotateItem");
		StartCoroutine("RotateItem", index);
	}

	/// <summary>
	/// Cycle menu objects by moving the thumbstick left
	/// </summary>
	private void CycleLeft()
	{
		waitToReset = true;

		string prevTag, currentTag;
		prevTag = currentIcon.tag;

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

		currentTag =  currentIcon.tag;
		UpdateMaterials(currentTag, prevTag);
		index = menuObjects.IndexOf(currentIcon);
		StopCoroutine("RotateItem");
		StartCoroutine("RotateItem", index);
	}

	/// <summary>
	/// Create container for the ring to center it
	/// </summary>
	/// <returns>The contianer.</returns>
	/// <param name="_icon">Icon.</param>
	/// <param name="_tag">Tag.</param>
	/// <param name="_distance">Distance.</param>
	private GameObject CreateContianer(GameObject _icon, string _tag, float _distance)
	{
		// Create container, set ring transform parent to container
		GameObject container = new GameObject("Container - " + _tag);
		container.transform.SetParent(transform);
		_icon.transform.SetParent(container.transform);

		// Center ring
		container.transform.localScale = Vector3.one;
		container.transform.position = transform.GetChild(0).position;
		_icon.transform.localScale = Vector3.one;
		_icon.transform.localPosition = Vector3.zero + _icon.transform.InverseTransformDirection(_icon.transform.right) * (_distance / 2);

		// Set tag
		container.tag = _tag;
		_icon.tag = "Untagged";

		return container;
	}

	/// <summary>
	/// Set materials of selected objects
	/// </summary>
	/// <param name="_tag">Tag.</param>
	private void UpdateMaterials(string _currentTag, string _prevTag = null)
	{
		GameObject[] currentObjects = GameObject.FindGameObjectsWithTag(_currentTag);
		foreach (GameObject go in currentObjects)
		{
			// Ignore the menu object
			if (menuObjects.Contains(go))
			{
				continue;
			}
			else
			{
				go.SendMessage("SetMaterials");
			}
		}

		if (_prevTag != null)
		{
			GameObject[] prevObjects = GameObject.FindGameObjectsWithTag(_prevTag);
			foreach (GameObject go in prevObjects)
			{
				// Ignore the menu object
				if (menuObjects.Contains(go))
				{
					continue;
				}
				else
				{
					go.SendMessage("SetMaterials");
				}
			}
		}
	}

	/// <summary>
	/// Rotate the menu object
	/// </summary>
	/// <returns>The item.</returns>
	/// <param name="index">Index.</param>
	private IEnumerator RotateItem(int index)
	{
		while (true)
		{
			menuObjects[index].transform.Rotate(Vector3.up * Time.deltaTime * 80);
			yield return null;
		}
	}
}
