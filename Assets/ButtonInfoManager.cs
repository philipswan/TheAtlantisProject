using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonInfoManager : MonoBehaviour {

	[Tooltip("Name of the gameobject this info is associated with")]
	public string ButtonObjectName;

	[Header("Text to be displayed")]
	public string MovementAction;
	public string MenuAction;

	private LineRenderer lr;
	private TextMesh tm;
	Transform button;

	// Use this for initialization
	void Start () {
		lr = GetComponent<LineRenderer>();
		tm = GetComponent<TextMesh>();

		tm.text = "";
		//lr.numCornerVertices = 10;
	}
	
	// Update is called once per frame
	void Update () {
		if (button != null)
		{
			if (tm.text != "")
			{
				if (!lr.enabled)
				{
					lr.enabled = true;
				}

				Vector3 pos1, pos2, pos3;
				if (name == "LHand Trigger")
				{
					pos1 = button.position - button.transform.right * 0.00001f;
					pos2 =  button.position - button.transform.right * 0.05f;
				}
				else if (name == "LIndex Trigger")
				{
					pos1 = button.position - button.transform.up * 0.00001f;
					pos2 =  button.position - button.transform.up * 0.05f;
				}
				else if (name == "RHand Trigger")
				{
					pos1 = button.position + button.transform.right * 0.00001f;
					pos2 =  button.position + button.transform.right * 0.05f;
				}
				else if (name == "RIndex Trigger")
				{
					pos1 = button.position + button.transform.up * 0.00001f;
					pos2 =  button.position + button.transform.up * 0.05f;
				}
				else
				{
					pos1 = button.position + button.transform.forward * 0.00001f;
					pos2 =  button.position + button.transform.forward * 0.05f;
				}

				pos3 = transform.position;
				Vector3[] linePos = new Vector3[] {pos1, pos2, pos3};

				lr.positionCount = linePos.Length;
				lr.SetPositions(linePos);
			}
			else 
			{
				lr.enabled = false;
			}
		}
	}

	/// <summary>
	/// Set the button transform reference after it's created
	/// </summary>
	public void OnCreate()
	{
		button = GameObject.Find(ButtonObjectName).transform;

		// If this is the last object to recieve the message, set all initial messages and deactive the objects
		if (transform.parent.GetChild(transform.parent.childCount-1).name == gameObject.name)
		{
			for (int i=0; i<transform.parent.childCount; i++)
			{
				transform.parent.GetChild(i).GetComponent<ButtonInfoManager>().SetInitialMessage();
			}
			transform.parent.GetComponent<HelpMenuManager>().DeactivateChildren();
		}
	}

	/// <summary>
	/// Toggle the message to be displayed
	/// </summary>
	/// <param name="_menuMessages">If set to <c>true</c> menu messages.</param>
	public void UpdateMessage(bool _menuMessages)
	{
		if (_menuMessages)
		{
			tm.text = MenuAction;
		}
		else
		{
			tm.text = MovementAction;
		}
	}

	/// <summary>
	/// Sets the initial message.
	/// </summary>
	public void SetInitialMessage()
	{
		tm.text = MovementAction;
	}
}
