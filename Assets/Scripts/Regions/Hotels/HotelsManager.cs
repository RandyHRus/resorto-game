using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotelsManager: MonoBehaviour
{
    [SerializeField] HotelRoomRegionInformation hotelRoomRegionInformation = null;

    //Available room means valid and unoccupied
    private static ArrayHashSet<HotelRoomRegionInstance> availableRooms = new ArrayHashSet<HotelRoomRegionInstance>();
    public static int AvailableRoomsCount => availableRooms.Count;

    private static ArrayHashSet<HotelRoomRegionInstance> validRooms = new ArrayHashSet<HotelRoomRegionInstance>();

    public delegate void HotelRoomCountChanged(int newCount);
    public static event HotelRoomCountChanged OnAvailableRoomsCountChanged;
    public static event HotelRoomCountChanged OnValidRoomsCountChanged;

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

        RegionManager.OnRegionCreatedEventManager.Subscribe(hotelRoomRegionInformation, OnHotelRoomCreatedHandler);
        RegionManager.OnRegionRemovedEventManager.Subscribe(hotelRoomRegionInformation, OnHotelRoomRemovedHandler);
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
}
