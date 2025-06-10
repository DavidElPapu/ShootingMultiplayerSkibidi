using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using TMPro;
using Newtonsoft.Json;
using Mirror.BouncyCastle.Utilities.IO;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine.Video;

public class Jugador : NetworkBehaviour
{
    #region Parametros
    private Rigidbody _rb;
    private InfoJugador _usernamePanel;

    [Header("Movimiento")]
    private Vector3 _moveDirection = new Vector3();
    public float moveSpeed = 10;
    public float speedLimit = 10;
    
    [Header("Gravedad")]
    public float gravityNormal = 50f;
    public float gravityJump = 9.81f;
    private bool _isJumping;

    [Header("Camara")]
    public Transform transformCam;
    public float camSpeed = 10f;
    private float _pitch = 0;
    private float _yaw = 0;
    private Vector2 _camInput = new Vector2();
    public float maxPitch = 60;
    public InputAction lookAction;

    [Header("Weapons")]
    public Transform transformCannon;

    [SyncVar(hook =nameof(OnKillChanged))]
    public int kills = 0;

    [Header("HP"), SyncVar(hook = nameof(HealthChanged))]
    private int hp = 5;
    private int maxHp = 10;
    public Transform healthBar;
    [SyncVar(hook = nameof(AliveHasChanged))]
    private bool isAlive = true;

    public float respawnTime = 5;

    [Header("NameTag")]
    public TextMeshPro nameTagObject;
    [SyncVar(hook =nameof(NameChanged))]
    private string username;

    [Header("Hats")]
    public Transform hatLocationTransform;
    public GameObject[] hatsGameObjects = new GameObject[5];
    [SyncVar(hook = nameof(HatChanged))]
    private int hatIndex =10;

    [Header("Team")]
    [SyncVar(hook = nameof(OnChangeTeam))]
    private Teams myTeam = Teams.None;

    [Header("UI")]
    public GameObject uiPanel;
    public PlayerHUD playerHUD;

    [Header("Animation")]
    public Animator animator;

    [Header("Body")]
    public GameObject[] noShowObjects = new GameObject[3];


