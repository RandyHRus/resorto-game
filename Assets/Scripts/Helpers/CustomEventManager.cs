using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomEventManager<T>
{
    private Dictionary<T, CustomEventGroup<T>> customEvents = new Dictionary<T, CustomEventGroup<T>>();

    public void Subscribe(T key, CustomEventGroup<T>.Delegate eventHandler)
    {
        void RemoveEvent(T eventKey)
        {
            if (!customEvents.TryGetValue(key, out CustomEventGroup<T> senderEventGroup))
            {
                throw new System.Exception("Key not found");
            }

            senderEventGroup.OnSubscribersEmpty -= RemoveEvent;
            customEvents.Remove(eventKey);
        }

        if (customEvents.TryGetValue(key, out CustomEventGroup<T> timeEvent))
        {
            timeEvent.AddSubscriber(eventHandler);
        }
        else
        {
            CustomEventGroup<T> newEventGroup = new CustomEventGroup<T>(key, eventHandler);
            customEvents.Add(key, newEventGroup);
            newEventGroup.OnSubscribersEmpty += RemoveEvent;
        }
    }

    public void Unsubscribe(T key, CustomEventGroup<T>.Delegate eventHandler)
    {
        if (!customEvents.TryGetValue(key, out CustomEventGroup<T> eventGroup))
        {
            throw new System.Exception("Key not found");
        }

        eventGroup.RemoveSubscriber(eventHandler);
    }

    public void TryInvokeEventGroup(T key, object[] args)
    {
        if (customEvents.TryGetValue(key, out CustomEventGroup<T> eventGroup))
        {
            eventGroup.InvokeEvent(args);
        }
    }
}

public class CustomEventGroup<T>
{
    public delegate void Delegate(object[] args);

    public delegate void SubscribersEmpty(T key);
    public event SubscribersEmpty OnSubscribersEmpty;

    private HashSet<Delegate> delegates;
    private T key;

    private bool invoking = false;
    private List<Delegate> waitingToBeUnSubbed;

    public CustomEventGroup(T key, Delegate initialSubscriber)
    {
        this.key = key;

        delegates = new HashSet<Delegate>();
        delegates.Add(initialSubscriber);
    }

    public void AddSubscriber(Delegate subscriber)
    {
        delegates.Add(subscriber);
    }

    public void RemoveSubscriber(Delegate subscriber)
    {
        if (invoking)
        {
            waitingToBeUnSubbed.Add(subscriber);
            return;
        }

        if (!delegates.Remove(subscriber))
            throw new System.Exception("Nothing to remove!");

        if (delegates.Count <= 0)
            OnSubscribersEmpty?.Invoke(key);
    }

    public void InvokeEvent(object[] args)
    {
        invoking = true;
        waitingToBeUnSubbed = new List<Delegate>();

        foreach (Delegate d in delegates)
        {
            d.Invoke(args);
        }

        invoking = false;
        foreach (Delegate d in waitingToBeUnSubbed)
        {
            RemoveSubscriber(d);
        }
    }
}
