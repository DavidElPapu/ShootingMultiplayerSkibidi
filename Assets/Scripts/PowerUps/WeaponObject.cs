using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "ScriptableObjects/Weapon", order = 0)]
public class WeaponObject : ScriptableObject
{
    public bool usesAmmo = false;
    public int ammo = 0;
    public int index;
    public bool hitScan = false;
    public int damage = 1;

    public WeaponData ToData()
    {
        return new WeaponData(usesAmmo, ammo, index, hitScan, damage);
    }
}

public struct WeaponData
{
    public bool usesAmmo;
    public int ammo;
    public int index;
    public bool hitScan;
    public int damage;

    public WeaponData(bool usesAmmo, int ammo, int index, bool hitScan, int damage)
    {
        this.usesAmmo = usesAmmo;
        this.ammo = ammo;
        this.index = index;
        this.hitScan = hitScan;
        this.damage = damage;
    }
}