    #endregion
    #region Unity
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
        maxHp = hp;
    }
    
    void FixedUpdate()
    {
        if(!isLocalPlayer)return;
        Vector3 flat = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
        Quaternion orientation = Quaternion.LookRotation(flat);
        Vector3 worldMoveDirection = orientation * _moveDirection;
        _rb.AddForce(worldMoveDirection * moveSpeed, ForceMode.Impulse);

        Vector3 horizontalVelocity = new Vector3(_rb.linearVelocity.x, 0, _rb.linearVelocity.z);
        if (horizontalVelocity.magnitude > speedLimit)
        {
            Vector3 limitedVelocity = horizontalVelocity.normalized * speedLimit;
            _rb.linearVelocity = new Vector3(limitedVelocity.x, _rb.linearVelocity.y, limitedVelocity.z);
        }

        if (!_isJumping)
        {
            _rb.AddForce(Vector3.down * gravityNormal, ForceMode.Acceleration);
        }
        animator.SetFloat("Velocity", horizontalVelocity.magnitude);
    }
    void Update()
    {
        //_camInput = lookAction.ReadValue<Vector2>();
        _camInput = Mouse.current.delta.ReadValue();
        _yaw += _camInput.x * camSpeed * Time.deltaTime;
        _pitch += _camInput.y * camSpeed * Time.deltaTime;

        _pitch = _pitch > maxPitch ? maxPitch : _pitch < (-maxPitch) ? -maxPitch : _pitch;
        transform.eulerAngles = new Vector3(0, _yaw,0);
        transformCam.eulerAngles = new Vector3(-_pitch, transformCam.rotation.eulerAngles.y, transformCam.rotation.eulerAngles.z);
    }
    #endregion
    #region PewPew
    [Command]
    private void CommandShoot(Vector3 origin,Vector3 direction )
    {
        if (Physics.Raycast(origin,direction, out RaycastHit hit, 100f))
        {
            if (hit.collider.gameObject.TryGetComponent<Jugador>(out Jugador hitPlayer) == true) 
            {
                if (hitPlayer.TakeDamage(1, myTeam)) 
                {
                    Debug.Log(name + "se papeo a: " + hitPlayer.name);
                    kills++;
                }
            }
        }
    }

    private void OnKillChanged(int oldKills, int newKills)
    {
        Debug.Log("ETC...");
    }
    #endregion

    #region HP
    private void HealthChanged(int oldHealth , int newHealth)
    {
        healthBar.localScale = new Vector3(healthBar.localScale.x, (float)newHealth / maxHp, healthBar.localScale.z);
        if (isLocalPlayer)
        {
            float foo = (float)newHealth / (float)maxHp;
            playerHUD.SetHP(foo);
        }
        if (newHealth < oldHealth)
            animator.SetTrigger("Hit");
    }

    [Server]
    public void IncreaseHealth(int amount)
    {
        hp = (hp + amount) > maxHp ? maxHp : hp + amount;
    }

    [Server]
    public bool TakeDamage(int amount, Teams elTeamo)
    {
        if (hp <= 0 || elTeamo == myTeam) { return false; }
        hp -= amount;
        if (hp <= 0) 
        {
            KillPlayer();
            return true;
        }
        return false;
    }

    [Server]
    private void KillPlayer()
    {
        isAlive = false;
    }

    private void AliveHasChanged(bool oldBool, bool newBool)
    {
        if (newBool == false) 
        {
            transform.localScale = new Vector3(10f, 10f, 10f);
            transformCam.gameObject.SetActive(false);
            gameObject.GetComponent<PlayerInput>().enabled = false;
            healthBar.gameObject.SetActive(false);
            if (!isLocalPlayer) return;
            Invoke("CommandRespawn", respawnTime);
            playerHUD.gameObject.SetActive(false);
            animator.SetBool("Death", true);
        }
        else
        {
            transform.localScale = Vector3.one;
            healthBar.gameObject.SetActive(true);
            transform.position=ShooterNetworkManager.singleton.GetStartPosition().position;
            if(!isLocalPlayer)
            {
                return;
            }
            gameObject.GetComponent<PlayerInput>().enabled = true;
            transformCam.gameObject.SetActive(true);
            playerHUD.gameObject.SetActive(false);
            animator.SetBool("Death", false);
        }
        
    }
    #endregion
    #region Input

    public void Shoot(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer)
        {
            return;
        }
        if (!context.performed)
        {
            return;
        }
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(.5f, .5f, 0));
        Vector3 targetPoint;
        if(Physics.Raycast(ray,out RaycastHit hit, 100f))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.origin + ray.direction * 100f;
        }

        Vector3 direction = (targetPoint - transformCannon.position).normalized;

        CommandShoot(transformCannon.position,direction);
    }

    public void SetMovement(InputAction.CallbackContext context)
    {
        if(!isLocalPlayer)return;
        Debug.Log("Moving");
        var dir = context.ReadValue<Vector2>().normalized;
        _moveDirection = new Vector3(dir.x, 0, dir.y);
        
    }

    public void ShowKills(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer) return;
        if (context.started)
        {
            Debug.Log("Started");
            uiPanel.SetActive(true);
            CommandGetKills();
        }
        else if (context.canceled) 
        {
            uiPanel.SetActive(false);
            Debug.Log("Canceled");
        }
    }
    [Command]
    private void CommandGetKills()
    {
        string content = "";
        var info = ScoreManager.singleton.GetSortedScore();
        foreach (var item in info)
        {
            content = content + "\u2022" + item.name + " - " + item.kills.ToString() + "<br>";
        }
        TargetShowKills(content);
    }
    [TargetRpc]
    private void  TargetShowKills(string infoClear)
    {
        uiPanel.GetComponent<UIManager>().ShowKills(infoClear);
    }

    public void SetLookDirection(InputAction.CallbackContext context)
    {
        //_camInput = context.ReadValue<Vector2>();
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        Cursor.lockState = CursorLockMode.Locked;
        _usernamePanel = GameObject.FindGameObjectWithTag("Username").GetComponent<InfoJugador>();
        name = _usernamePanel.PideUsuario();
        CommandChangeName(_usernamePanel.PideUsuario());
        CommandChangeHat(_usernamePanel.PideSombrero());
        _usernamePanel.gameObject.SetActive(false);
        CommandRegisterPlayer();
        uiPanel = FindAnyObjectByType<UIManager>(FindObjectsInactive.Include).gameObject;
        playerHUD = FindFirstObjectByType<PlayerHUD>(FindObjectsInactive.Include);
        playerHUD.gameObject.SetActive(true);

    }
    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        PlayerInput playerInput = GetComponent<PlayerInput>();
        playerInput.enabled = true;
        lookAction = playerInput.actions.actionMaps[0].actions[1];
        transformCam.gameObject.SetActive(true);
        nameTagObject.gameObject.SetActive(false);
        healthBar.gameObject.SetActive(false);
        //if (!isLocalPlayer) return;
        foreach (var part in noShowObjects)
        {
            part.SetActive(false);
        }
    }
    #endregion
    #region Mirror
    public override void OnStartServer()
    {
        base.OnStartServer();
        CommandSetTeam();
    }

    #endregion
    [Command]
    private void CommandChangeName(string myName)
    {
        username = myName;
    }

    private void NameChanged(string oldName, string newName)
    {
        nameTagObject.text = newName;
        name = newName;
    }

    [Command]
    private void CommandRespawn()
    {
        isAlive = true;
        hp = maxHp;
    }

    [Command]
    private void CommandChangeHat(int myHat)
    {
        hatIndex = myHat;
    }

    private void HatChanged(int oldHatIndex, int newHatIndex)
    {
        GameObject newHat = Instantiate(hatsGameObjects[newHatIndex], hatLocationTransform);
        if (!isLocalPlayer) return;
        newHat.SetActive(false);
    }

    [Server]
    private void CommandSetTeam()
    {
        myTeam = TeamManager.singleton.GetBalancedTeam();
        TeamManager.singleton.RegisterPlayer(this, myTeam);
    }

    private void OnChangeTeam(Teams oldTeam, Teams newTeam)
    {
        SetLook(newTeam);
    }
    private void SetLook(Teams elTeam)
    {
        var mat = noShowObjects[0].GetComponent<SkinnedMeshRenderer>().material;
        mat.SetFloat("_Toggle", elTeam == Teams.Alpha ? 0 : 1);

        if (!isLocalPlayer) return;
        var miMat = transformCam.GetChild(0).GetComponent<MeshRenderer>().materials[0]; 
        miMat.SetFloat("_Toggle", elTeam == Teams.Alpha ? 0 : 1);

        Debug.Log("" + "Soy " + elTeam.ToString() + " gurl");
    }

    [Command]
    private void CommandRegisterPlayer()
    {
        ScoreManager.singleton.RegisterPlayer(this);
    }
}
