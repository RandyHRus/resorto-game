using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouristComponents: NPCComponents
{
    public TouristInformation TouristInformation => (TouristInformation)npcInformation;
    public readonly TouristDialogue dialogue;
    public readonly TouristInterest[] interests;
    public readonly TouristHappiness happiness;
    public HotelRoomRegionInstance AssignedRoom { get; private set; }
    public Vector2Int? AssignedBedPosition { get; private set; }
    
    private HashSet<Activity> interestedActivities;

    public TouristComponents(TouristInformation info, Transform touristTransform, TouristDialogue dialogue, TouristInterest[] interests, TouristHappiness happiness): base(info, touristTransform)
    {
        this.dialogue = dialogue;
        this.interests = interests;
        this.happiness = happiness;

        interestedActivities = new HashSet<Activity>();
        foreach (TouristInterest i in interests)
        {
            foreach (Activity a in i.Activies)
            {
                if (!interestedActivities.Contains(a))
                    interestedActivities.Add(a);
            }
        }

        HotelsManager.Instance.OnAvailableRoomsCountChanged += OnAvailableRoomsCountChangedHandler;
        SubscribeToEvent(NPCInstanceEvent.Delete, OnNPCDeleteHandler);

        TryAssignRandomRoom();
    }

    private void OnNPCDeleteHandler(object[] args)
    {
        UnsubscribeToEvent(NPCInstanceEvent.Delete, OnNPCDeleteHandler);
        HotelsManager.Instance.OnAvailableRoomsCountChanged -= OnAvailableRoomsCountChangedHandler;

        if (AssignedRoom != null)
        {
            UnAssignRoom();
        }
    }

    private void OnAvailableRoomsCountChangedHandler(int count)
    {
        if (AssignedRoom == null)
            TryAssignRandomRoom();
    }

    private void TryAssignRandomRoom()
    {
        HotelRoomRegionInstance availableRoom = HotelsManager.Instance.GetRandomOpenValidRoom();
        if (availableRoom != null)
        {
            AssignRoom(availableRoom);
        }
    }

    private void AssignRoom(HotelRoomRegionInstance room)
    {
        AssignedRoom = room;
        room.OccupyRoom();
        room.OnRoomValidityChanged += OnAssignedRoomValidityChangedHandler;
        room.OnRegionRemoved += OnAssignedRoomRemovedHandler;
        //TODO: Change so each member in group gets assigned a different bed when I implement tourist groups
        AssignedBedPosition = AssignedRoom.GetRandomBedPositionInRoom(); 
    }

    private void UnAssignRoom()
    {
        AssignedRoom.UnOccupyRoom();
        AssignedRoom.OnRoomValidityChanged -= OnAssignedRoomValidityChangedHandler;
        AssignedRoom.OnRegionRemoved -= OnAssignedRoomRemovedHandler;
        AssignedBedPosition = null;
        AssignedRoom = null;
        Debug.Log("Unassign");
    }

    private void OnAssignedRoomValidityChangedHandler(HotelRoomRegionInstance room)
    {
        if (!room.IsValid)
            UnAssignRoom();
    }

    private void OnAssignedRoomRemovedHandler()
    {
        UnAssignRoom();
    }

    public bool IsInterestedInActivity(Activity activity)
    {
        return interestedActivities.Contains(activity);
    }

    //Gets random empty position next to bed
    public Vector2Int? GetRandomBedAccessLocation()
    {
        if (AssignedBedPosition == null)
            return null;

        Vector2Int bedPosition = (Vector2Int)AssignedBedPosition;

        TileInformationManager.Instance.TryGetTileInformation(bedPosition, out TileInformation tileInfo);
        BuildOnTile bed = tileInfo.TopMostBuild;

        ArrayHashSet<Vector2Int> emptyNeighbourPositions = new ArrayHashSet<Vector2Int>();

        foreach (Vector2Int bedNeighbourPos in bed.neighbours)
        {
            if (!CollisionManager.CheckForCollisionOnTile(bedNeighbourPos, tileInfo.layerNum))
                emptyNeighbourPositions.Add(bedNeighbourPos);
        }

        if (emptyNeighbourPositions.Count == 0)
        {
            Debug.Log("No access to bed!");
            return null;
        }

        return emptyNeighbourPositions.GetRandom();
    }
}
