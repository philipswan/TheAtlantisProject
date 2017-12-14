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
			Vector3 pos1 = button.position + button.transform.forward * 0.00001f;
			Vector3 pos2 =  button.position + button.transform.forward * 0.05f;
			Vector4 pos3 = transform.position;
			Vector3[] linePos = new Vector3[] {pos1, pos2, pos3};

			lr.positionCount = linePos.Length;
			lr.SetPositions(linePos);
		}
	}

	/// <summary>
	/// Set the button transform reference after it's created
	/// </summary>
	public void OnCreate()
	{
		button = GameObject.Find(ButtonObjectName).transform;
		transform.parent.GetComponent<HelpMenuManager>().DeactivateChildren();
		tm.text = MovementAction;
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
}
