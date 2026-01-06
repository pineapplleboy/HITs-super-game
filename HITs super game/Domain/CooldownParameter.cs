namespace Domain
{
    public class CooldownParameter
    {
        public float Cooldown { get; set; } = 1f;
        public float CurrentCd { get; set; } = 0f;

        public CooldownParameter(float cooldown, float currentCd)
        {
            Cooldown = cooldown;
            CurrentCd = currentCd;
        }
    }
}
