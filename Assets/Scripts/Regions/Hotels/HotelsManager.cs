using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotelsManager: MonoBehaviour
{
    [SerializeField] HotelRoomRegionInformation hotelRoomRegionInformation = null;

    //Available room means valid and unoccupied
    private ArrayHashSet<HotelRoomRegionInstance> availableRooms = new ArrayHashSet<HotelRoomRegionInstance>();
    public int AvailableRoomsCount => availableRooms.Count;

    private ArrayHashSet<HotelRoomRegionInstance> validRooms = new ArrayHashSet<HotelRoomRegionInstance>();

    private Dictionary<HotelRoomRegionInstance, HotelFrontDeskRegionInstance> roomToFrontDeskConnection = new Dictionary<HotelRoomRegionInstance, HotelFrontDeskRegionInstance>();
    private Dictionary<HotelFrontDeskRegionInstance, HashSet<HotelRoomRegionInstance>> frontDeskToRoomsConnection = new Dictionary<HotelFrontDeskRegionInstance, HashSet<HotelRoomRegionInstance>>();

    public delegate void HotelRoomCountChanged(int newCount);
    public event HotelRoomCountChanged OnAvailableRoomsCountChanged;
    public event HotelRoomCountChanged OnValidRoomsCountChanged;

    private static HotelsManager _instance;
    public static HotelsManager Instance { get { return _instance; } }
    private void Awake()
    {
        //Singleton
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }
        }
    }

    private void Start()
    {
        RegionManager.Instance.OnRegionCreatedEventManager.Subscribe(hotelRoomRegionInformation, OnHotelRoomCreatedHandler);
        RegionManager.Instance.OnRegionRemovedEventManager.Subscribe(hotelRoomRegionInformation, OnHotelRoomRemovedHandler);
    }

    private void OnDestroy()
    {
        RegionManager.Instance.OnRegionCreatedEventManager.Unsubscribe(hotelRoomRegionInformation, OnHotelRoomCreatedHandler);
        RegionManager.Instance.OnRegionRemovedEventManager.Unsubscribe(hotelRoomRegionInformation, OnHotelRoomRemovedHandler);
    }

    private void OnHotelRoomCreatedHandler(object[] args)
    {
        HotelRoomRegionInstance newRegion = (HotelRoomRegionInstance)args[0];

        newRegion.OnRoomValidityChanged += RefreshRoomValidity;
        newRegion.OnRoomOccupied += RefreshRoomAvailablility;
        newRegion.OnRoomUnOccupied += RefreshRoomAvailablility;

        RefreshRoomValidity(newRegion);
    }

    private void OnHotelRoomRemovedHandler(object[] args)
    {
        HotelRoomRegionInstance regionToRemove = (HotelRoomRegionInstance)args[0];

        regionToRemove.OnRoomValidityChanged -= RefreshRoomValidity;
        regionToRemove.OnRoomOccupied -= RefreshRoomAvailablility;
        regionToRemove.OnRoomUnOccupied -= RefreshRoomAvailablility;

        if (availableRooms.Contains(regionToRemove))
        {
            availableRooms.Remove(regionToRemove);
            OnAvailableRoomsCountChanged?.Invoke(AvailableRoomsCount);
        }
        if (validRooms.Contains(regionToRemove))
        {
            validRooms.Remove(regionToRemove);
            OnValidRoomsCountChanged?.Invoke(validRooms.Count);
        }
    }

    private void RefreshRoomAvailablility(HotelRoomRegionInstance room)
    {
        if (room.IsValidAndUnOccupied && !availableRooms.Contains(room))
        {
            availableRooms.Add(room);
            OnAvailableRoomsCountChanged?.Invoke(AvailableRoomsCount);
        }
        else if (!room.IsValidAndUnOccupied && availableRooms.Contains(room))
        {
            availableRooms.Remove(room);
            OnAvailableRoomsCountChanged?.Invoke(AvailableRoomsCount);
        }      
    }

    private void RefreshRoomValidity(HotelRoomRegionInstance room)
    {
        if (room.IsValid && !validRooms.Contains(room))
        {
            validRooms.Add(room);
            OnValidRoomsCountChanged?.Invoke(validRooms.Count);
        }
        else if (!room.IsValid && validRooms.Contains(room))
        {
            validRooms.Remove(room);
            OnValidRoomsCountChanged?.Invoke(validRooms.Count);
        }

        RefreshRoomAvailablility(room);
    }

    public HotelRoomRegionInstance GetRandomOpenValidRoom()
    {
        if (availableRooms.Count == 0)
            return null;
        else
            return availableRooms.GetRandom();
    }

    public HotelFrontDeskRegionInstance GetFrontDeskConnectedToRoom(HotelRoomRegionInstance room)
    {
        if (roomToFrontDeskConnection.TryGetValue(room, out HotelFrontDeskRegionInstance frontDesk))
        {
            return frontDesk;
        }
        else
        {
            return null;
        }
    }

    public HashSet<HotelRoomRegionInstance> GetRoomsConnectedToFrontDesk(HotelFrontDeskRegionInstance frontDesk)
    {
        if (frontDeskToRoomsConnection.TryGetValue(frontDesk, out HashSet<HotelRoomRegionInstance> rooms))
        {
            return new HashSet<HotelRoomRegionInstance>(rooms); //Creates a copy
        }
        else
        {
            return null;
        }
    }

    public void AddConnection(HotelFrontDeskRegionInstance frontDesk, HotelRoomRegionInstance room)
    {
        if (roomToFrontDeskConnection.ContainsKey(room))
        {
            throw new System.Exception("Room already has connection");
        }

        //Add room to frontDesk connection
        roomToFrontDeskConnection.Add(room, frontDesk);

        //Add frontDesk to rooms connection
        if (frontDeskToRoomsConnection.TryGetValue(frontDesk, out HashSet<HotelRoomRegionInstance> existingSet))
        {
            existingSet.Add(room);
        }
        else
        {
            HashSet<HotelRoomRegionInstance> newSet = new HashSet<HotelRoomRegionInstance>();
            newSet.Add(room);
            frontDeskToRoomsConnection.Add(frontDesk, newSet);
        }
    }

    public void RemoveConnection(HotelRoomRegionInstance room)
    {
        HotelFrontDeskRegionInstance frontDesk = roomToFrontDeskConnection[room];
        roomToFrontDeskConnection.Remove(room);
        frontDeskToRoomsConnection[frontDesk].Remove(room);
    }
}
