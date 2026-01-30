using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public enum ProjectileType
{
    Arrow,
    AttackSpell,
    HealSpell
}

[Serializable]
public class ProjectileInfo
{
    [Tooltip("投射物の種類")]
    public ProjectileType type;

    [Tooltip("投射物のプレファブ")]
    public GameObject prefab;
}

public class ProjectileObjectPool : MonoBehaviour
{
    [Tooltip("投射物の関連資料")]
    [SerializeField]
    private List<ProjectileInfo> _projectileObjects;

    List<Projectile> ArrowPool;
    List<Projectile> LightningSpellPool;
    List<Projectile> HealSpellPool;

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

    public void CreatePool(int maxCount)
    {
        ArrowPool = new List<Projectile>();
        LightningSpellPool = new List<Projectile>();
        HealSpellPool = new List<Projectile>();

        FillObjectPool(ProjectileType.Arrow, ArrowPool, maxCount);
        FillObjectPool(ProjectileType.AttackSpell, LightningSpellPool, maxCount);
        FillObjectPool(ProjectileType.HealSpell, HealSpellPool, maxCount);
    }

    private void FillObjectPool(ProjectileType type, List<Projectile> pool, int maxCount)
    {
        ProjectileInfo info = GetProjectileInfo(type);
        if (info == null)
        {
            Debug.LogError($"{type}'s data is not attached.");
            return;
        }

        for (int index = 0; index < maxCount; index++)
        {
            GameObject gameObject = Instantiate(info.prefab);
            Projectile obj = gameObject.GetComponent<Projectile>();
            obj.gameObject.SetActive(false);
            pool.Add(obj);
        }
    }

    public Projectile GetObj(ProjectileType type, Vector3 position, Vector3 enemyPos)
    {
        Projectile projectile = null;

        switch (type)
        {
            case ProjectileType.Arrow:
                projectile = GetObjectPool(ArrowPool.Count, ArrowPool, position, enemyPos);
                break;

            case ProjectileType.AttackSpell:
                projectile = GetObjectPool(LightningSpellPool.Count, LightningSpellPool, position, enemyPos);
                break;

            case ProjectileType.HealSpell:
                projectile = GetObjectPool(HealSpellPool.Count, HealSpellPool, position, enemyPos);
                break;
        }
        if(projectile != null) return projectile;

        ProjectileInfo info = GetProjectileInfo(type);
        if(info == null)
        {
            Debug.LogError($"{type}'s data is not attached.");
            return null;
        }

        GameObject newObj = Instantiate(info.prefab);
        if (newObj)
        {
            Projectile newProjectile = newObj.GetComponent<Projectile>();
            newProjectile.gameObject.SetActive(false);

            switch (type)
            {
                case ProjectileType.Arrow:
                    ArrowPool.Add(newProjectile);
                    break;

                case ProjectileType.AttackSpell:
                    LightningSpellPool.Add(newProjectile);
                    break;

                case ProjectileType.HealSpell:
                    HealSpellPool.Add(newProjectile);
                    break;
            }
            return newProjectile;
        }
        return null;
    }
    private Projectile GetObjectPool(int count, List<Projectile> ObjectPool, Vector3 position, Vector3 enemyPos)
    {
        for (int i = 0; i < count; i++)
        {
            if (ObjectPool[i].gameObject.activeSelf == false)
            {
                Projectile projectile = ObjectPool[i];
                projectile.Initialize(position, enemyPos);
                projectile.gameObject.SetActive(true);
                return projectile;
            }
        }

        return null;
    }


    public void Release(Projectile obj)
    {
        obj.gameObject.SetActive(false);
    }

    private ProjectileInfo GetProjectileInfo(ProjectileType type)
    {
        return _projectileObjects.FirstOrDefault(projectile => projectile.type == type);
    }
}
