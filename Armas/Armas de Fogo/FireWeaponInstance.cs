using UnityEngine;


[System.Serializable]
public class FireWeaponInstance
{
    public WeaponItemScriptable weaponData;
    public int currentAmmo;         // munição atual
    public string instanceID;
    public bool equipado;


    public FireWeaponInstance(WeaponItemScriptable data, string weaponId)
    {
        weaponData = data;
        instanceID = weaponId;
        currentAmmo = Random.Range(0, data.municaoMaxima + 1);
        equipado = false;
    }
}
