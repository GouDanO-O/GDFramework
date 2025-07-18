using System;
using GDFramework.Utility;
using GDFrameworkCore;
using GDFrameworkExtend.StorageKit;
using UnityEngine;

namespace Game.World.Player
{
    public class PlayerHealthyModel : AbstractModel
    {
        [AutoSave]
        public BindableProperty<int> CurrentHealth = new BindableProperty<int>();

        [AutoSave]
        public BindableProperty<int> MaxHealth = new BindableProperty<int>();
        
        [AutoSave]
        public BindableProperty<int> CurrentArmor = new BindableProperty<int>();

        [AutoSave]
        public BindableProperty<int> MaxArmor = new BindableProperty<int>();

        public BindableProperty<bool> IsDeath = new BindableProperty<bool>();

        public BindableProperty<bool> IsInvincible = new BindableProperty<bool>();
        
        protected override void OnInit()
        {
            
        }

        public void InitPlayerHealthyModel()
        {
            RegisterBindableProperties();
        }
        
        private void RegisterBindableProperties()
        {
            if (GameManager.Instance.IsNewGame())
            {
                this.CurrentArmor.Value = this.MaxArmor.Value;
                this.CurrentHealth.Value =this.MaxHealth.Value;
            
                this.IsDeath.SetValueWithoutEvent(false);
                this.IsInvincible.Value = false;
            }

        }

        /// <summary>
        /// 受到伤害
        /// </summary>
        /// <param name="damage"></param>
        public void BeHarmed(int damage)
        {
            if(IsDeath.Value || IsInvincible.Value)
                return;

            int lastDamage = CurrentArmor.Value - damage;
            if (lastDamage >= 0)
            {
                ReduceArmor((damage));
            }
            else
            {
                SetArmor(0);
                ReduceHealth(Math.Abs(lastDamage));
            }

            CheckIsDead();
        }

        public void CheckIsDead()
        {
            if (CurrentHealth.Value <= 0)
            {
                IsDeath.Value = true;
            }
        }

        /// <summary>
        /// 设置临时护甲值
        /// </summary>
        /// <param name="armor"></param>
        public void SetArmor(int armor)
        {
            CurrentArmor.Value = armor;
            if (CurrentArmor.Value > MaxArmor.Value)
            {
                CurrentArmor.Value = MaxArmor.Value;
            }
        }

        /// <summary>
        /// 设置临时生命值
        /// </summary>
        /// <param name="health"></param>
        public void SetHealth(int health)
        {
            CurrentHealth.Value = health;
            if (CurrentHealth.Value > MaxHealth.Value)
            {
                CurrentHealth.Value = MaxHealth.Value;
            }
        }
        
        /// <summary>
        /// 减少护甲
        /// </summary>
        /// <param name="damage"></param>
        public void ReduceArmor(int damage)
        {
            CurrentArmor.Value -= damage;
        }

        /// <summary>
        /// 减少生命
        /// </summary>
        /// <param name="damage"></param>
        public void ReduceHealth(int damage)
        {
            CurrentHealth.Value -= damage;
        }

        /// <summary>
        /// 增加护甲
        /// </summary>
        /// <param name="armor"></param>
        public void IncreaseArmor(int armor)
        {
            CurrentArmor.Value += armor;
        }

        /// <summary>
        /// 增加生命
        /// </summary>
        /// <param name="health"></param>
        public void IncreaseHealth(int health)
        {
            CurrentHealth.Value += health;
        }

        /// <summary>
        /// 受到伤害
        /// </summary>
        /// <param name="damage"></param>
        public void BeHarmed(float damage)
        {
            BeHarmed((int)damage);
        }
    }
}