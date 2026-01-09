using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Domain
{
    public abstract class Weapon : MonoBehaviour
    {
        protected DamageParameter damageParameter = new DamageParameter();

        protected abstract void Update();
    }
}
