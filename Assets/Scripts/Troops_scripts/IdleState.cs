using UnityEngine;

public class IdleState : State
{
	public ChaseState ChaseState;
	public bool canSeeTheEnemy;

	public override State RunCurrentState()
	{
		if (canSeeTheEnemy)
		{
			return ChaseState;
		}
		else
		{
			return this;
		}
	}
}
