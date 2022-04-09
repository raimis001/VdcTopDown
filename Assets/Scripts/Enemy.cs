using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
	public float hp = 1;
	public float speed = 2;

	public float attackRadius = 3.5f;
	public float coolDown = 1;
	public float attackHit = 0.05f;

	public GameObject attackEffect;

	float coolTime = 0;

	private void Start()
	{
		attackEffect.SetActive(false);
	}

	public void Hit(float hit)
	{
		hp -= hit;
		if (hp < 0)
		{
			gameObject.SetActive(false);
		}
	}

	private void Update()
	{
		transform.position = Vector2.MoveTowards(transform.position, Player.instance.transform.position, Time.deltaTime * speed);

		Vector2 pos = Player.instance.transform.position;
		Vector2 self = transform.position;

		if (Vector2.Distance(pos, self) < attackRadius)
		{
			Attack();
		}

	}

	void Attack()
	{
		if (coolTime > 0)
		{
			coolTime -= Time.deltaTime;
			return;
		}

		coolTime = coolDown;
		attackEffect.SetActive(true);
		Invoke(nameof(HideEffect), 0.4f);
		Player.instance.Hit(attackHit);
	}

	void HideEffect()
	{
		attackEffect.SetActive(false);
	}

}
