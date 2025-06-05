using UnityEngine;
public interface IDamageable
{
	void TakeDamage(int damage, Transform hitTransform = null,string name="");
}