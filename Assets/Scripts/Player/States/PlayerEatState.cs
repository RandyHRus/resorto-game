using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Eat")]
public class PlayerEatState : PlayerState
{
    public override bool AllowMovement => (eatingCoroutine == null);
    public override bool AllowMouseDirectionChange => (eatingCoroutine == null);
    public override CameraMode CameraMode => CameraMode.Follow;

    private static readonly float eatTime = 3f;
    private Coroutine eatingCoroutine;

    private FoodItemInformation food;

    public delegate void FoodEaten(FoodItemInformation food);
    public event FoodEaten OnFoodEaten;

    public override void StartState(object[] args)
    {
        food = (FoodItemInformation)args[0];
    }

    public override void Execute()
    {
        if (CheckMouseOverUI.GetButtonDownAndNotOnUI("Primary"))
        {
            eatingCoroutine = Coroutines.Instance.StartCoroutine(Eating());
        }
        if (Input.GetButtonUp("Primary"))
        {
            StopEating();
        }
    }

    public override void EndState()
    {
        StopEating();
    }

    IEnumerator Eating()
    {
        float timer = 0;
        ProgressBar.Show(true);

        while (Input.GetButton("Primary") && timer < eatTime)
        {
            timer += Time.deltaTime;
            if (timer >= eatTime)
                timer = eatTime;

            ProgressBar.SetFill(timer / eatTime);

            yield return 0;
        }

        InventoryManager.Instance.SelectedSlot.RemoveItem(1); //TODO make listener of event

        OnFoodEaten?.Invoke(food);

        ProgressBar.Show(false);
    }

    private void StopEating()
    {
        if (eatingCoroutine != null)
        {
            Coroutines.Instance.StopCoroutine(eatingCoroutine);
            eatingCoroutine = null;
        }

        ProgressBar.Show(false);
    }
}
