using UnityEngine;
using System.Collections.Generic;

public class ProjectileGroup : ProjectileScript
{
    private List<ProjectileScript> projectiles = new List<ProjectileScript>();
    private Vector3 groupCenter = Vector3.zero;
    public List<ProjectileScript> Projectiles { get => projectiles; }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        
        // Update all projectiles in the group
        for (int i = projectiles.Count - 1; i >= 0; i--)
        {
            ProjectileScript projectile = projectiles[i];
            if (projectile == null || projectile.gameObject == null)
            {
                projectiles.RemoveAt(i);
            }
            else
            {
                // Call Update on each child projectile to maintain their lifecycle
                projectile.enabled = true;
            }
        }
    }

    protected override void Move()
    {
        // Composite pattern: delegate Move to all child projectiles
        foreach (ProjectileScript projectile in projectiles)
        {
            if (projectile != null && projectile.gameObject.activeSelf)
            {
                // Call Move on each child to update their positions
                projectile.SetDirection(projectile.MoveDirection);
                projectile.transform.position += projectile.MoveDirection * projectile.MoveSpeed * Time.deltaTime;
            }
        }
        
        // Group itself also moves (can be used for group-level movement)
        base.Move();
    }

    public override void SetDirection(Vector3 direction)
    {
        // Composite: apply direction to all children
        base.SetDirection(direction);
        SetDirectionAll(direction);
    }

    public override void SetSpeed(float speed)
    {
        // Composite: apply speed to all children
        base.SetSpeed(speed);
        SetSpeedAll(speed);
    }

    public void AddProjectile(ProjectileScript projectile)
    {
        if (projectile != null && !projectiles.Contains(projectile))
        {
            projectiles.Add(projectile);
        }
    }

    public void RemoveProjectile(ProjectileScript projectile)
    {
        if (projectile != null)
        {
            projectiles.Remove(projectile);
        }
    }

    public void AddProjectiles(List<ProjectileScript> newProjectiles)
    {
        foreach (ProjectileScript projectile in newProjectiles)
        {
            AddProjectile(projectile);
        }
    }

    public void Clear()
    {
        projectiles.Clear();
    }

    public void SetDirectionAll(Vector3 direction)
    {
        foreach (ProjectileScript projectile in projectiles)
        {
            if (projectile != null)
            {
                projectile.SetDirection(direction);
            }
        }
    }

    public void SetSpeedAll(float speed)
    {
        foreach (ProjectileScript projectile in projectiles)
        {
            if (projectile != null)
            {
                projectile.SetSpeed(speed);
            }
        }
    }

    public Vector3 GetGroupCenter()
    {
        if (projectiles.Count == 0)
            return Vector3.zero;

        Vector3 center = Vector3.zero;
        foreach (ProjectileScript projectile in projectiles)
        {
            if (projectile != null)
            {
                center += projectile.transform.position;
            }
        }

        groupCenter = center / projectiles.Count;
        return groupCenter;
    }

    public void DestroyAll()
    {
        foreach (ProjectileScript projectile in projectiles)
        {
            if (projectile != null)
            {
                Destroy(projectile.gameObject);
            }
        }
        projectiles.Clear();
    }

    public List<ProjectileScript> GetProjectiles()
    {
        return new List<ProjectileScript>(projectiles);
    }

    public int GetProjectileCount()
    {
        return projectiles.Count;
    }
}