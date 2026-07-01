namespace KnightOnline
{
    public interface IDamageable
    {
        void TakeDamage(float damage);
        void TakeDamage(int damage, bool isCritical);
    }
}
