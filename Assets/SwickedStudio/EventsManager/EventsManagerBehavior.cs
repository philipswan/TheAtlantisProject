using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventsManagerBehavior : MonoBehaviour
{
    /// Client API

    public delegate void OneOffEventHandler(object data, EventArgs args);
    public delegate void ToggleEventHandler(bool data, EventArgs args);
    public delegate void TimedEventHandler(int data, EventArgs args);
    
    // Get the singleton instance
    public static EventsManagerBehavior instance
    {
        get
        {
            if (!eventManager)
            {
                eventManager = FindObjectOfType(typeof(EventsManagerBehavior)) as EventsManagerBehavior;

                if (!eventManager)
                {
                    Debug.LogError("There needs to be one active EventManger script on a GameObject in your scene.");
                }
                else
                {
                    eventManager.Init();
                }
            }

            return eventManager;
        }
    }

    public void OneOffListen(string eventName, OneOffEventHandler callback)
    {
		OneOffEventHandler e;

        if (instance.oneOffEventRegistry.TryGetValue (eventName, out e))
        {
            e += callback;
        } 
        else
        {
            e += callback;
            instance.oneOffEventRegistry.Add(eventName, e);
        }
    }

    public void OneOffTrigger(string eventName, object data)
    {
        OneOffEventHandler e;

        if (instance.oneOffEventRegistry.TryGetValue(eventName, out e))
        {
			if (e != null)
            {
            	e(data, EventArgs.Empty);
            }
		}
    }

    public void ToggleListen(string eventName, ToggleEventHandler callback)
    {
		ToggleEventHandler e;

        if (instance.toggleEventRegistry.TryGetValue (eventName, out e))
        {
            e += callback;
        } 
        else
        {
            e += callback;
            instance.toggleEventRegistry.Add(eventName, e);
        }
    }

    public void ToggleTrigger(string eventName, bool data)
    {
        ToggleEventHandler e;

        if (instance.toggleEventRegistry.TryGetValue(eventName, out e))
        {
			if (e != null)
            {
            	e(data, EventArgs.Empty);
            }
		}
    }

    public void TimedListen(string eventName, TimedEventHandler callback)
    {
		TimedEventHandler e;

        if (instance.timedEventRegistry.TryGetValue (eventName, out e))
        {
            e += callback;
        } 
        else
        {
            e += callback;
            instance.timedEventRegistry.Add(eventName, e);
        }
    }

    public void TimedTrigger(string eventName, int data)
    {
        TimedEventHandler e;

        if (instance.timedEventRegistry.TryGetValue(eventName, out e))
        {
			if (e != null)
            {
            	e(data, EventArgs.Empty);
            }
		}
    }

    /// Callbacks

    private static EventsManagerBehavior eventManager;

    private Dictionary<string, OneOffEventHandler> oneOffEventRegistry;
    private Dictionary<string, ToggleEventHandler> toggleEventRegistry;
    private Dictionary<string, TimedEventHandler> timedEventRegistry;

    // Singleton constructor
    private void Init()
    {
        if (oneOffEventRegistry == null)
        {
            oneOffEventRegistry = new Dictionary<string, OneOffEventHandler>();
        }

        if (toggleEventRegistry == null)
        {
            toggleEventRegistry = new Dictionary<string, ToggleEventHandler>();
        }
    }

	


    // ---

    public void DebugEvent(OneOffEventHandler e)
    {
        if (e != null)
        {
            int i = 0;
            foreach (var thing in e.GetInvocationList())
            {
                i++;
                Debug.Log(i + ": " + thing.ToString());
            }
        }
        else
        {
            Debug.Log("null");
        }
    }


}
