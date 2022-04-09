using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Viking : MonoBehaviour
{
    public static Viking instance;
    public static Vector3 position => instance.transform.position; 

    [Header("Moving")]
    public float rotationSpeed = 3;
    public float moveSpeed = 5;
    public float runSpeed = 1.5f;

    public float coolDown = 0.7f;

    [Header("Meele attack")]
    public float attackRadius = 1f;
    public float attackHit = 1f;

    public List<GameObject> weapons;

    [Header("Bows")]
    public float bowCooldown = 1f;
    public Transform arrowPosition;
    public Arrow arrowPrefab;
    public Transform arrowPool;
    public Transform arrowMarker;
    public float bowHit = 1f;

    [Header("Shields")]
    public float shieldDefence = 0.5f;
    public GameObject shieldObject;
    private bool shield = false;

    [Header("Inventory")]
    public Inventory inventory;

    [Header("UI")]
    public Image hitProgress;
    public TMP_Text healthText;

    public Image staminaProgress;
    public TMP_Text staminaText;

    public InfoUI infoUI;

    [Header("Skills")]
    public SkillTree skillTree;
    public float staminaDelta = 1;
    public float staminaAttack = 5;
    public float staminaBow = 3;
    public float staminaRun = 1.5f;

    readonly List<Arrow> arrowList = new List<Arrow>();

    bool bowAttack = false;

    int weaponIndex = 0;

    float bowStrenght = 0;
    float maxStrenght = 1;

    float inputX;
    float inputShift;
    Rigidbody2D body;
    Animator anim;

    float coolTime = 0;
    float runTime = 0;

    internal float health = 20;
    internal float maxHealth = 20;

    internal float stamina = 20;
    internal float maxStamina = 20;


    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        maxStamina = Mathf.Ceil(skillTree.GetSkill(SkillKind.Stamina) * 20);
        maxHealth = Mathf.Ceil(skillTree.GetSkill(SkillKind.Health) * 20);

        stamina = maxStamina;
        health = maxHealth;

        ChangeWeapon();
    }

    void Update()
    {
        UpdateUI();

        Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mouse - (Vector2)transform.position;

        transform.right = Vector2.MoveTowards(transform.right, direction, Time.deltaTime * rotationSpeed);

        Vector3 euler = transform.eulerAngles;
        if (euler.y != 0)
        {
            euler.z = euler.y;
            euler.y = 0;
        }
        transform.eulerAngles = euler;


        inputX = Input.GetAxis("Vertical");

        inputShift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ? runSpeed * skillTree.GetSkill(SkillKind.Running) : 1;

        if (inputShift == 1)
        {
            runTime = 0;
            stamina += Time.deltaTime * staminaDelta;
            if (stamina > maxStamina)
                stamina = maxStamina;
        }
        else if (inputX != 0)
        {
            if (stamina <= 0)
            {
                infoUI.ShowMessage("LOW STAMINA!");
                inputShift = 1;
            }
            else
            {
                runTime += Time.deltaTime;
                stamina -= Time.deltaTime * staminaRun;
                if (stamina < 0)
                    stamina = 0;
            }
        }

        if (runTime > 5)
        {
            runTime = 0;
            skillTree.AddSkill(SkillKind.Running);
            AddStamina();
        }

        if (BuildUI.buildMode)
            return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            weaponIndex = 0;
            ChangeWeapon();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            weaponIndex = 1;
            ChangeWeapon();

            shield = false;
            anim.SetBool("Shield", shield);
            shieldObject.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && weaponIndex == 0)
        {
            if (shield)
            {
                shield = false;
                anim.SetBool("Shield", shield);
            }

            shieldObject.SetActive(!shieldObject.activeInHierarchy);
        }

        int alpha = -1;
        if (int.TryParse(Input.inputString, out int a))
            alpha = a;

        if (alpha > 3 && alpha < 9)
        {
            if (inventory.GetSlot(alpha, out ItemClass item) > 0)
            {
                if (item.type == 2)
                {
                    if (health < maxHealth)
                    {
                        health += item.health;
                        if (health > maxHealth)
                            health = maxHealth;

                        inventory.Remove(item.id);
                    }

                }
            }
        }

        if (weaponIndex == 0 && shieldObject.activeInHierarchy)
        {
            bool _shield = shield;
            shield = Input.GetMouseButton(1);
            if (_shield != shield)
            {
                anim.SetBool("Shield", shield);
            }
        }


        if (coolTime > 0)
        {
            coolTime -= Time.deltaTime;
            return;
        }

        if (weaponIndex == 0 && !shield)
        {
            if (Input.GetMouseButtonDown(0))
            {
                MeeleAttack();
            }
            return;
        }

        if (weaponIndex == 1)
        {
            if (inventory.Count("Arrows1") <= 0)
                return;

            if (!bowAttack && Input.GetMouseButtonDown(0))
            {
                Debug.Log("Start bow attack");
                bowAttack = true;
            }
            if (bowAttack && Input.GetMouseButton(0))
            {
                if (bowStrenght < maxStrenght)
                    bowStrenght += Time.deltaTime;

                Vector3 scale = Vector3.one * Mathf.Lerp(1, 0.3f, bowStrenght / maxStrenght);
                arrowMarker.localScale = scale;
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                pos.z = 0;
                arrowMarker.position = pos;
                arrowMarker.rotation = arrowPosition.rotation;
                arrowMarker.gameObject.SetActive(true);
            }
            if (Input.GetMouseButtonUp(0) && bowStrenght > 0)
            {
                BowAttack();
                inventory.Remove("Arrows1");
                bowStrenght = 0;
                coolTime = bowCooldown;
                arrowMarker.gameObject.SetActive(false);
            }
            if (Input.GetMouseButtonDown(1))
            {
                bowStrenght = 0;
                arrowMarker.gameObject.SetActive(false);
                bowAttack = false;
            }
        }

    }

    void UpdateUI()
    {
        hitProgress.fillAmount = health / maxHealth;
        staminaProgress.fillAmount = stamina / maxStamina;
        
        staminaText.text = maxStamina.ToString("0");
        healthText.text = maxHealth.ToString("0");
    }

    void ChangeWeapon()
    {
        for (int i = 0; i < weapons.Count; i++)
        {
            weapons[i].SetActive(i == weaponIndex);
        }
    }

    void MeeleAttack()
    {

        if (stamina < staminaAttack)
        {
            infoUI.ShowMessage("LOW STAMINA!");
            return;
        }

        //ATTACK
        anim.SetTrigger("MeeleAttack");
        coolTime = coolDown;

        stamina -= staminaAttack;
        AddStamina();

        //Check enemy

        Zombie[] zombies = GameObject.FindObjectsOfType<Zombie>();
        foreach (Zombie zombie in zombies)
        {
            if (Vector2.Distance(zombie.transform.position, transform.position) > attackRadius)
                continue;

            zombie.Hit(attackHit * skillTree.GetSkill(SkillKind.Swords));

            skillTree.AddSkill(0);
        }
    }

    void BowAttack()
    {
        if (stamina < staminaBow)
        {
            infoUI.ShowMessage("LOW STAMINA!");
            return;
        }

        Arrow arr1 = null;
        foreach (Arrow arrow in arrowList)
        {
            if (arrow.gameObject.activeInHierarchy)
                continue;

            arr1 = arrow;
            break;
        }

        if (arr1 == null)
        {
            arr1 = Instantiate(arrowPrefab, arrowPool);
            arrowList.Add(arr1);
        }

        stamina -= staminaBow;
        AddStamina();

        arr1.Shot(arrowPosition, bowStrenght, bowHit * skillTree.GetSkill(SkillKind.Bows), body.velocity.magnitude);
        skillTree.AddSkill(SkillKind.Bows);
    }

    public void Hit(float hp)
    {
        if (!gameObject.activeInHierarchy)
            return;

        health -= hp * (1 - (shield ? shieldDefence * skillTree.GetSkill(SkillKind.Shields) : 0));
        AddHealth();

        if (shield)
            skillTree.AddSkill(SkillKind.Shields);

        if (health < 0)
        {
            Die();
        }
    }

    public void Die()
    {
        gameObject.SetActive(false);
    }

    void FixedUpdate()
    {
        body.velocity = transform.right * moveSpeed * inputX * inputShift;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            if (collision.TryGetComponent(out MapItem item))
            {
                inventory.Add(item.item, item.count);
                collision.gameObject.SetActive(false);
            }
        }
        if (collision.CompareTag("Health"))
        {
            health = maxHealth;
            collision.gameObject.SetActive(false);
        }
    }

    private void AddStamina()
    {
        int st = skillTree.GetLevel(SkillKind.Stamina);

        skillTree.AddSkill(SkillKind.Stamina);

        int st1 = skillTree.GetLevel(SkillKind.Stamina);

        if (st == st1)
            return;

        maxStamina = Mathf.Ceil(20 * skillTree.GetSkill(SkillKind.Stamina));

    }

    private void AddHealth()
    {
        int h = skillTree.GetLevel(SkillKind.Health);

        skillTree.AddSkill(SkillKind.Health);

        int h1 = skillTree.GetLevel(SkillKind.Health);

        if (h == h1)
            return;

        maxHealth = Mathf.Ceil(20 * skillTree.GetSkill(SkillKind.Health));
    }

    public void Healing(float h)
    {
        health += h;
        if (health > maxHealth)
            health = maxHealth;
    }
}
