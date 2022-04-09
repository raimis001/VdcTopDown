using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
	public float radius = 5;
	public int maxEnemy = 10;
	public float interval = 5;

	public Zombie enemyPrefab;

	readonly List<Zombie> enemyList = new List<Zombie>();

	private void Start()
	{
		StartCoroutine(Spawn());
	}

	IEnumerator Spawn()
	{
		float time = interval;
		while (true)
		{
			yield return new WaitForSeconds(time);

			bool isSpawn = false;
			foreach (Zombie e in enemyList)
			{
				if (e.gameObject.activeInHierarchy)
					continue;

				SpawnEnemy(e);
				isSpawn = true;
				break;
			}

			if (isSpawn)
				continue;

			if (enemyList.Count >= maxEnemy)
				continue;

			Zombie e1 = Instantiate(enemyPrefab, transform);
			enemyList.Add(e1);
			SpawnEnemy(e1);

		}
	} 

	void SpawnEnemy(Zombie enemy)
	{
		Vector3 pos = Random.insideUnitCircle * radius;
		enemy.transform.position = transform.position + pos;
		enemy.gameObject.SetActive(true);
	}

}
