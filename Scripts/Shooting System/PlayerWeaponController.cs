using UnityEngine;
using StarterAssets;

public class PlayerWeaponController : MonoBehaviour
{
    [Header("References Settings")]
    [SerializeField] private GameObject[] objWeapons;
    [SerializeField] private StarterAssetsInputs _inputs;

    private int _selectedIndexWeapon = 0; // или ссылку на выбранное оружее 
    private Weapon[] _weapons;

    private void OnEnable()
    {
        _inputs.OnPlayerFire += OnFire;
        _inputs.OnPlayerSwitchWeapon += OnSwitchedWeapon;
    }

    private void Start()
    {
        InitWeapons();
        ActivSelectedWeapon();
    }

    private void OnDisable()
    {
        _inputs.OnPlayerFire -= OnFire;
        _inputs.OnPlayerSwitchWeapon -= OnSwitchedWeapon;
    }

    private void InitWeapons()
    {
        _weapons = new Weapon[objWeapons.Length];

        for (int i = 0; i < objWeapons.Length; i++)
        {
            _weapons[i] = objWeapons[i].GetComponent<Weapon>();
        }
    }

    private void ActivSelectedWeapon()
    {
        for (int i = 0; i < objWeapons.Length; i++)
        {
            objWeapons[i].SetActive(false);
        }

        objWeapons[_selectedIndexWeapon].SetActive(true);
    }

    private void OnFire()
    {
        _weapons[_selectedIndexWeapon].TryShoot();
    }

    private void OnSwitchedWeapon()
    {
        _selectedIndexWeapon = _selectedIndexWeapon < _weapons.Length - 1 ?
            _selectedIndexWeapon + 1 : 0;

        ActivSelectedWeapon();
    }
}
