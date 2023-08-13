using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using TMPro;

public class ManageGame : MonoBehaviour
{
    public GameObject acabouText;
    public Door terminarDoor;
    public bool isacabando;
    public bool foge;
    public SpectatorManager spectatorManager;
        public Generator[] allGen;
        public List<Generator> activeGenerators = new List<Generator>();
        public bool ativados;
        public TMP_Text genText;
    public int playersPost;
    public TMP_Text alive;

    public void Awake()
    {
                allGen = FindObjectsOfType<Generator>();

    }

    public void Start()
    {
        GameObject.FindObjectOfType<DiscordPresence>().ChangePresence("In Game", "");
    }

    public void Update()
    {
        genText.text="Generators Left: \n"+activeGenerators.Count+"/"+allGen.Length.ToString();
        alive.text = "Players Left: \n"+(PhotonNetwork.PlayerList.Length-spectatorManager.playersMortos).ToString()+"/"+PhotonNetwork.PlayerList.Length.ToString();

        // Limpa a lista de geradores ativos
        activeGenerators.Clear();

        // Procura por todos os objetos Generators na cena
        Generator[] allGenerators = FindObjectsOfType<Generator>();

        // Adiciona apenas os geradores ativados à lista de geradores ativos
        foreach (Generator generator in allGenerators)
        {
            if (generator.Ativada)
            {
                activeGenerators.Add(generator);
            }
        }
        foreach(Generator a in allGen)
        {
            ativados=true;
            if (!a.Ativada)
            {
                ativados = false;
                break;
            }
        }
        if(ativados && !terminarDoor.isOpen)
        {
            terminarDoor.Mudar();
        }
        foge = terminarDoor.isOpen && ativados;
    }

    public void AcabarJogoo()
    {
        if(!isacabando)
        {
            isacabando=true;
            AcabarJogo();
        }
    }

    public void AcabarJogo()
    {
        StartCoroutine(acabarGame());
        acabouText.active=true;
    }

    IEnumerator acabarGame()
    {
        yield return new WaitForSeconds(3f);
        if(SceneManager.GetActiveScene().name!="Lobby")
        {
        PhotonNetwork.LoadLevel("Lobby");
        PhotonNetwork.CurrentRoom.IsOpen=true;
        }
        yield break;
    }

    [PunRPC]
    public void addPlayers()
    {
        playersPost++;
    }
}