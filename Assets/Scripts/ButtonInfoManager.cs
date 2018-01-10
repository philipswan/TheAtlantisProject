using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonInfoManager : MonoBehaviour {

	public enum Button {
		X, Y, LHand, LIndex, LThumbstick, A, B, RHand, RIndex, RThumbstick
	}

	[Tooltip("Select the controller button this is tied to")]
	public Button AssociatedButton;

	private string movementAction;
	private string menuAction;

	private LineRenderer lr;
	private TextMesh tm;
	private string buttonObjectName;
	private Transform button;

	// Use this for initialization
	void Start () {
		lr = GetComponent<LineRenderer>();
		tm = GetComponent<TextMesh>();
		GetButtonName();

		tm.text = "";
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
		button = GameObject.Find(buttonObjectName).transform;

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
			tm.text = menuAction;
		}
		else
		{
			tm.text = movementAction;
		}
	}

	/// <summary>
	/// Sets the initial message.
	/// </summary>
	public void SetInitialMessage()
	{
		tm.text = movementAction;
	}

	private void GetButtonName()
	{

		switch (AssociatedButton)
		{
			case Button.A:
				buttonObjectName = "rctrl:b_button01";
				menuAction = "Toggle menu";
				movementAction = "Toggle menu";
				break;
			case Button.B:
				buttonObjectName = "rctrl:b_button02";
				menuAction = "Toggle menu";
				movementAction = "Toggle menu";
				break;
			case Button.LHand:
				buttonObjectName = "lctrl:b_hold";
				movementAction = "Decrease size";
				break;
			case Button.LIndex:
				buttonObjectName = "lctrl:b_trigger";
				movementAction = "Decrease size";
				break;
			case Button.LThumbstick:
				buttonObjectName = "lctrl:b_stick_IGNORE";
				movementAction = "Move up/down";
				menuAction = "Cycle menu items";
				break;
			case Button.RHand:
				buttonObjectName = "rctrl:b_hold";
				movementAction = "Increase size";
				break;
			case Button.RIndex:
				buttonObjectName = "rctrl:b_trigger";
				movementAction = "Increase size";
				break;
			case Button.RThumbstick:
				buttonObjectName = "rctrl:b_stick_IGNORE";
				movementAction = "Move around";
				menuAction = "Cycle meny items";
				break;
			case Button.X:
				buttonObjectName = "lctrl:b_button01";
				movementAction = "Reset position";
				break;
			case Button.Y:
				buttonObjectName = "lctrl:b_button02";
				movementAction = "Reset position";
				break;
		}

	}
}
