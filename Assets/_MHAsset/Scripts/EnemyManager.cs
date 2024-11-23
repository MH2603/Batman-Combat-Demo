using UnityEngine;
using MH;
using System.Collections.Generic;

namespace MH
{

    public class EnemyManager : MonoBehaviour
    {
        #region -------------------- Fields -------------------
        public CombatController Player;

        #endregion

        #region -------------------- Properties -------------------

        [SerializeField] private List<EnemyController> enemies = new();
        private EnemyController attackEnemy;
        private int indexAttackEnemy = 0;

        #endregion

        #region -------------------- Unity Methods -------------------

        private void Start()
        {
            Init(); 
        }


        #endregion

        #region -------------------- Public Methods -------------------

        //public void RegisterEnemy(EnemyController enemy)
        //{
        //    if (enemies.IndexOf(enemy) == -1) enemies.Add(enemy);
        //}

        public void OnSortBehavior()
        {
            SortEnemyBehavior();
        }

        public EnemyController GetAttackEnemy()
        {
            return attackEnemy;
        }

        public EnemyController GetRandomEnemy()
        {
            return enemies[Random.Range(0, enemies.Count)];
        }

        #endregion

        #region -------------------- Private Methods -------------------

        private void Init()
        {
            indexAttackEnemy = 0;   
            attackEnemy  = null;

            foreach (var enemy in enemies)
            {
                enemy.Init();   
            }

            SortEnemyBehavior();
        }

        private void SortEnemyBehavior()
        {
            attackEnemy = enemies[indexAttackEnemy % enemies.Count];
            indexAttackEnemy ++;

            attackEnemy.OnChase();

            foreach (var enemy in enemies)
            {
                if (enemy != attackEnemy)
                {
                    enemy.ChoseRequireState();
                }
            }
        }

        

        #endregion

    }

}
