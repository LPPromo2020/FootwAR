﻿using System.Collections.Generic;

public enum TeamColor { BLUE, RED, SPECTATOR };

/// <summary>
/// Classe qui permet de sauvegarder les données d'une équipe
///   -> Trois type d'équipe
///       -> Rouge
///       -> Bleu
///       -> Spectator
///   -> Liste des joueurs dans cette équipe
///
/// On peut convertir les informations de l'équipe en JSON
/// La vérification de si l'équipe possédent le joueur est implémenté
/// </summary>
public class Team
{
    private List<PlayerOnTeam> m_lstpList = new List<PlayerOnTeam>();
    private TeamColor m_tcColor;

    public Team(TeamColor color) { m_tcColor = color; }

    public void AddPlayer(PlayerOnTeam p) => m_lstpList.Add(p);
    public List<PlayerOnTeam> AllPlayer() => m_lstpList;
    public TeamColor Color() => m_tcColor;

    public bool HaveThisPlayer(string guid) {
        bool ret = false;
        m_lstpList.ForEach(p => { if (p.Guid == guid) ret = true; });
        return ret;
    }

    public PlayerOnTeam GetPlayer(string guid) {
        return m_lstpList.Find(p => p.Guid == guid);
    }

    public string ToJson() {
        string json = "{";
        m_lstpList.ForEach(player => json += player.ToJson() + ',');
        return json + "\"score\":0 }";
    }
}
