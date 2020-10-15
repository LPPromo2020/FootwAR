public class User
{
    public int nbDefeats, nbVictorys;
    private string m_sID, m_sPseudo, m_sEmail;

    public User(string id, string pseudo, string email, int v, int d)
    {
        m_sID = id;
        m_sPseudo = pseudo;
        m_sEmail = email;
        nbVictorys = v;
        nbDefeats = d;
    }

    public string ID() => m_sID;
    public string PSEUDO() => m_sPseudo;
}
