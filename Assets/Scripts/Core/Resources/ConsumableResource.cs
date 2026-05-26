namespace Core.Resources
{
    public sealed class ConsumableResource : Resource
    {
        public bool TryUse(float amount = 1f)
        {
            return TrySpend(amount);
        }

        public void Add(float amount)
        {
            Increase(amount);
        }

        public void Remove(float amount)
        {
            Decrease(amount);
        }
    }
}