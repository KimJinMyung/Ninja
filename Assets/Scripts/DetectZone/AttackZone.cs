using Player_State.Extension;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AttackZone : MonoBehaviour
{
    Player ownerPlayer;
    Monster ownerMonster;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") && !other.CompareTag("Monster")) return;

        if (other.CompareTag("Player"))
        {
            Player player = other.transform.root.GetComponent<Player>();
            ownerMonster = transform.root.GetComponentInChildren<Monster>();

            if (ownerMonster == null) return;

            Player_data player_Data = player.InputVm.player_Data;
            player_Data.HP -= ownerMonster.MonsterViewModel.MonsterInfo.ATK;
            player.InputVm.RequestOnPlayerInfo(player_Data.PlayerId, player_Data);

        }
        else
        {


            Monster monster = other.transform.GetComponent<Monster>();
            Debug.Log(monster.monsterId);
            ownerPlayer = transform.root.GetComponent<Player>();

            if (ownerPlayer == null) return;

            monster.Hurt(monster.monsterId,ownerPlayer);
            //Monster_data monster_info = monster.MonsterViewModel.MonsterInfo;
            //monster_info.HP -= ownerPlayer.InputVm.player_Data.ATK;
            //monster.MonsterViewModel.RequestMonsterInfoChanged(monster.monsterId, monster_info);
            //if(monster.Monster_Info.HP > 0)
            //{
            //    Vector3 moveDir = (monster.transform.position - ownerPlayer.transform.position).normalized;
            //    monster.transform.position = Vector3.Lerp(monster.transform.position, monster.transform.position + moveDir * 3f, 10f / Time.deltaTime);
            //}            
        }
    }
}
