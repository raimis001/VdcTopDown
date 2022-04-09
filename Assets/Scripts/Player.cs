using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
	public static Player instance;

	public float speed = 3;
	public Transform attackPoint;
	public LayerMask enemyLayer;
	public Transform attackPivot;
	public Image hpProgress;

	public SpriteRenderer swordSprite;
	public Sprite[] swords;

	public Text killText;

	float hp = 1;
	float attackHit = 0.2f;
	int dieCount = 0;
	int swordLevel = 1;

	Rigidbody2D rigi;
	Vector2 input = Vector2.zero;
	Animator anim;
	

	private void Awake()
	{
		instance = this;
	}

	void Start()
	{
		rigi = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		killText.text = "0";
	}

	void Update()
	{
		hpProgress.fillAmount = hp;

		//TODO read inputs
		input.x = Input.GetAxis("Horizontal");
		input.y = Input.GetAxis("Vertical");

		if (Input.GetKeyDown(KeyCode.Space))
		{
			Attack();
		}

	}

	void FixedUpdate()
	{
		//TODO fizika update
		rigi.velocity = input * speed;

		if (input.x != 0)
		{
			Vector3 scale = attackPivot.localScale;
			scale.x = rigi.velocity.x < 0 ? 1 : -1;
			attackPivot.localScale = scale;
		}
	}

	void Attack()
	{
		anim.SetTrigger("Attack");

		Collider2D c = Physics2D.OverlapCircle(attackPoint.position, 0.5f, enemyLayer);
		if (c == null)
			return;

		Enemy e = c.GetComponent<Enemy>();
		if (e == null)
			return;

		e.Hit(attackHit);
		if (e.hp <= 0)
			dieCount++;

		killText.text = dieCount.ToString();

		if (dieCount > swordLevel * 10)
		{
			swordLevel++;
			attackHit += 0.05f;

			if (swords.Length > swordLevel - 1)
			{
				swordSprite.sprite = swords[swordLevel - 1];
			}
		}
	}
	public void Hit(float hit)
	{
		hp -= hit;
		if (hp <= 0)
			Die();
	}

	void Die()
	{
		SceneManager.LoadScene("SampleScene");
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!collision.CompareTag("Health"))
			return;

		if (hp >= 1)
			return;

		hp = 1;
		Destroy(collision.gameObject);
	}
}
