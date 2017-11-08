//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: Sends simple controller button events to UnityEvents
//
//=============================================================================

using UnityEngine;
using UnityEngine.Events;

namespace Valve.VR.InteractionSystem
{
	//-------------------------------------------------------------------------
	[RequireComponent( typeof( Interactable ) )]
	public class InteractableButtonEvents : MonoBehaviour
	{
		public UnityEvent onTriggerDown;
		public UnityEvent onTriggerUp;
		public UnityEvent onGripDown;
		public UnityEvent onGripUp;
		public UnityEvent onTouchpadDown;
		public UnityEvent onTouchpadUp;
		public UnityEvent onTouchpadTouch;
		public UnityEvent onTouchpadRelease;

		//-------------------------------------------------
		void Update()
		{
			for ( int i = 0; i < Player.instance.handCount; i++ )
			{
				Hand hand = Player.instance.GetHand( i );

				if ( hand.controller != null )
				{
					if ( hand.controller.GetPressDown( Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger ) )
					{
						print("Trigger Down");
						onTriggerDown.Invoke();
					}

					if ( hand.controller.GetPressUp( Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger ) )
					{
						print("Trigger Up");
						onTriggerUp.Invoke();
					}

					if ( hand.controller.GetPressDown( Valve.VR.EVRButtonId.k_EButton_Grip ) )
					{
						print("Grip Down");
						onGripDown.Invoke();
					}

					if ( hand.controller.GetPressUp( Valve.VR.EVRButtonId.k_EButton_Grip ) )
					{
						print("Grip Up");
						onGripUp.Invoke();
					}

					if ( hand.controller.GetPressDown( Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad ) )
					{
						print("Touch Pad Down");
						onTouchpadDown.Invoke();
					}

					if ( hand.controller.GetPressUp( Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad ) )
					{
						print("Touch Pad Up");
						onTouchpadUp.Invoke();
					}

					if ( hand.controller.GetTouchDown( Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad ) )
					{
						print("Touchpad Touch");
						onTouchpadTouch.Invoke();
					}

					if ( hand.controller.GetTouchUp( Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad ) )
					{
						print("Touchpad Release");
						onTouchpadRelease.Invoke();
					}
				}
			}

		}
	}
}
