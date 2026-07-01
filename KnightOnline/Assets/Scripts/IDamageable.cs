namespace KnightOnline
{
    public interface IDamageable
    {
        void TakeDamage(int damage);
        void TakeDamage(int damage, bool isCritical);
        void TakeDamage(float damage);
    }
}
