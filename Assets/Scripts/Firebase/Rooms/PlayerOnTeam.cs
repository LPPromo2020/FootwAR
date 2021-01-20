using UnityEngine;

/// <summary>
/// Permet de sauvegarder les informations d'un joueur
/// </summary>
public class PlayerOnTeam
{
    public string Guid;

    private Transform m_tranform;

    public PlayerOnTeam(string guid, Transform t = null) {
        Guid = guid;
        m_tranform = t;
    }

    // public Transform transform { set { m_tranform = value; } }

    public void UpdateTransform(string info) {
        float[] val = StringToFloat(info);

        m_tranform.position = new Vector3(val[0], val[1], val[2]);
        m_tranform.rotation = new Quaternion(val[3], val[4], val[5], val[6]);
        m_tranform.localScale = new Vector3(val[7], val[8], val[9]);
    }

    private float[] StringToFloat(string s) {
        string[] transformInfo = s.Split('¤');
        float[] values = new float[transformInfo.Length];

        for (int i = 0; i < values.Length; i++) {
            values[i] = float.Parse(transformInfo[i]);
        }

        return values;
    }

    public string ToJson() {
        return $"\"{Guid}\":" + "{\"prs\":\"x¤y¤z¤x¤y¤z¤x¤y¤z\"}";
    }
}
