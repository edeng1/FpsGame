public interface IDamageable
{
    bool getAwayTeam();
    void TakeDamage(float damage,int actorNumber,string gunName, bool headshot=false);
}