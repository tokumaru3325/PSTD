using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public enum ProjectileType
{
    Arrow,
    AttackSpell,
    HealSpell
}

public class ProjectileInfo
{
    public ProjectileType type;

    public GameObject prefab;
}

public class ProjectileObjectPool : MonoBehaviour
{
    [SerializeField]
    List<ProjectileInfo> _projectileObjects;

    List<Arrow> ArrowPool;
    List<LightningSpell> LightningSpellPool;
    List<HealSpell> HealSpellPool;

    public static ProjectileObjectPool Instance { get; private set; }

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void Create(int maxCount)
    {
        ArrowPool = new List<Arrow>();
        LightningSpellPool = new List<LightningSpell>();
        HealSpellPool = new List<HealSpell>();

        for (int j = 0; j < _projectileObjects.Count; j++) {
            for (int i = 0; i < maxCount; i++)
            {               
                GameObject gameObject = Instantiate(_projectileObjects[j].prefab);
                Projectile projectile = gameObject.GetComponent<Arrow>();
                projectile.gameObject.SetActive(false);
                //ArrowPool.Add(projectile);
            }
        }
    }

    private void CreateProjectilePool(int maxCount, ProjectileType type)
    {
        GameObject gameObject = _projectileObjects.Find(n => n.type == type).prefab;

        for(int i = 0; i < maxCount; i++)
        {

        }
    }

    public Projectile GetObj(ProjectileType type)
    {

        return null;
    }

    public void Release(Projectile obj)
    {
        obj.gameObject.SetActive(false);
    }
}
