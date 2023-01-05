using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LightningWeapon : WeaponSpawner
{

    bool _isAttack = false;
    private GameObject _playerUI = null;
    Image _image_skill;

    void Awake()
    {
        _weaponID = 5;
        _playerUI = GameObject.FindWithTag("PlayerUI");
        if (object.ReferenceEquals(_playerUI, null))
        {
            Managers.Resource.Destroy(gameObject);
            return;
        }

        _image_skill = Util.FindChild<Image>(_playerUI, "CursorCoolTimeImg");
        
    }

    private void OnEnable()
    {
        _image_skill.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        _image_skill.gameObject.SetActive(false);
    }

    void Update()
    {
        UpdateSkillCoolTimeImage();
        if (Input.GetMouseButtonDown(0))
        {
            if (!_isAttack)
            {
                SetWeaponStat();
                StartCoroutine(DamageCoolTime());
                StartCoroutine(LightnigEffect());
                Collider2D[] collider2Ds = Physics2D.OverlapCircleAll(Camera.main.ScreenToWorldPoint(Managers.Game.MousePos), _size, LayerMask.GetMask("Enemy"));
                foreach (Collider2D coll in collider2Ds)
                {
                    GameObject go = coll.gameObject;
                    go.GetComponent<EnemyController>().OnDamaged(_damage);
                }
                Debug.Log($"Lightning Attack! Give {_damage} to Enemy {collider2Ds.Length}");
            }
        }
    }

    void SetWeaponStat()
    {
        base.SetWeaponStat();
    }

    void UpdateSkillCoolTimeImage()
    {
        _image_skill.transform.position = Managers.Game.MousePos;
    }

    IEnumerator DamageCoolTime()
    {
        _isAttack = true;
        float currentCooltime = _cooldown;
        while (currentCooltime > 0f)
        {
            currentCooltime -= Time.deltaTime;
            _image_skill.fillAmount = ((_cooldown - currentCooltime) / _cooldown);
            yield return new WaitForFixedUpdate();

        }
        _isAttack = false;
    }
    IEnumerator LightnigEffect()
    {
        GameObject lightnigEffect = Managers.Game.Spawn(Define.WorldObject.Unknown, "Weapon/Lightning");
        lightnigEffect.transform.position = Camera.main.ScreenToWorldPoint(Managers.Game.MousePos) - new Vector3(0,0, Camera.main.transform.position.z);
        yield return new WaitForSeconds(0.5f);
        Managers.Resource.Destroy(lightnigEffect);
    }
}
