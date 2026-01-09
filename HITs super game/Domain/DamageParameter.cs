namespace Domain
{
    public class DamageParameter
    {
        public int Damage { get; set; } = 50;
        public float CritRate { get; set; } = 4;

        public DamageParameter(int damage, float critRate)
        {
            Damage = damage;
            CritRate = critRate;
        }

        public DamageParameter()
        {
            
        }
    }
}
