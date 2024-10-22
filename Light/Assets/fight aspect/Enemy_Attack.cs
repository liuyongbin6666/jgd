using System.Collections;
using Components;
using GameData;
using Sirenix.OdinInspector;
using UnityEngine;
using Utls;

namespace fight_aspect
{
    /// <summary>
    /// 怪物攻击组件
    /// </summary>
    public class Enemy_Attack : TrackingComponentBase
    {
        public BulletManager bulletManager;
        [SerializeField,LabelText("攻击CD")]float cd= 1f;
        [SerializeField,LabelText("请选择射击方式")] BulletTracking bulletTracking;
        float startTime;
        PlayerControlComponent player;
        bool isBusy;
        protected override void OnTrackingEnter(GameObject go)
        {
            if (player) return;
            player = go.GetPlayerControlFromColliderHandler();
            if(isBusy)return;
            StartComponent();
        }
        protected override void OnTrackingExit(GameObject go)
        {
            if (player.gameObject == go) player = null;
        }

        void StartComponent()
        {
            isBusy = true;
            StartCoroutine(CountingColdDown());
        }

        void ResetComponent()
        {
            isBusy = false;
            StopCoroutine(CountingColdDown());
        }
        IEnumerator CountingColdDown()
        {
            while (true)
            {
                bulletManager.Shoot(transform, player, bulletTracking);
                yield return new WaitForSeconds(cd);
                "Cd 结束".Log(this);
                yield return new WaitUntil(() => player);
                $"{player.name}存在，攻击！".Log(this);
            }
        }
        void OutAttackRange(Collider col)
        {
            if (col.tag != GameTag.Player) return;
            player = null;
        }
        void InAttackRange(Collider col)
        {
            if (col.tag != GameTag.Player) return;
            player = col.GetPlayerControlFromColliderHandler();
        }
    }
}
    