using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "States/NPC/Fishing")]
public class NPCFishingState : NPCActivityState, ITouristStateDialogue
{
    private FishingStateController controller;

    public override string DisplayMessage => "Fishing";

    public override void Initialize()
    {
        base.Initialize();

        FishingResources resources = new FishingResources(npcInstance.npcTransform, new object[] { npcInstance });

        controller = new FishingStateController(new NPCFishingDefaultPhase(resources),
                                                new NPCFishingChargingPhase(resources),
                                                new FishingCastingPhase(resources),
                                                new NPCFishingBobbingPhase(resources),
                                                new NPCFishingHookedPhase(resources), 
                                                resources);

        //controller.OnFishingEnd += () => InvokeChangeState(typeof(NPCIdleState), null);
        controller.OnFishingEnd += InvokeEndState; //Can't do above because NPCIdle state might not be present (Eg. Could be touristIdleState)
    }

    public class NPCFishingDefaultPhase: FishingDefaultPhase
    {
        public NPCFishingDefaultPhase(FishingResources resources) : base(resources) { }

        public override void StartState(object[] args)
        {
            base.StartState(args);

            Vector2Int currentPosition = new Vector2Int(Mathf.RoundToInt(resources.characterTransform.position.x), Mathf.RoundToInt(resources.characterTransform.position.y));

            TileInformationManager.Instance.TryGetTileInformation(currentPosition, out TileInformation tileInfo);
            RegionInstance region = tileInfo.Region;

            if (region is FishingRegionInstance fishingRegion)
            {
                if (!fishingRegion.IsValidFishingPositionInThisRegion(currentPosition))
                {
                    //End fishing
                    Debug.Log("No longer a valid fishing position, ending state.");
                    InvokeChangePhase(null, null);
                    return;
                }

                List<Vector2Int> validDirections = fishingRegion.GetDirectionsWithWater(currentPosition);              

                Vector2Int targetDirection = validDirections[UnityEngine.Random.Range(0, validDirections.Count)];

                NPCInstance npcInstance = (NPCInstance)resources.additionalResources[0];
                npcInstance.npcDirection.SetDirectionOnMove(targetDirection);

                int targetFishingLineLength = 0;

                for (int i = 1; i <= maxCastLength; i++)
                {
                    Vector2Int checkPos = currentPosition + (targetDirection * i);
                    if (fishingRegion.IsDeepWaterInRegion(checkPos))
                    {
                        targetFishingLineLength = i;
                    }
                    else
                    {
                        break;
                    }
                }

                InvokeChangePhase(typeof(FishingChargingPhase), new object[] { (float)targetDirection.x, (float)targetDirection.y, targetFishingLineLength });
                return;
            }
            else
            {
                //End fishing
                Debug.Log("No longer a fishing region, ending state.");
                InvokeChangePhase(null, null);
                return;
            }
        }
    }

    public class NPCFishingChargingPhase: FishingChargingPhase
    {
        public NPCFishingChargingPhase(FishingResources resources) : base(resources) { }

        private float targetTimer;

        public override void StartState(object[] args)
        {
            base.StartState(args);

            int targetFishingLineLength = (int)args[2];

            //We want (timer/maxCastHoldTime)*maxFishingLineLength = targetCastLength
            targetTimer = (targetFishingLineLength * maxCastHoldTime)/maxCastLength;
        }

        public override void Execute()
        {
            base.Execute();

            if (timer >= targetTimer)
            {
                InvokeChangePhase(typeof(FishingCastingPhase), new object[] { timer/maxCastHoldTime, xDir, yDir });
                return;
            }
        }
    }

    public class NPCFishingBobbingPhase : FishingBobbingPhase
    {
        public NPCFishingBobbingPhase(FishingResources resources) : base(resources) { }

        float timerTillEnd;

        public override void StartState(object[] args)
        {
            base.StartState(args);
            timerTillEnd = UnityEngine.Random.Range(5f, 6f);
        }

        public override void Execute()
        {
            base.Execute();

            timerTillEnd -= Time.deltaTime;

            if (timerTillEnd < 0)
            {
                //End fishing
                InvokeChangePhase(null, null);
                return;
            }
        }
    }

    public class NPCFishingHookedPhase: FishingHookedPhase
    {
        public NPCFishingHookedPhase(FishingResources resources) : base(resources) { }

        float secondsToBeHooked;

        public override void StartState(object[] args)
        {
            base.StartState(args);

            secondsToBeHooked = UnityEngine.Random.Range(2f, 5f);
        }

        public override void Execute()
        {
            base.Execute();

            secondsToBeHooked -= Time.deltaTime;

            if (secondsToBeHooked < 0)
            {
                //End fishing
                InvokeChangePhase(null, null);
                return;
            }
        }
    }

    public override void StartState(object[] args)
    {
        base.StartState(args);
        controller.StartState();
    }

    public override void Execute()
    {
        base.Execute();
        controller.Execute();
    }

    public override void LateExecute()
    {
        base.LateExecute();
        controller.LateExecute();
    }

    public override void EndState()
    {
        base.EndState();
        controller.EndState();
    }

    public TouristDialogueType GetTouristDialogueType()
    {
        return TouristDialogueType.Fishing;
    }
}
