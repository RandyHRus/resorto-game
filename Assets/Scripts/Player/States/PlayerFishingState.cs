using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Player/Fishing")]
public class PlayerFishingState : PlayerState
{
    private static FishingStateController controller;

    public override bool AllowMovement
    {
        get { return (controller.CurrentPhase is FishingDefaultPhase); }
    }

    public override bool AllowMouseDirectionChange
    {
        get { return (controller.CurrentPhase is FishingDefaultPhase); }
    }

    public override CameraMode CameraMode => CameraMode.Follow;

    public override void Initialize()
    {
        base.Initialize();

        FishingResources resources = new FishingResources(Player);

        controller = new FishingStateController(new PlayerFishingDefaultPhase(resources),
                                                new PlayerFishingChargingPhase(resources),
                                                new FishingCastingPhase(resources),
                                                new PlayerFishingBobbingPhase(resources),
                                                new PlayerFishingHookedPhase(resources),
                                                resources);
    }

    public class PlayerFishingDefaultPhase: FishingDefaultPhase
    {
        public PlayerFishingDefaultPhase(FishingResources resources): base(resources) { }

        public override void Execute()
        {
            base.Execute();

            if (CheckMouseOverUI.GetButtonDownAndNotOnUI("Primary"))
            {
                float xDir = PlayerDirection.Instance.VisualDirection.DirectionVector.x;
                float yDir = PlayerDirection.Instance.VisualDirection.DirectionVector.y;
                controller.SwitchPhase<FishingChargingPhase>(new object[] { xDir, yDir });
            }
        }
    }

    public class PlayerFishingChargingPhase: FishingChargingPhase
    {
        bool firstExecute;

        public PlayerFishingChargingPhase(FishingResources resources) : base(resources) { }

        public override void StartState(object[] args)
        {
            base.StartState(args);
            firstExecute = true;

            PlayerMovement.Instance.StopMovement(); //TODO: Maybe there could be a better way to call this?
        }

        public override void Execute()
        {
            base.Execute();

            //StartState is called 1 frame before, so we dont want the progress bar showing 1 frame before
            if (firstExecute)
            {

                ProgressBar.Show(true);
            }

            ProgressBar.SetFill(timer / maxCastHoldTime);

            if (Input.GetButtonUp("Primary"))
            {
                InvokeChangePhase(typeof(FishingCastingPhase), new object[] { timer / maxCastHoldTime, xDir, yDir });
                return;
            }

            firstExecute = false;
        }

        public override void EndState()
        {
            base.EndState();
            ProgressBar.Show(false);
        }
    }

    public class PlayerFishingBobbingPhase : FishingBobbingPhase
    {
        public PlayerFishingBobbingPhase(FishingResources resources) : base(resources) { }

        public override void Execute()
        {
            base.Execute();

            if (CheckMouseOverUI.GetButtonDownAndNotOnUI("Primary"))
            {
                InvokeChangePhase(typeof(FishingDefaultPhase), null);
                return;
            }
        }
    }

    public class PlayerFishingHookedPhase : FishingHookedPhase
    {
        private static readonly float fishCaughtFlySpeed = 10f;

        private Transform caughtFishFlyingInstance;
        private FishItemInstance randomFish;
        private int clickCount;

        public PlayerFishingHookedPhase(FishingResources resources) : base(resources) { }

        public override void StartState(object[] args)
        {
            base.StartState(args);
            clickCount = 5; //Todo: Come up with something else other then clicks
        }

        public override void Execute()
        {
            void FishFlyingProgress(Vector2 position)
            {
                caughtFishFlyingInstance.position = new Vector3(position.x, position.y, DynamicZDepth.GetDynamicZDepth(position, DynamicZDepth.CAUGHT_FISH_OFFSET));
            }

            void FishFlyingEnd()
            {
                GameObject.Destroy(caughtFishFlyingInstance.gameObject);
                InventoryManager.Instance.AddItem(randomFish, 1);
            }

            base.Execute();

            if (CheckMouseOverUI.GetButtonDownAndNotOnUI("Primary"))
                clickCount--;

            if (clickCount <= 0)
            {
                //Finish here
                GameObject.Destroy(swimmingFishTransform.gameObject);

                caughtFishFlyingInstance = GameObject.Instantiate(ResourceManager.Instance.CaughtFishPrefab, lineEndPosition, Quaternion.identity).transform;

                randomFish = CaughtFishGenerator.Instance.GetRandomFish();
                caughtFishFlyingInstance.GetComponent<SpriteRenderer>().sprite = randomFish.ItemInformation.ItemIcon;

                Coroutines.Instance.StartCoroutine(LerpEffect.LerpVectorSpeed(lineEndPosition, Player.position, fishCaughtFlySpeed, FishFlyingProgress, FishFlyingEnd, true));

                InvokeChangePhase(typeof(FishingDefaultPhase), null);
                return;
            }
        }
    }

    public override void StartState(object[] args)
    {
        controller.StartState();
    }

    public override void Execute()
    {
        controller.Execute();
    }

    public override void LateExecute()
    {
        base.LateExecute();
        controller.LateExecute();
    }

    public override void EndState()
    {
        controller.EndState();
    }
}
