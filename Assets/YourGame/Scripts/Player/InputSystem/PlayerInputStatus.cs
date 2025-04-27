using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerInputAction
{
    public class PlayerInputStatus : MonoBehaviour
    {
        [Header("Character Input Values")]
		public Vector2 move;
		public bool interactive;
		public bool map;
		public bool restart;
		public bool equip1;
		public bool equip2;
		public bool switchequip;
		public bool useEquip;
		public bool bag;
		public bool skill;
		public bool target;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnInteractive(InputValue value)
		{
			interactive = value.isPressed;
		}

		public void OnMap(InputValue value)
		{
			map = value.isPressed;
		}

		public void OnRestart(InputValue value)
		{
			restart = value.isPressed;
		}

		public void OnEquip1(InputValue value)
		{
			equip1 = value.isPressed;
		}
		public void OnEquip2(InputValue value)
		{
			equip2 = value.isPressed;
		}

		public void OnSwitchEquip(InputValue value)
		{
			switchequip = value.isPressed;
		}

		public void OnUseEquip(InputValue value)
		{
			useEquip = value.isPressed;
		}

		public void OnBag(InputValue value)
		{
			bag = value.isPressed;
		}

		public void OnSkill(InputValue value)
		{
			skill = value.isPressed;
		}

		public void OnTarget(InputValue value)
		{
			target = value.isPressed;
		}


		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void InteractiveInput(bool newInteractiveState)
		{
			interactive = newInteractiveState;
		}

		public void MapInput(bool newMapState)
		{
			map = newMapState;
		}

		public void RestartInput(bool newRestartState)
		{
			restart = newRestartState;
		}

		public void Equip1Input(bool newEquip1State)
		{
			equip1 = newEquip1State;
		}

		public void Equip2Input(bool newEquip2State)
		{
			equip2 = newEquip2State;
		}

		public void SwitchEquipInput(bool newSwitchEquipState)
		{
			switchequip = newSwitchEquipState;
		}

		public void UseEquipInput(bool newUseEquipState)
		{
			useEquip = newUseEquipState;
		}

		public void BagInput(bool newBagState)
		{
			bag = newBagState;
		}

		public void SkillInput(bool newSkillState)
		{
			skill = newSkillState;
		}

		public void TargetInput(bool newTargetState)
		{
			target = newTargetState;
		}


		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
    }
}
